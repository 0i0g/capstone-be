using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.TransferRequestVoucher;
using Application.ViewModels.TransferVoucher;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class TransferVoucherService : BaseService, ITransferVoucherService
    {
        private readonly ITransferVoucherRepository _transferVoucherRepository;
        private readonly IQueryable<TransferVoucher> _transferVoucherQueryable;

        private readonly IQueryable<TransferRequestVoucher> _transferRequestVoucherQueryable;
        private readonly IQueryable<Product> _productQueryable;
        private readonly IQueryable<Warehouse> _warehouseQueryable;
        private readonly IQueryable<User> _userQueryable;

        public TransferVoucherService(IServiceProvider provider) : base(provider)
        {
            _transferVoucherRepository = _unitOfWork.TransferVoucher;
            _transferVoucherQueryable =
                _transferVoucherRepository.GetMany(x => x.IsDeleted != true).Include(x => x.Details);

            _transferRequestVoucherQueryable =
                _unitOfWork.TransferRequestVoucher
                    .GetMany(x => x.IsDeleted != true && x.Status == EnumStatusRequest.Confirmed)
                    .Include(x => x.Details);
            _productQueryable =
                _unitOfWork.Product.GetMany(x => x.IsDeleted != true);
            _warehouseQueryable = _unitOfWork.Warehouse.GetMany(x => x.IsDeleted != true);
            _userQueryable = _unitOfWork.User.GetMany(x => x.IsDeleted != true && x.IsActive == true);
        }

        public async Task<IActionResult> CreateTransferVoucher(CreateTransferVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            if (_warehouseQueryable.FirstOrDefault(x => x.Id == model.InboundWarehouseId) == null)
            {
                return ApiResponse.NotFound(MessageConstant.WarehouseNotFound);
            }

            if (model.DeliveryManId != null)
            {
                if (_userQueryable.FirstOrDefault(x => x.Id == model.DeliveryManId) == null)
                {
                    return ApiResponse.NotFound(MessageConstant.UserNotFound);
                }
            }

            if (model.RecipientId != null)
            {
                if (_userQueryable.FirstOrDefault(x => x.Id == model.RecipientId) == null)
                {
                    return ApiResponse.NotFound(MessageConstant.UserNotFound);
                }
            }

            var transferVoucher = new TransferVoucher()
            {
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                Status = EnumStatusVoucher.Pending,
                Locked = false,
                InboundWarehouseId = model.InboundWarehouseId,
                OutboundWarehouseId = CurrentUser.Warehouse!.Value,
                DeliveryManId = model.DeliveryManId,
                RecipientId = model.RecipientId
            };

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.TransferVoucherDetailEmpty);

                var productsInModel = model.Details.Select(x => x.ProductId).ToList();
                var productIdsSet = new HashSet<Guid>(productsInModel);
                if (productIdsSet.Count != productsInModel.Count)
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductTransferVoucherDetail);

                var productsExist = _productQueryable.Where(x => productsInModel.Contains(x.Id)).Select(x => x.Id);
                var productsNotExist = productsInModel.Except(productsExist).ToList();

                if (productsNotExist is { Count: > 0 })
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsNotFound.WithValues(string.Join(", ", productsNotExist)));

                if (model.RequestId != null)
                {
                    var request = _transferRequestVoucherQueryable.Include(x => x.Details)
                        .FirstOrDefault(x => x.Id == model.RequestId);
                    if (request == null)
                    {
                        return ApiResponse.NotFound(MessageConstant.TransferRequestVoucherNotFound);
                    }

                    if (CurrentUser.Warehouse != request.OutboundWarehouseId)
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

                    transferVoucher.RequestId = model.RequestId;
                }

                transferVoucher.Details = model.Details.Select(x => new TransferVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = transferVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _transferVoucherRepository.Add(transferVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateTransferVoucher(UpdateTransferVoucherModel model)
        {
            var transferVoucher = _transferVoucherQueryable
                .Include(x => x.Details)
                .Include(x => x.Request).ThenInclude(x => x.Details)
                .FirstOrDefault(x =>
                    x.Id == model.Id && CurrentUser.Warehouse == x.OutboundWarehouseId);
            if (transferVoucher == null)
                return ApiResponse.NotFound(MessageConstant.TransferVoucherNotFound);

            if (transferVoucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateTransferVoucher);

            if (model.InboundWarehouseId != null)
            {
                if (_warehouseQueryable.FirstOrDefault(x => x.Id == model.InboundWarehouseId) == null)
                {
                    return ApiResponse.NotFound(MessageConstant.WarehouseNotFound);
                }
            }

            if (model.DeliveryManId != null)
            {
                if (_userQueryable.FirstOrDefault(x => x.Id == model.DeliveryManId) == null)
                {
                    return ApiResponse.NotFound(MessageConstant.UserNotFound);
                }
            }

            if (model.RecipientId != null)
            {
                if (_userQueryable.FirstOrDefault(x => x.Id == model.RecipientId) == null)
                {
                    return ApiResponse.NotFound(MessageConstant.UserNotFound);
                }
            }

            transferVoucher.ReportingDate = model.ReportingDate ?? transferVoucher.ReportingDate;
            transferVoucher.Description = model.Description ?? transferVoucher.Description;
            transferVoucher.Status = model.Status ?? transferVoucher.Status;
            transferVoucher.InboundWarehouseId =
                model.InboundWarehouseId ?? transferVoucher.InboundWarehouseId;
            transferVoucher.DeliveryManId = model.DeliveryManId ?? transferVoucher.DeliveryManId;
            transferVoucher.RecipientId = model.RecipientId ?? transferVoucher.RecipientId;

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.TransferVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductTransferVoucherDetail);
                }

                var products = _productQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                if (transferVoucher.Request != null)
                {
                    if (CurrentUser.Warehouse != transferVoucher.Request.OutboundWarehouseId)
                    {
                        return ApiResponse.NotFound(MessageConstant.RequiredWarehouseRequestVoucher);
                    }

                    var modelProductIds = model.Details.Select(x => x.ProductId).ToList();
                    var requestProductIds = transferVoucher.Request.Details.Select(x => x.ProductId).ToList();
                    var conflictProductIds = modelProductIds.Except(requestProductIds).ToList();
                    if (conflictProductIds is { Count: > 0 })
                    {
                        return ApiResponse.BadRequest(
                            MessageConstant.ProductsNotFoundInRequestDetails.WithValues(string.Join(", ",
                                conflictProductIds)));
                    }
                }

                transferVoucher.Details.Clear();
                transferVoucher.Details = model.Details.Select(x => new TransferVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = transferVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _transferVoucherRepository.Update(transferVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> ReceiveTransferVoucher(ReceiveTransferVoucherModel model)
        {
            var transferVoucher = _transferVoucherQueryable
                .Include(x => x.Details)
                .Include(x => x.Request).ThenInclude(x => x.Details)
                .FirstOrDefault(x =>
                    x.Id == model.Id && x.Status == EnumStatusVoucher.Delivered);

            if (transferVoucher == null)
                return ApiResponse.NotFound(MessageConstant.TransferVoucherNotFound);

            if (CurrentUser.Warehouse != transferVoucher.InboundWarehouseId)
            {
                return ApiResponse.NotFound(MessageConstant.RequiredWarehouseVoucher);
            }

            if (transferVoucher.Locked == true)
                return ApiResponse.BadRequest(MessageConstant.ForbiddenToUpdateTransferVoucher);

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.TransferVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateProductTransferVoucherDetail);
                }

                var products = _productQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                var modelProductIds = model.Details.Select(x => x.ProductId).ToList();
                var voucherProductIds = transferVoucher.Details.Select(x => x.ProductId).ToList();
                var conflictProductIds = modelProductIds.Except(voucherProductIds).ToList();
                if (conflictProductIds is { Count: > 0 })
                {
                    return ApiResponse.BadRequest(
                        MessageConstant.ProductsNotFoundInVoucherDetails.WithValues(string.Join(", ",
                            conflictProductIds)));
                }

                foreach (var modelDetail in model.Details)
                {
                    var detail = transferVoucher.Details.FirstOrDefault(x => x.ProductId == modelDetail.ProductId);
                    if (detail != null)
                    {
                        detail.RealQuantity = modelDetail.RealQuantity;
                    }
                }
            }

            _transferVoucherRepository.Update(transferVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulTransferVoucher(Guid id)
        {
            var transferVoucher =
                _transferVoucherQueryable.Include(x => x.Details).FirstOrDefault(x =>
                    x.Id == id && CurrentUser.Warehouse == x.OutboundWarehouseId);
            if (transferVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.TransferVoucherNotFound);
            }

            transferVoucher.IsDeleted = true;
            _transferVoucherRepository.Update(transferVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Lock(Guid id)
        {
            var transferVoucher =
                _transferVoucherQueryable.FirstOrDefault(x =>
                    x.Id == id && CurrentUser.Warehouse == x.OutboundWarehouseId);
            if (transferVoucher == null)
                return ApiResponse.BadRequest(MessageConstant.TransferVoucherNotFound);

            transferVoucher.Locked = true;

            _transferVoucherRepository.Update(transferVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Unlock(Guid id)
        {
            var transferVoucher =
                _transferVoucherQueryable.FirstOrDefault(x =>
                    x.Id == id && CurrentUser.Warehouse == x.OutboundWarehouseId);
            if (transferVoucher == null)
                return ApiResponse.BadRequest(MessageConstant.TransferVoucherNotFound);

            transferVoucher.Locked = false;

            _transferVoucherRepository.Update(transferVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetTransferVoucher(Guid id)
        {
            var transferVoucher = _transferVoucherQueryable
                .Where(x => x.Id == id && CurrentUser.Warehouse == x.OutboundWarehouseId)
                .Select(x => new TransferVoucherViewModel
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
                        : new FetchTransferRequestVoucherViewModel
                        {
                            Id = x.Request.Id,
                            Code = x.Request.Code
                        },
                    InboundWarehouse = x.InboundWarehouse == null
                        ? null
                        : new FetchWarehouseViewModel
                        {
                            Id = x.InboundWarehouseId,
                            Name = x.InboundWarehouse.Name
                        },
                    OutboundWarehouse = x.OutboundWarehouse == null
                        ? null
                        : new FetchWarehouseViewModel
                        {
                            Id = x.OutboundWarehouseId,
                            Name = x.OutboundWarehouse.Name
                        },
                    DeliveryMan = x.DeliveryMan == null
                        ? null
                        : new FetchUserViewModel
                        {
                            Id = x.DeliveryMan.Id,
                            Name = x.DeliveryMan.FullName,
                            Avatar = x.DeliveryMan.Avatar
                        },
                    Recipient = x.Recipient == null
                        ? null
                        : new FetchUserViewModel
                        {
                            Id = x.Recipient.Id,
                            Name = x.Recipient.FullName,
                            Avatar = x.Recipient.Avatar
                        },
                    Details = x.Details == null
                        ? null
                        : x.Details.Select(y => new TransferVoucherDetailViewModel
                        {
                            Id = y.Id,
                            RequestQuantity = x.Request == null
                                ? null
                                : x.Request.Details.FirstOrDefault(z => z.ProductId == y.ProductId).Quantity,
                            VoucherQuantity = y.Quantity,
                            RealQuantity = y.RealQuantity,
                            ProductName = y.ProductName,
                            Product = new FetchProductViewModel()
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name
                            },
                        }).OrderBy(y => y.ProductName).ToList(),
                }).FirstOrDefault();

            if (transferVoucher == null)
                return ApiResponse.NotFound(MessageConstant.TransferVoucherNotFound);

            return ApiResponse.Ok(transferVoucher);
        }

        public IActionResult FetchTransferVoucherOutbound(FetchModel model)
        {
            var transferVouchers = _transferVoucherQueryable.AsNoTracking().Where(x =>
                    (string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword)) &&
                    x.OutboundWarehouseId == CurrentUser.Warehouse)
                .Take(model.Size).Select(x => new FetchTransferVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(transferVouchers);
        }

        public IActionResult FetchTransferVoucherInbound(FetchModel model)
        {
            var transferVouchers = _transferVoucherQueryable.AsNoTracking().Where(x =>
                    (string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword)) &&
                    x.InboundWarehouseId == CurrentUser.Warehouse)
                .Take(model.Size).Select(x => new FetchTransferVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(transferVouchers);
        }

        public IActionResult SearchTransferVoucherInInboundWarehouse(SearchTransferVoucherInWarehouseModel model)
        {
            var query = _transferVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (x.InboundWarehouseId == CurrentUser.Warehouse)
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

            var data = query.Select(x => new SearchTransferVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Request = x.Request == null
                    ? null
                    : new FetchTransferRequestVoucherViewModel
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    },
                InboundWarehouse = x.InboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.InboundWarehouse.Id,
                        Name = x.InboundWarehouse.Name
                    },
                OutboundWarehouse = x.OutboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.OutboundWarehouse.Id,
                        Name = x.OutboundWarehouse.Name
                    },
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchTransferVoucherInOutboundWarehouse(SearchTransferVoucherInWarehouseModel model)
        {
            var query = _transferVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (x.OutboundWarehouseId == CurrentUser.Warehouse)
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

            var data = query.Select(x => new SearchTransferVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Request = x.Request == null
                    ? null
                    : new FetchTransferRequestVoucherViewModel
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    },
                InboundWarehouse = x.InboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.InboundWarehouse.Id,
                        Name = x.InboundWarehouse.Name
                    },
                OutboundWarehouse = x.OutboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.OutboundWarehouse.Id,
                        Name = x.OutboundWarehouse.Name
                    },
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchTransferVoucherByInboundWarehouse(SearchTransferVoucherByWarehouseModel model)
        {
            var query = _transferVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (x.InboundWarehouseId == model.WarehouseId)
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

            var data = query.Select(x => new SearchTransferVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Request = x.Request == null
                    ? null
                    : new FetchTransferRequestVoucherViewModel
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    },
                InboundWarehouse = x.InboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.InboundWarehouse.Id,
                        Name = x.InboundWarehouse.Name
                    },
                OutboundWarehouse = x.OutboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.OutboundWarehouse.Id,
                        Name = x.OutboundWarehouse.Name
                    },
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchTransferVoucherByOutboundWarehouse(SearchTransferVoucherByWarehouseModel model)
        {
            var query = _transferVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status) &&
                (x.OutboundWarehouseId == model.WarehouseId)
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

            var data = query.Select(x => new SearchTransferVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Request = x.Request == null
                    ? null
                    : new FetchTransferRequestVoucherViewModel
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    },
                InboundWarehouse = x.InboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.InboundWarehouse.Id,
                        Name = x.InboundWarehouse.Name
                    },
                OutboundWarehouse = x.OutboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.OutboundWarehouse.Id,
                        Name = x.OutboundWarehouse.Name
                    },
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchTransferVoucherAllWarehouse(SearchTransferVoucherAllWarehouseModel model)
        {
            var query = _transferVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                (model.Status == null || x.Status == model.Status)
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

            var data = query.Select(x => new SearchTransferVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
                Request = x.Request == null
                    ? null
                    : new FetchTransferRequestVoucherViewModel
                    {
                        Id = x.Request.Id,
                        Code = x.Request.Code
                    },
                InboundWarehouse = x.InboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.InboundWarehouse.Id,
                        Name = x.InboundWarehouse.Name
                    },
                OutboundWarehouse = x.OutboundWarehouse == null
                    ? null
                    : new FetchWarehouseViewModel()
                    {
                        Id = x.OutboundWarehouse.Id,
                        Name = x.OutboundWarehouse.Name
                    },
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }
    }
}