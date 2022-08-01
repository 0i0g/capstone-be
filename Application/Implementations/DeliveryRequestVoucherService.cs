using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.DeliveryRequestVoucher;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class DeliveryRequestVoucherService : BaseService, IDeliveryRequestVoucherService
    {
        private readonly IDeliveryRequestVoucherRepository _deliveryRequestVoucherRepository;
        private readonly IDeliveryRequestVoucherDetailRepository _deliveryRequestVoucherDetailRepository;

        private readonly IQueryable<DeliveryRequestVoucher> _deliveryRequestVoucherQueryable;
        private readonly IQueryable<DeliveryRequestVoucher> _deliveryRequestVoucherAllQueryable;
        private readonly IQueryable<DeliveryRequestVoucherDetail> _deliveryRequestVoucherDetailQueryable;
        private readonly IQueryable<Customer> _customerQueryable;
        private readonly IQueryable<Product> _productQueryable;

        public DeliveryRequestVoucherService(IServiceProvider provider) : base(provider)
        {
            _deliveryRequestVoucherRepository = _unitOfWork.DeliveryRequestVoucher;
            _deliveryRequestVoucherDetailRepository = _unitOfWork.DeliveryRequestVoucherDetail;

            _deliveryRequestVoucherQueryable =
                _deliveryRequestVoucherRepository.GetMany(x =>
                    x.WarehouseId == CurrentUser.Warehouse && x.IsDeleted == false);
            _deliveryRequestVoucherAllQueryable = _deliveryRequestVoucherRepository.GetMany(x => x.IsDeleted == false);
            _deliveryRequestVoucherDetailQueryable = _deliveryRequestVoucherDetailRepository.GetAll();
            _customerQueryable = _unitOfWork.Customer.GetMany(x => x.IsDeleted == false);
            _productQueryable = _unitOfWork.Product.GetMany(x => x.IsDeleted == false && x.IsActive == true);
        }

        public async Task<IActionResult> CreateDeliveryRequestVoucher(CreateDeliveryRequestVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
            if (!isCustomerExisted) return ApiResponse.NotFound(MessageConstant.CustomerNotFound);

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is {Count: > 0})
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));
            }

            var id = new Guid();

            var voucher = new DeliveryRequestVoucher()
            {
                Id = id,
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                Status = EnumStatusRequest.Pending,
                Locked = false,
                CustomerId = model.CustomerId,
                WarehouseId = (Guid) CurrentUser.Warehouse!,
                Details = model.Details?.Select(x => new DeliveryRequestVoucherDetail()
                {
                    Quantity = x.Quantity,
                    VoucherId = id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList()
            };

            _deliveryRequestVoucherRepository.Add(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateDeliveryRequestVoucher(UpdateDeliveryRequestVoucherModel model)
        {
            var voucher = _deliveryRequestVoucherQueryable.Include(x => x.Details)
                .FirstOrDefault(x => x.Id == model.Id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            if (voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);

            if (model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.NotFound(MessageConstant.CustomerNotFound);
            }

            voucher.ReportingDate = model.ReportingDate ?? voucher.ReportingDate;
            voucher.Description = model.Description ?? voucher.Description;
            voucher.CustomerId = model.CustomerId ?? voucher.CustomerId;
            voucher.Status = model.Status ?? voucher.Status;

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is {Count: > 0})
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));

                voucher.Details.Clear();
                voucher.Details = model.Details.Select(x => new DeliveryRequestVoucherDetail()
                {
                    Quantity = x.Quantity,
                    VoucherId = voucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _deliveryRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulDeliveryRequestVoucher(Guid id)
        {
            var voucher = _deliveryRequestVoucherQueryable.Include(x => x.Details)
                .FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            voucher.IsDeleted = true;
            _deliveryRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        // public async Task<IActionResult> AddDeliveryRequestVoucherDetail(CreateDeliveryRequestVoucherDetailModel model)
        // {
        //     var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.VoucherId);
        //     if (voucher == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);
        //
        //     if (voucher.Locked == true)
        //         return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);
        //
        //     var product = _productQueryable.FirstOrDefault(x => x.Id == model.ProductId);
        //     if (product == null) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);
        //
        //     var isDuplicate = _deliveryRequestVoucherDetailQueryable.Any(x =>
        //         x.ProductId == model.ProductId && x.VoucherId == model.VoucherId);
        //     if (isDuplicate)
        //         return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);
        //
        //     var detail = new DeliveryRequestVoucherDetail()
        //     {
        //         Quantity = model.Quantity,
        //         ProductId = model.ProductId,
        //         VoucherId = model.VoucherId,
        //         ProductName = product.Name
        //     };
        //
        //     _deliveryRequestVoucherDetailRepository.Add(detail);
        //     await _unitOfWork.SaveChanges();
        //
        //     return ApiResponse.Ok();
        // }
        //
        // public async Task<IActionResult> UpdateDeliveryRequestVoucherDetail(
        //     UpdateDeliveryRequestVoucherDetailModel model)
        // {
        //     var detail = _deliveryRequestVoucherDetailQueryable.Include(x => x.Voucher)
        //         .FirstOrDefault(x => x.Id == model.Id);
        //     if (detail == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherDetailNotFound);
        //
        //     if (detail.Voucher.Locked == true)
        //         return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);
        //
        //     if (model.ProductId != null)
        //     {
        //         var isProductExited = _productQueryable.Any(x => x.Id == model.ProductId);
        //         if (!isProductExited) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);
        //
        //         var isDuplicate = _deliveryRequestVoucherDetailQueryable.Any(x =>
        //             x.ProductId == model.ProductId && x.VoucherId == detail.VoucherId);
        //         if (isDuplicate)
        //             return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);
        //     }
        //
        //     detail.Quantity = model.Quantity ?? detail.Quantity;
        //     detail.ProductId = model.ProductId ?? detail.ProductId;
        //     detail.ProductName = model.ProductId != null
        //         ? _productQueryable.FirstOrDefault(x => x.Id == model.ProductId)?.Name
        //         : detail.ProductName;
        //
        //     _deliveryRequestVoucherDetailRepository.Update(detail);
        //     await _unitOfWork.SaveChanges();
        //
        //     return ApiResponse.Ok();
        // }
        //
        // public async Task<IActionResult> DeleteDeliveryRequestVoucherDetail(Guid id)
        // {
        //     var detail = _deliveryRequestVoucherDetailQueryable.Include(x => x.Voucher)
        //         .FirstOrDefault(x => x.Id == id);
        //     if (detail == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherDetailNotFound);
        //
        //     if (detail.Voucher.Locked == true)
        //         return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);
        //
        //     _deliveryRequestVoucherDetailRepository.Remove(detail);
        //     await _unitOfWork.SaveChanges();
        //
        //     return ApiResponse.Ok();
        // }

        public async Task<IActionResult> Lock(Guid id)
        {
            var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            voucher.Locked = true;

            _deliveryRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Unlock(Guid id)
        {
            var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            voucher.Locked = false;

            _deliveryRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        // public async Task<IActionResult> UpdateDeliveryRequestVoucherStatus(
        //     UpdateDeliveryRequestVoucherStatusModel model)
        // {
        //     var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
        //     if (voucher == null)
        //         return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);
        //
        //     if (voucher.Locked == true)
        //         return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);
        //
        //     voucher.Status = model.Status;
        //
        //     _deliveryRequestVoucherRepository.Update(voucher);
        //     await _unitOfWork.SaveChanges();
        //
        //     return ApiResponse.Ok();
        // }

        public IActionResult GetDeliveryRequestVoucher(Guid id)
        {
            var voucher = _deliveryRequestVoucherQueryable.Where(x => x.Id == id).Select(x =>
                new DeliveryRequestVoucherViewModel()
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
                        : x.Details.Select(y => new DeliveryRequestVoucherDetailViewModel()
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
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

            if (voucher == null) return ApiResponse.NotFound(MessageConstant.DeliveryRequestVoucherNotFound);

            return ApiResponse.Ok(voucher);
        }

        public IActionResult SearchDeliveryRequestVoucherInWarehouse(SearchDeliveryRequestVoucherInWarehouseModel model)
        {
            var query = _deliveryRequestVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchDeliveryRequestVoucherViewModel()
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

        public IActionResult SearchDeliveryRequestVoucherByWarehouse(SearchDeliveryRequestVoucherByWarehouseModel model)
        {
            var query = _deliveryRequestVoucherAllQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchDeliveryRequestVoucherViewModel()
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

        public IActionResult SearchDeliveryRequestVoucherAllWarehouse(
            SearchDeliveryRequestVoucherAllWarehouseModel model)
        {
            var query = _deliveryRequestVoucherAllQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchDeliveryRequestVoucherViewModel()
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

        public IActionResult FetchDeliveryRequestVoucher(FetchModel model)
        {
            var vouchers = _deliveryRequestVoucherQueryable.AsNoTracking()
                .Where(x => string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size)
                .Select(x =>
                    new FetchDeliveryRequestVoucherViewModel()
                    {
                        Id = x.Id,
                        Code = x.Code
                    }).ToList();

            return ApiResponse.Ok(vouchers);
        }

        public IActionResult GetDeliveryRequestDetails(Guid id)
        {
            var details = _deliveryRequestVoucherQueryable.Where(x => x.Id == id).Include(x => x.Details)
                .Select(x => x.Details.Select(y => new DeliveryRequestVoucherDetailViewModel
                {
                    Quantity = y.Quantity,
                    Product = new FetchProductViewModel
                    {
                        Id = y.ProductId,
                        Name = y.Product.Name
                    }
                })).FirstOrDefault();

            if (details == null)
            {
                return ApiResponse.NotFound(MessageConstant.DeliveryRequestVoucherNotFound);
            }

            return ApiResponse.Ok(details);
        }
    }
}