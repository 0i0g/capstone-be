using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.ReceiveVoucher;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class ReceiveVoucherService : BaseService, IReceiveVoucherService
    {
        private readonly IReceiveVoucherRepository _receiveVoucherRepository;

        private readonly IQueryable<ReceiveVoucher> _receiveVoucherQueryable;
        private readonly IQueryable<ReceiveVoucher> _receiveVoucherAllWarehouseQueryable;
        private readonly IQueryable<ReceiveRequestVoucher> _receiveRequestVoucherQueryable;
        private readonly IQueryable<Customer> _customerQueryable;
        private readonly IQueryable<Product> _productQueryable;

        public ReceiveVoucherService(IServiceProvider provider) : base(provider)
        {
            _receiveVoucherRepository = _unitOfWork.ReceiveVoucher;
            _receiveVoucherQueryable =
                _receiveVoucherRepository.GetMany(x => x.IsDeleted == false && x.WarehouseId == CurrentUser.Warehouse);
            _receiveVoucherAllWarehouseQueryable = _receiveVoucherRepository.GetMany(x => x.IsDeleted == false);
            _receiveRequestVoucherQueryable =
                _unitOfWork.ReceiveRequestVoucher.GetMany(x =>
                    x.IsDeleted == false && x.WarehouseId == CurrentUser.Warehouse &&
                    x.Status == EnumStatusRequest.Confirmed);
            _customerQueryable = _unitOfWork.Customer.GetMany(x => x.IsDeleted == false);
            _productQueryable = _unitOfWork.Product.GetMany(x => x.IsDeleted == false && x.IsActive == true);
        }

        public async Task<IActionResult> CreateReceiveVoucher(CreateReceiveVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            if (model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.NotFound(MessageConstant.CustomerNotFound);
            }

            var receiveVoucher = new ReceiveVoucher()
            {
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                Status = EnumStatusVoucher.Pending,
                Locked = false,
                CustomerId = model.CustomerId,
                WarehouseId = (Guid)CurrentUser.Warehouse!,
            };

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.ReceiveVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductReceiveVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is { Count: > 0 })
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));

                if (model.RequestId != null)
                {
                    var request = _receiveRequestVoucherQueryable.Include(x => x.Details)
                        .FirstOrDefault(x => x.Id == model.RequestId);
                    if (request == null)
                    {
                        return ApiResponse.NotFound(MessageConstant.ReceiveRequestVoucherNotFound);
                    }

                    if (CurrentUser.Warehouse != request.WarehouseId)
                    {
                        return ApiResponse.NotFound(MessageConstant.RequiredWarehouseRequestVoucher);
                    }

                    var modelProductIds = model.Details.Select(x => x.ProductId).ToList();
                    var requestProductIds = request.Details.Select(x => x.ProductId).ToList();
                    var conflictProductIds = modelProductIds.Except(requestProductIds).ToList();
                    if (conflictProductIds is { Count: > 0 })
                    {
                        return ApiResponse.BadRequest(
                            MessageConstant.ProductsNotFoundInRequestDetails.WithValues(string.Join(", ",
                                conflictProductIds)));
                    }

                    receiveVoucher.RequestId = model.RequestId;
                }

                receiveVoucher.Details = model.Details.Select(x => new ReceiveVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = receiveVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _receiveVoucherRepository.Add(receiveVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateReceiveVoucher(UpdateReceiveVoucherModel model)
        {
            var receiveVoucher = _receiveVoucherQueryable
                .Include(x => x.Details)
                .Include(x => x.Request).ThenInclude(x => x.Details)
                .FirstOrDefault(x => x.Id == model.Id);
            if (receiveVoucher == null)
                return ApiResponse.NotFound(MessageConstant.ReceiveVoucherNotFound);

            if (receiveVoucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateReceiveVoucher);

            if (model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.NotFound(MessageConstant.CustomerNotFound);
            }

            receiveVoucher.ReportingDate = model.ReportingDate ?? receiveVoucher.ReportingDate;
            receiveVoucher.Description = model.Description ?? receiveVoucher.Description;
            receiveVoucher.CustomerId = model.CustomerId ?? receiveVoucher.CustomerId;
            receiveVoucher.Status = model.Status ?? receiveVoucher.Status;

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.ReceiveVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductReceiveVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is { Count: > 0 })
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));

                if (receiveVoucher.Request != null)
                {
                    if (CurrentUser.Warehouse != receiveVoucher.Request.WarehouseId)
                    {
                        return ApiResponse.NotFound(MessageConstant.RequiredWarehouseRequestVoucher);
                    }

                    var modelProductIds = model.Details.Select(x => x.ProductId).ToList();
                    var requestProductIds = receiveVoucher.Request.Details.Select(x => x.ProductId).ToList();
                    var conflictProductIds = modelProductIds.Except(requestProductIds).ToList();
                    if (conflictProductIds is { Count: > 0 })
                    {
                        return ApiResponse.BadRequest(
                            MessageConstant.ProductsNotFoundInRequestDetails.WithValues(string.Join(", ",
                                conflictProductIds)));
                    }
                }

                receiveVoucher.Details.Clear();
                receiveVoucher.Details = model.Details.Select(x => new ReceiveVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = receiveVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _receiveVoucherRepository.Update(receiveVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulReceiveVoucher(Guid id)
        {
            var receiveVoucher = _receiveVoucherQueryable.Include(x => x.Details)
                .FirstOrDefault(x => x.Id == id);
            if (receiveVoucher == null)
                return ApiResponse.NotFound(MessageConstant.ReceiveVoucherNotFound);

            receiveVoucher.IsDeleted = true;
            _receiveVoucherRepository.Update(receiveVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Lock(Guid id)
        {
            var receiveVoucher = _receiveVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (receiveVoucher == null)
                return ApiResponse.NotFound(MessageConstant.ReceiveVoucherNotFound);

            receiveVoucher.Locked = true;
            _receiveVoucherRepository.Update(receiveVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Unlock(Guid id)
        {
            var receiveVoucher = _receiveVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (receiveVoucher == null)
                return ApiResponse.NotFound(MessageConstant.ReceiveVoucherNotFound);

            receiveVoucher.Locked = false;
            _receiveVoucherRepository.Update(receiveVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetReceiveVoucher(Guid id)
        {
            var receiveVoucher = _receiveVoucherQueryable.Where(x => x.Id == id).Select(x =>
                new ReceiveVoucherViewModel()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ReportingDate = x.ReportingDate,
                    Description = x.Description,
                    Status = x.Status,
                    Locked = x.Locked,
                    CreatedAt = x.CreatedAt,
                    CreateBy = x.CreatedBy == null
                        ? null
                        : new FetchUserViewModel
                        {
                            Id = x.CreatedBy.Id,
                            Name = x.CreatedBy.FullName,
                            Avatar = x.CreatedBy.Avatar
                        },
                    Request = x.Request == null
                        ? null
                        : new FetchReceiveRequestVoucherViewModel()
                        {
                            Id = x.Request.Id,
                            Code = x.Request.Code
                        },
                    Customer = x.Customer == null
                        ? null
                        : new FetchCustomerViewModel()
                        {
                            Id = x.Customer.Id,
                            Name = x.Customer.Name
                        },
                    Warehouse = x.Warehouse == null
                        ? null
                        : new FetchWarehouseViewModel()
                        {
                            Id = x.Warehouse.Id,
                            Name = x.Warehouse.Name
                        },
                    Details = x.Details == null
                        ? null
                        : x.Details.Select(y => new ReceiveVoucherDetailViewModel()
                        {
                            Id = y.Id,
                            RequestQuantity = x.Request == null
                                ? null
                                : x.Request.Details.FirstOrDefault(z => z.ProductId == y.ProductId).Quantity,
                            VoucherQuantity = y.Quantity,
                            ProductName = y.ProductName,
                            Product = y.Product == null
                                ? null
                                : new FetchProductViewModel()
                                {
                                    Id = y.Product.Id,
                                    Name = y.Product.Name
                                }
                        }).OrderBy(y => y.ProductName).ToList()
                }).FirstOrDefault();

            if (receiveVoucher == null) return ApiResponse.NotFound(MessageConstant.ReceiveVoucherNotFound);

            return ApiResponse.Ok(receiveVoucher);
        }

        public IActionResult FetchReceiveVoucher(FetchModel model)
        {
            var receiveVouchers = _receiveVoucherQueryable.AsNoTracking()
                .Where(x => string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size)
                .Select(x =>
                    new FetchReceiveVoucherViewModel()
                    {
                        Id = x.Id,
                        Code = x.Code
                    }).ToList();

            return ApiResponse.Ok(receiveVouchers);
        }

        public IActionResult SearchReceiveVoucherInWarehouse(SearchReceiveVoucherInWarehouseModel model)
        {
            var query = _receiveVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (string.IsNullOrWhiteSpace(model.Customer) || x.Customer.Name.Contains(model.Customer))
            );

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                case "CODE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Code).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Code).ThenByDescending(x => x.CreatedAt);
                    break;
                case "REPORTINGDATE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.ReportingDate).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.ReportingDate).ThenByDescending(x => x.CreatedAt);
                    break;
                case "STATUS":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Status).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Status).ThenByDescending(x => x.CreatedAt);
                    break;
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Code, CreatedAt, ReportingDate, Status"));
            }

            var data = query.Select(x => new SearchReceiveVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Customer = x.Customer == null
                    ? null
                    : new FetchCustomerViewModel()
                    {
                        Id = x.Customer.Id,
                        Name = x.Customer.Name
                    },
                Warehouse = x.Warehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.Warehouse.Id,
                        Name = x.Warehouse.Name
                    },
                Request = x.Request == null
                    ? null
                    : new FetchReceiveRequestVoucherViewModel()
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    }
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchReceiveVoucherByWarehouse(SearchReceiveVoucherByWarehouseModel model)
        {
            var query = _receiveVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (string.IsNullOrWhiteSpace(model.Customer) || x.Customer.Name.Contains(model.Customer)) &&
                (model.WarehouseId == x.WarehouseId)
            );

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                case "CODE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Code).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Code).ThenByDescending(x => x.CreatedAt);
                    break;
                case "REPORTINGDATE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.ReportingDate).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.ReportingDate).ThenByDescending(x => x.CreatedAt);
                    break;
                case "STATUS":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Status).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Status).ThenByDescending(x => x.CreatedAt);
                    break;
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Code, CreatedAt, ReportingDate, Status"));
            }

            var data = query.Select(x => new SearchReceiveVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Customer = x.Customer == null
                    ? null
                    : new FetchCustomerViewModel()
                    {
                        Id = x.Customer.Id,
                        Name = x.Customer.Name
                    },
                Warehouse = x.Warehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.Warehouse.Id,
                        Name = x.Warehouse.Name
                    },
                Request = x.Request == null
                    ? null
                    : new FetchReceiveRequestVoucherViewModel()
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    }
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchReceiveVoucherAllWarehouse(SearchReceiveVoucherAllWarehouseModel model)
        {
            var query = _receiveVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (string.IsNullOrWhiteSpace(model.Customer) || x.Customer.Name.Contains(model.Customer))
            );

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                case "CODE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Code).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Code).ThenByDescending(x => x.CreatedAt);
                    break;
                case "REPORTINGDATE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.ReportingDate).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.ReportingDate).ThenByDescending(x => x.CreatedAt);
                    break;
                case "STATUS":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Status).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Status).ThenByDescending(x => x.CreatedAt);
                    break;
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Code, CreatedAt, ReportingDate, Status"));
            }

            var data = query.Select(x => new SearchReceiveVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Customer = x.Customer == null
                    ? null
                    : new FetchCustomerViewModel()
                    {
                        Id = x.Customer.Id,
                        Name = x.Customer.Name
                    },
                Warehouse = x.Warehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.Warehouse.Id,
                        Name = x.Warehouse.Name
                    },
                Request = x.Request == null
                    ? null
                    : new FetchReceiveRequestVoucherViewModel()
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    }
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }
    }
}