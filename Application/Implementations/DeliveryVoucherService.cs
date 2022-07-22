using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.DeliveryRequestVoucher;
using Application.ViewModels.DeliveryVoucher;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class DeliveryVoucherService : BaseService, IDeliveryVoucherService
    {
        private readonly IDeliveryVoucherRepository _deliveryVoucherRepository;

        private readonly IQueryable<DeliveryVoucher> _deliveryVoucherQueryable;
        private readonly IQueryable<DeliveryVoucher> _deliveryVoucherAllWarehouseQueryable;
        private readonly IQueryable<DeliveryRequestVoucher> _deliveryRequestVoucherQueryable;
        private readonly IQueryable<Customer> _customerQueryable;
        private readonly IQueryable<Product> _productQueryable;

        public DeliveryVoucherService(IServiceProvider provider) : base(provider)
        {
            _deliveryVoucherRepository = _unitOfWork.DeliveryVoucher;
            _deliveryVoucherQueryable =
                _deliveryVoucherRepository.GetMany(x => x.IsDeleted == false && x.WarehouseId == CurrentUser.Warehouse);
            _deliveryVoucherAllWarehouseQueryable = _deliveryVoucherRepository.GetMany(x => x.IsDeleted == false);
            _deliveryRequestVoucherQueryable =
                _unitOfWork.DeliveryRequestVoucher.GetMany(x =>
                    x.IsDeleted == false && x.WarehouseId == CurrentUser.Warehouse);
            _customerQueryable = _unitOfWork.Customer.GetMany(x => x.IsDeleted == false);
            _productQueryable = _unitOfWork.Product.GetMany(x => x.IsDeleted == false && x.IsActive == true);
        }

        public async Task<IActionResult> CreateDeliveryVoucher(CreateDeliveryVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
            if (!isCustomerExisted) return ApiResponse.NotFound(MessageConstant.CustomerNotFound);

            var deliveryVoucher = new DeliveryVoucher()
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
                    return ApiResponse.BadRequest(MessageConstant.DeliveryVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is { Count: > 0 })
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));

                if (model.RequestId != null)
                {
                    var request = _deliveryRequestVoucherQueryable.Include(x=>x.Details).FirstOrDefault(x => x.Id == model.RequestId);
                    if (request == null)
                    {
                        return ApiResponse.NotFound(MessageConstant.DeliveryRequestVoucherNotFound);
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

                    deliveryVoucher.RequestId = model.RequestId;
                }

                deliveryVoucher.Details = model.Details.Select(x => new DeliveryVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = deliveryVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _deliveryVoucherRepository.Add(deliveryVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateDeliveryVoucher(UpdateDeliveryVoucherModel model)
        {
            var deliveryVoucher = _deliveryVoucherQueryable
                .Include(x => x.Details)
                .Include(x => x.Request).ThenInclude(x => x.Details)
                .FirstOrDefault(x => x.Id == model.Id);
            if (deliveryVoucher == null)
                return ApiResponse.NotFound(MessageConstant.DeliveryVoucherNotFound);

            if (deliveryVoucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryVoucher);

            if (model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.NotFound(MessageConstant.CustomerNotFound);
            }

            deliveryVoucher.ReportingDate = model.ReportingDate ?? deliveryVoucher.ReportingDate;
            deliveryVoucher.Description = model.Description ?? deliveryVoucher.Description;
            deliveryVoucher.CustomerId = model.CustomerId ?? deliveryVoucher.CustomerId;
            deliveryVoucher.Status = model.Status ?? deliveryVoucher.Status;

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.DeliveryVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is { Count: > 0 })
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));

                if (deliveryVoucher.Request != null)
                {
                    if (CurrentUser.Warehouse != deliveryVoucher.Request.WarehouseId)
                    {
                        return ApiResponse.NotFound(MessageConstant.RequiredWarehouseRequestVoucher);
                    }

                    var modelProductIds = model.Details.Select(x => x.ProductId).ToList();
                    var requestProductIds = deliveryVoucher.Request.Details.Select(x => x.ProductId).ToList();
                    var conflictProductIds = modelProductIds.Except(requestProductIds).ToList();
                    if (conflictProductIds is { Count: > 0 })
                    {
                        return ApiResponse.BadRequest(
                            MessageConstant.ProductsNotFoundInRequestDetails.WithValues(string.Join(", ",
                                conflictProductIds)));
                    }
                }

                deliveryVoucher.Details.Clear();
                deliveryVoucher.Details = model.Details.Select(x => new DeliveryVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = deliveryVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _deliveryVoucherRepository.Update(deliveryVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulDeliveryVoucher(Guid id)
        {
            var deliveryVoucher = _deliveryVoucherQueryable.Include(x => x.Details)
                .FirstOrDefault(x => x.Id == id);
            if (deliveryVoucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryVoucherNotFound);

            deliveryVoucher.IsDeleted = true;
            _deliveryVoucherRepository.Update(deliveryVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Lock(Guid id)
        {
            var voucher = _deliveryVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryVoucherNotFound);

            voucher.Locked = true;

            _deliveryVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Unlock(Guid id)
        {
            var voucher = _deliveryVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryVoucherNotFound);

            voucher.Locked = false;

            _deliveryVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetDeliveryVoucher(Guid id)
        {
            var deliveryVoucher = _deliveryVoucherQueryable.Where(x => x.Id == id).Select(x =>
                new DeliveryVoucherViewModel()
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
                        : new FetchDeliveryRequestVoucherViewModel
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
                        : x.Details.Select(y => new DeliveryVoucherDetailViewModel()
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

            if (deliveryVoucher == null) return ApiResponse.NotFound(MessageConstant.DeliveryVoucherNotFound);

            return ApiResponse.Ok(deliveryVoucher);
        }

        public IActionResult FetchDeliveryVoucher(FetchModel model)
        {
            var vouchers = _deliveryVoucherQueryable.AsNoTracking()
                .Where(x => string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size)
                .Select(x =>
                    new FetchDeliveryVoucherViewModel()
                    {
                        Id = x.Id,
                        Code = x.Code
                    }).ToList();

            return ApiResponse.Ok(vouchers);
        }

        public IActionResult SearchDeliveryVoucherInWarehouse(SearchDeliveryVoucherInWarehouseModel model)
        {
            var query = _deliveryVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchDeliveryVoucherViewModel()
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
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchDeliveryVoucherByWarehouse(SearchDeliveryVoucherByWarehouseModel model)
        {
            var query = _deliveryVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (string.IsNullOrWhiteSpace(model.Customer) || x.Customer.Name.Contains(model.Customer)) &&
                (x.WarehouseId == model.WarehouseId)
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

            var data = query.Select(x => new SearchDeliveryVoucherViewModel()
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
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchDeliveryVoucherAllWarehouse(SearchDeliveryVoucherAllWarehouseModel model)
        {
            var query = _deliveryVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchDeliveryVoucherViewModel()
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
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }
    }
}