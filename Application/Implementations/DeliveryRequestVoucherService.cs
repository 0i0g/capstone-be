using System;
using System.Collections.Generic;
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
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class DeliveryRequestVoucherService : BaseService, IDeliveryRequestVoucherService
    {
        private readonly IDeliveryRequestVoucherRepository _deliveryRequestVoucherRepository;
        private readonly IDeliveryRequestVoucherDetailRepository _deliveryRequestVoucherDetailRepository;

        private readonly IQueryable<DeliveryRequestVoucher> _deliveryRequestVoucherQueryable;
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
            _deliveryRequestVoucherDetailQueryable = _deliveryRequestVoucherDetailRepository.GetAll();
            _customerQueryable = _unitOfWork.Customer.GetMany(x => x.IsDeleted == false);
            _productQueryable = _unitOfWork.Product.GetMany(x => x.IsDeleted == false);
        }

        public async Task<IActionResult> CreateDeliveryRequestVoucher(CreateDeliveryRequestVoucherModel model)
        {
            var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
            if (!isCustomerExisted) return ApiResponse.BadRequest(MessageConstant.CustomerNotFound);

            if (model.Details != null)
            {
                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is {Count: > 0})
                    return ApiResponse.BadRequest(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));
            }

            var id = new Guid();

            var voucher = new DeliveryRequestVoucher()
            {
                Id = id,
                VoucherDate = model.VoucherDate,
                Note = model.Note,
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
            var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            if (voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);

            if (model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.BadRequest(MessageConstant.CustomerNotFound);
            }

            voucher.VoucherDate = model.VoucherDate ?? voucher.VoucherDate;
            voucher.Note = model.Note ?? voucher.Note;
            voucher.CustomerId = model.CustomerId ?? voucher.CustomerId;

            _deliveryRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public Task<IActionResult> RemoveDeliveryRequestVoucher(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> AddDeliveryRequestVoucherDetail(CreateDeliveryRequestVoucherDetailModel model)
        {
            var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.VoucherId);
            if (voucher == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            if (voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);

            var product = _productQueryable.FirstOrDefault(x => x.Id == model.ProductId);
            if (product == null) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);

            var isDuplicate = _deliveryRequestVoucherDetailQueryable.Any(x =>
                x.ProductId == model.ProductId && x.VoucherId == model.VoucherId);
            if (isDuplicate)
                return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);

            var detail = new DeliveryRequestVoucherDetail()
            {
                Quantity = model.Quantity,
                ProductId = model.ProductId,
                VoucherId = model.VoucherId,
                ProductName = product.Name
            };

            _deliveryRequestVoucherDetailRepository.Add(detail);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateDeliveryRequestVoucherDetail(
            UpdateDeliveryRequestVoucherDetailModel model)
        {
            var detail = _deliveryRequestVoucherDetailQueryable.Include(x => x.Voucher)
                .FirstOrDefault(x => x.Id == model.Id);
            if (detail == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherDetailNotFound);

            if (detail.Voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);

            if (model.ProductId != null)
            {
                var isProductExited = _productQueryable.Any(x => x.Id == model.ProductId);
                if (!isProductExited) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);

                var isDuplicate = _deliveryRequestVoucherDetailQueryable.Any(x =>
                    x.ProductId == model.ProductId && x.VoucherId == detail.VoucherId);
                if (isDuplicate)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductDeliveryRequestVoucherDetail);
            }

            detail.Quantity = model.Quantity ?? detail.Quantity;
            detail.ProductId = model.ProductId ?? detail.ProductId;
            detail.ProductName = model.ProductId != null
                ? _productQueryable.FirstOrDefault(x => x.Id == model.ProductId)?.Name
                : detail.ProductName;

            _deliveryRequestVoucherDetailRepository.Update(detail);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> DeleteDeliveryRequestVoucherDetail(Guid id)
        {
            var detail = _deliveryRequestVoucherDetailQueryable.Include(x => x.Voucher)
                .FirstOrDefault(x => x.Id == id);
            if (detail == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherDetailNotFound);

            if (detail.Voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);

            _deliveryRequestVoucherDetailRepository.Remove(detail);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

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

        public async Task<IActionResult> UpdateDeliveryRequestVoucherStatus(
            UpdateDeliveryRequestVoucherStatusModel model)
        {
            var voucher = _deliveryRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            if (voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateDeliveryRequestVoucher);

            voucher.Status = model.Status;

            _deliveryRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetDeliveryRequestVoucher(Guid id)
        {
            var voucher = _deliveryRequestVoucherQueryable.Where(x => x.Id == id).Select(x =>
                new DeliveryRequestVoucherViewModel()
                {
                    Id = x.Id,
                    Code = x.Code,
                    VoucherDate = x.VoucherDate,
                    Note = x.Note,
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
                        }).ToList()
                }).FirstOrDefault();

            if (voucher == null) return ApiResponse.BadRequest(MessageConstant.DeliveryRequestVoucherNotFound);

            return ApiResponse.Ok(voucher);
        }

        public IActionResult SearchDeliveryRequestVoucher(SearchDeliveryRequestVoucherModel model)
        {
            var query = _deliveryRequestVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.VoucherDateFrom == null || x.VoucherDate >= model.VoucherDateFrom) &&
                (model.VoucherDateTo == null || x.VoucherDate <= model.VoucherDateTo) &&
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
                        ? query.OrderBy(x => x.Inc).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Inc).ThenByDescending(x => x.CreatedAt);
                    break;
                case "VOUCHERDATE":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.VoucherDate).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.VoucherDate).ThenByDescending(x => x.CreatedAt);
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
                        MessageConstant.OrderByInvalid.WithValues("Code, CreatedAt, VoucherDate, Status"));
            }

            var data = query.Select(x => new SearchReceiveRequestVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                VoucherDate = x.VoucherDate,
                Note = x.Note,
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
    }
}