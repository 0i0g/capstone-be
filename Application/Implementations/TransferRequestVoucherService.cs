using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.TransferRequestVoucher;
using Data.Entities;
using Data.Enums;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class TransferRequestVoucherService : BaseService, ITransferRequestVoucherService
    {
        private readonly ITransferRequestVoucherRepository _transferRequestVoucherRepository;
        private readonly IQueryable<TransferRequestVoucher> _transferRequestVoucherQueryable;
        private readonly IProductRepository _productRepository;
        private readonly IQueryable<Product> _productsQueryable;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IQueryable<Warehouse> _warehouseQueryable;
        private readonly IUserRepository _userRepository;
        private readonly IQueryable<User> _userQueryable;

        public TransferRequestVoucherService(IServiceProvider provider) : base(provider)
        {
            _transferRequestVoucherRepository = _unitOfWork.TransferRequestVoucher;
            _transferRequestVoucherQueryable =
                _transferRequestVoucherRepository.GetMany(x => x.IsDeleted != true).Include(x => x.Details);
            _productRepository = _unitOfWork.Product;
            _productsQueryable =
                _productRepository.GetMany(x => x.IsDeleted != true);
            _warehouseRepository = _unitOfWork.Warehouse;
            _warehouseQueryable = _warehouseRepository.GetMany(x => x.IsDeleted != true);
            _userRepository = _unitOfWork.User;
            _userQueryable = _userRepository.GetMany(x => x.IsDeleted != true);
        }

        public async Task<IActionResult> CreateTransferRequestVoucher(CreateTransferRequestVoucherModel model)
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

            var newTransferRequestVoucher = new TransferRequestVoucher
            {
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                Status = EnumStatusRequest.Pending,
                Locked = false,
                InboundWarehouseId = model.InboundWarehouseId,
                OutboundWarehouseId = CurrentUser.Warehouse!.Value,
                DeliveryManId = model.DeliveryManId,
                RecipientId = model.RecipientId
            };

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.TransferRequestVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateTransferRequestVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                newTransferRequestVoucher.Details = model.Details.Select(x => new TransferRequestVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = newTransferRequestVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _transferRequestVoucherRepository.Add(newTransferRequestVoucher);

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateTransferRequestVoucher(UpdateTransferRequestVoucherModel model)
        {
            var transferRequestVoucher =
                _transferRequestVoucherQueryable.Include(x => x.Details).FirstOrDefault(x =>
                    x.Id == model.Id && CurrentUser.Warehouse == x.OutboundWarehouseId);
            if (transferRequestVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.TransferRequestVoucherNotFound);
            }

            if (transferRequestVoucher.Locked == true)
            {
                return ApiResponse.NotFound(MessageConstant.ForbiddenToUpdateTransferRequestVoucher);
            }

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

            transferRequestVoucher.ReportingDate = model.ReportingDate ?? transferRequestVoucher.ReportingDate;
            transferRequestVoucher.Description = model.Description ?? transferRequestVoucher.Description;
            transferRequestVoucher.Status = model.Status ?? transferRequestVoucher.Status;
            transferRequestVoucher.InboundWarehouseId =
                model.InboundWarehouseId ?? transferRequestVoucher.InboundWarehouseId;
            transferRequestVoucher.DeliveryManId = model.DeliveryManId ?? transferRequestVoucher.DeliveryManId;
            transferRequestVoucher.RecipientId = model.RecipientId ?? transferRequestVoucher.RecipientId;

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.TransferRequestVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateTransferRequestVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                transferRequestVoucher.Details.Clear();
                transferRequestVoucher.Details = model.Details.Select(x => new TransferRequestVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = transferRequestVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _transferRequestVoucherRepository.Update(transferRequestVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        // public async Task<IActionResult> ReceiveTransferRequestVoucher(ReceiveTransferRequestVoucherModel model)
        // {
        //     var transferRequestVoucher =
        //         _transferRequestVoucherQueryable.Include(x => x.Details).FirstOrDefault(x =>
        //             x.Id == model.Id && CurrentUser.Warehouse == x.InboundWarehouseId);
        //     if (transferRequestVoucher == null)
        //     {
        //         return ApiResponse.NotFound(MessageConstant.TransferRequestVoucherNotFound);
        //     }
        //     
        //     if (model.Details != null)
        //     {
        //         if (model.Details.Count == 0)
        //             return ApiResponse.BadRequest(MessageConstant.TransferRequestVoucherDetailEmpty);
        //
        //         var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
        //         if (duplicateDetail.Count > 0)
        //         {
        //             return ApiResponse.BadRequest(MessageConstant.DuplicateTransferRequestVoucherDetailsProduct);
        //         }
        //
        //         var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
        //             .Select(x => x.Id).ToList();
        //         var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
        //         if (failProducts.Count > 0)
        //         {
        //             return ApiResponse.NotFound(
        //                 MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
        //         }
        //
        //         foreach (var modelDetail in model.Details)
        //         {
        //             var detail =
        //                 transferRequestVoucher.Details.FirstOrDefault(x => x.ProductId == modelDetail.ProductId);
        //             if (detail != null)
        //             {
        //                 detail.RealQuantity = modelDetail.RealQuantity;
        //             }
        //         }
        //     }
        //     
        //     _transferRequestVoucherRepository.Update(transferRequestVoucher);
        //     await _unitOfWork.SaveChanges();
        //
        //     return ApiResponse.Ok();
        // }

        public async Task<IActionResult> RemoveTransferRequestVoucher(Guid id)
        {
            var transferRequestVoucher =
                _transferRequestVoucherQueryable.Include(x => x.Details).FirstOrDefault(x => x.Id == id);
            if (transferRequestVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.TransferRequestVoucherNotFound);
            }

            transferRequestVoucher.IsDeleted = true;
            _transferRequestVoucherRepository.Update(transferRequestVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Lock(Guid id)
        {
            var transferRequestVoucher = _transferRequestVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (transferRequestVoucher == null)
                return ApiResponse.BadRequest(MessageConstant.TransferRequestVoucherNotFound);

            transferRequestVoucher.Locked = true;

            _transferRequestVoucherRepository.Update(transferRequestVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> Unlock(Guid id)
        {
            var transferRequestVoucher = _transferRequestVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (transferRequestVoucher == null)
                return ApiResponse.BadRequest(MessageConstant.TransferRequestVoucherNotFound);

            transferRequestVoucher.Locked = false;

            _transferRequestVoucherRepository.Update(transferRequestVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetTransferRequestVoucher(Guid id)
        {
            var transferRequestVoucher = _transferRequestVoucherQueryable.Include(x => x.Details)
                .ThenInclude(x => x.Product)
                .Select(x => new TransferRequestVoucherViewModel
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
                        : x.Details.Select(y => new TransferRequestVoucherDetailViewModel
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            ProductName = y.ProductName,
                            Product = new FetchProductViewModel()
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name
                            },
                        }).ToList(),
                }).FirstOrDefault(x => x.Id == id);

            if (transferRequestVoucher == null)
                return ApiResponse.NotFound(MessageConstant.TransferRequestVoucherNotFound);

            return ApiResponse.Ok(transferRequestVoucher);
        }

        public IActionResult FetchTransferRequestVoucherOutbound(FetchModel model)
        {
            var transferRequestVouchers = _transferRequestVoucherQueryable.AsNoTracking().Where(x =>
                    (string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword)) &&
                    x.OutboundWarehouseId == CurrentUser.Warehouse)
                .Take(model.Size).Select(x => new FetchTransferRequestVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(transferRequestVouchers);
        }
        
        public IActionResult FetchTransferRequestVoucherInbound(FetchModel model)
        {
            var transferRequestVouchers = _transferRequestVoucherQueryable.AsNoTracking().Where(x =>
                    (string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword)) &&
                    x.InboundWarehouseId == CurrentUser.Warehouse)
                .Take(model.Size).Select(x => new FetchTransferRequestVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(transferRequestVouchers);
        }

        public IActionResult SearchTransferRequestVoucherInWarehouse(SearchTransferRequestVoucherInWarehouseModel model)
        {
            var query = _transferRequestVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchTransferRequestVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
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

        public IActionResult SearchTransferRequestVoucherByInboundWarehouse(SearchTransferRequestVoucherByWarehouseModel model)
        {
            var query = _transferRequestVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchTransferRequestVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
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
        
        public IActionResult SearchTransferRequestVoucherByOutboundWarehouse(SearchTransferRequestVoucherByWarehouseModel model)
        {
            var query = _transferRequestVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchTransferRequestVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
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

        public IActionResult SearchTransferRequestVoucherAllWarehouse(
            SearchTransferRequestVoucherAllWarehouseModel model)
        {
            var query = _transferRequestVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchTransferRequestVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Description = x.Description,
                Status = x.Status,
                Locked = x.Locked,
                CreatedAt = x.CreatedAt,
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