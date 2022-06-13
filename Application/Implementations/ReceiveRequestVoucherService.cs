using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class ReceiveRequestVoucherService : BaseService, IReceiveRequestVoucherService
    {
        private readonly IReceiveRequestVoucherRepository _receiveRequestVoucherRepository;
        private readonly IReceiveRequestVoucherDetailRepository _receiveRequestVoucherDetailRepository;

        private readonly IQueryable<ReceiveRequestVoucher> _receiveRequestVoucherQueryable;
        private readonly IQueryable<Customer> _customerQueryable;
        private readonly IQueryable<Product> _productQueryable;
        private readonly IQueryable<ReceiveRequestVoucherDetail> _receiveRequestVoucherDetailQueryable;

        public ReceiveRequestVoucherService(IServiceProvider provider) : base(provider)
        {
            _receiveRequestVoucherRepository = _unitOfWork.ReceiveRequestVoucher;
            _receiveRequestVoucherDetailRepository = _unitOfWork.ReceiveRequestVoucherDetail;

            _receiveRequestVoucherQueryable =
                _receiveRequestVoucherRepository.GetMany(x =>
                    x.WarehouseId == CurrentUser.Warehouse && x.IsDeleted == false);
            _customerQueryable = _unitOfWork.Customer.GetMany(x => x.IsDeleted == false);
            _productQueryable = _unitOfWork.Product.GetMany(x => x.IsDeleted == false);
            _receiveRequestVoucherDetailQueryable = _receiveRequestVoucherDetailRepository.GetAll();
        }

        public async Task<IActionResult> CreateReceiveRequestVoucher(CreateReceiveRequestVoucherModel model)
        {
            if (model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.BadRequest(MessageConstant.CustomerNotFound);
            }

            if (model.Details != null)
            {
                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductReceiveRequestVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is {Count: > 0})
                    return ApiResponse.BadRequest(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));
            }

            var requestId = new Guid();

            var receiveRequestVoucher = new ReceiveRequestVoucher()
            {
                Id = requestId,
                VoucherDate = model.VoucherDate,
                Note = model.Note,
                Status = EnumStatusRequest.Pending,
                Locked = false,
                CustomerId = model.CustomerId,
                WarehouseId = (Guid) CurrentUser.Warehouse!,
                Details = model.Details?.Select(x => new ReceiveRequestVoucherDetail()
                {
                    Quantity = x.Quantity,
                    VoucherId = requestId,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList()
            };

            _receiveRequestVoucherRepository.Add(receiveRequestVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateReceiveRequestVoucher(UpdateReceiveRequestVoucherModel model)
        {
            var receiveRequestVoucher = _receiveRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (receiveRequestVoucher == null)
                return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherNotFound);

            if (receiveRequestVoucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateReceiveRequestVoucher);

            if (model.IsCustomerIdNull == false && model.CustomerId != null)
            {
                var isCustomerExisted = _customerQueryable.Any(x => x.Id == model.CustomerId);
                if (!isCustomerExisted) return ApiResponse.BadRequest(MessageConstant.CustomerNotFound);
            }

            receiveRequestVoucher.VoucherDate = model.VoucherDate ?? receiveRequestVoucher.VoucherDate;
            receiveRequestVoucher.Note = model.Note ?? receiveRequestVoucher.Note;
            receiveRequestVoucher.CustomerId =
                model.IsCustomerIdNull ? null : model.CustomerId ?? receiveRequestVoucher.CustomerId;

            _receiveRequestVoucherRepository.Update(receiveRequestVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public Task<IActionResult> RemoveReceiveRequestVoucher(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> AddReceiveRequestVoucherDetail(CreateReceiveRequestVoucherDetailModel model)
        {
            var voucher = _receiveRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.VoucherId);
            if (voucher == null) return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherNotFound);

            if (voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateReceiveRequestVoucher);

            var product = _productQueryable.FirstOrDefault(x => x.Id == model.ProductId);
            if (product == null) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);

            var isDuplicate = _receiveRequestVoucherDetailQueryable.Any(x =>
                x.ProductId == model.ProductId && x.VoucherId == model.VoucherId);
            if (isDuplicate) return ApiResponse.BadRequest(MessageConstant.DuplicateProductReceiveRequestVoucherDetail);

            var detail = new ReceiveRequestVoucherDetail()
            {
                Quantity = model.Quantity,
                VoucherId = model.VoucherId,
                ProductId = model.ProductId,
                ProductName = product.Name
            };

            _receiveRequestVoucherDetailRepository.Add(detail);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateReceiveRequestVoucherDetail(UpdateReceiveRequestVoucherDetailModel model)
        {
            var detail = _receiveRequestVoucherDetailQueryable.Include(x => x.Voucher)
                .FirstOrDefault(x => x.Id == model.Id);
            if (detail == null) return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherDetailNotFound);

            if (detail.Voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateReceiveRequestVoucher);

            if (model.ProductId != null)
            {
                var isProductExited = _productQueryable.Any(x => x.Id == model.ProductId);
                if (!isProductExited) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);

                var isDuplicate = _receiveRequestVoucherDetailQueryable.Any(x =>
                    x.ProductId == model.ProductId && x.VoucherId == detail.VoucherId);
                if (isDuplicate)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductReceiveRequestVoucherDetail);
            }

            detail.Quantity = model.Quantity ?? detail.Quantity;
            detail.ProductId = model.ProductId ?? detail.ProductId;
            detail.ProductName = model.ProductId != null
                ? _productQueryable.FirstOrDefault(x => x.Id == model.ProductId)?.Name
                : detail.ProductName;

            _receiveRequestVoucherDetailRepository.Update(detail);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> DeleteReceiveRequestVoucherDetail(Guid id)
        {
            var detail = _receiveRequestVoucherDetailQueryable.Include(x => x.Voucher).FirstOrDefault(x => x.Id == id);
            if (detail == null) return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherDetailNotFound);

            if (detail.Voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateReceiveRequestVoucher);

            _receiveRequestVoucherDetailRepository.Remove(detail);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Lock(Guid id)
        {
            var voucher = _receiveRequestVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherNotFound);

            voucher.Locked = true;

            _receiveRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Unlock(Guid id)
        {
            var voucher = _receiveRequestVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherNotFound);

            voucher.Locked = false;

            _receiveRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateReceiveRequestVoucherStatus(UpdateReceiveRequestVoucherStatusModel model)
        {
            var voucher = _receiveRequestVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (voucher == null)
                return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherNotFound);

            if (voucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateReceiveRequestVoucher);

            voucher.Status = model.Status;

            _receiveRequestVoucherRepository.Update(voucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetReceiveRequestVoucher(Guid id)
        {
            var voucher = _receiveRequestVoucherQueryable.Where(x => x.Id == id).Select(x =>
                new ReceiveRequestVoucherViewModel()
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
                        : x.Details.Select(y => new ReceiveRequestVoucherDetailViewModel()
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
            if (voucher == null) return ApiResponse.BadRequest(MessageConstant.ReceiveRequestVoucherNotFound);

            return ApiResponse.Ok(voucher);
        }

        public IActionResult SearchReceiveRequestVoucher(SearchReceiveRequestVoucherModel model)
        {
            var query = _receiveRequestVoucherQueryable.AsNoTracking().Where(x =>
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

        public IActionResult FetchReceiveRequestVoucher(FetchModel model)
        {
            var vouchers = _receiveRequestVoucherQueryable.AsNoTracking()
                .Where(x => string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size)
                .Select(x =>
                    new FetchReceiveRequestVoucherViewModel()
                    {
                        Id = x.Id,
                        Code = x.Code
                    }).ToList();

            return ApiResponse.Ok(vouchers);
        }
    }
}