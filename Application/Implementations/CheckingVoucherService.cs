using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.CheckingVoucher;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class CheckingVoucherService : BaseService, ICheckingVoucherService
    {
        private readonly ICheckingVoucherRepository _checkingVoucherRepository;
        private readonly IQueryable<CheckingVoucher> _checkingVoucherQueryable;
        private readonly IQueryable<CheckingVoucher> _checkingVoucherAllWarehouseQueryable;
        private readonly IProductRepository _productRepository;
        private readonly IQueryable<Product> _productsQueryable;

        public CheckingVoucherService(IServiceProvider provider) : base(provider)
        {
            _checkingVoucherRepository = _unitOfWork.CheckingVoucher;
            _checkingVoucherQueryable =
                _checkingVoucherRepository.GetMany(x => x.IsDeleted != true && x.WarehouseId == CurrentUser.Warehouse)
                    .Include(x => x.Details);
            _checkingVoucherAllWarehouseQueryable = _checkingVoucherRepository.GetMany(x => x.IsDeleted != true)
                .Include(x => x.Details);
            _productRepository = _unitOfWork.Product;
            _productsQueryable =
                _productRepository.GetMany(x => x.IsDeleted != true);
        }

        public async Task<IActionResult> CreateCheckingVoucher(CreateCheckingVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            var newCheckingVoucher = new CheckingVoucher
            {
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                WarehouseId = CurrentUser.Warehouse!.Value
            };

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.CheckingVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateCheckingVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                newCheckingVoucher.Details = model.Details.Select(x => new CheckingVoucherDetail
                {
                    Quantity = x.Quantity,
                    RealQuantity = x.RealQuantity,
                    VoucherId = newCheckingVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y=>y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _checkingVoucherRepository.Add(newCheckingVoucher);

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateCheckingVoucher(UpdateCheckingVoucherModel model)
        {
            var checkingVoucher =
                _checkingVoucherQueryable.Include(x => x.Details).FirstOrDefault(x => x.Id == model.Id);
            if (checkingVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.CheckingVoucherNotFound);
            }

            checkingVoucher.ReportingDate = model.ReportingDate ?? checkingVoucher.ReportingDate;
            checkingVoucher.Description = model.Description ?? checkingVoucher.Description;
            
            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.CheckingVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateCheckingVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                checkingVoucher.Details.Clear();
                checkingVoucher.Details = model.Details.Select(x => new CheckingVoucherDetail
                {
                    VoucherId = checkingVoucher.Id,
                    Quantity = x.Quantity,
                    RealQuantity = x.RealQuantity,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y=>y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _checkingVoucherRepository.Update(checkingVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveCheckingVoucher(Guid id)
        {
            var checkingVoucher =
                _checkingVoucherQueryable.Include(x => x.Details).FirstOrDefault(x => x.Id == id);
            if (checkingVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.CheckingVoucherNotFound);
            }

            checkingVoucher.IsDeleted = true;
            _checkingVoucherRepository.Update(checkingVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetCheckingVoucher(Guid id)
        {
            var checkingVoucher = _checkingVoucherQueryable.Include(x => x.Details).ThenInclude(x => x.Product)
                .Select(x => new CheckingVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    ReportingDate = x.ReportingDate,
                    Description = x.Description,
                    CreateAt = x.CreatedAt,
                    CreateBy = x.CreatedBy == null
                        ? null
                        : new FetchUserViewModel
                        {
                            Id = x.CreatedBy.Id,
                            Name = x.CreatedBy.FullName,
                            Avatar = x.CreatedBy.Avatar
                        },
                    Warehouse = x.Warehouse == null
                        ? null
                        : new FetchWarehouseViewModel
                        {
                            Id = x.WarehouseId,
                            Name = x.Warehouse.Name
                        },
                    Details = x.Details == null
                        ? null
                        : x.Details.Select(y => new CheckingVoucherDetailViewModel
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            RealQuantity = y.RealQuantity,
                            ProductName = y.ProductName,
                            Product = new FetchProductViewModel()
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name
                            },
                        }).ToList(),
                }).FirstOrDefault(x => x.Id == id);

            if (checkingVoucher == null) return ApiResponse.NotFound(MessageConstant.CheckingVoucherNotFound);

            return ApiResponse.Ok(checkingVoucher);
        }

        public IActionResult FetchCheckingVoucher(FetchModel model)
        {
            var checkingVouchers = _checkingVoucherQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchCheckingVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(checkingVouchers);
        }

        public IActionResult SearchCheckingVoucherInWarehouse(SearchCheckingVoucherInWarehouseModel model)
        {
            var query = _checkingVoucherQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate));

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.ReportingDate);
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
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Code, ReportingDate, CreateAt"));
            }

            var data = query.Select(x => new SearchCheckingVoucherViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                Description = x.Description,
                ReportingDate = x.ReportingDate,
                Warehouse = x.Warehouse != null
                    ? new FetchWarehouseViewModel
                    {
                        Id = x.WarehouseId,
                        Name = x.Warehouse.Name
                    }
                    : null,
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchCheckingVoucherByWarehouse(SearchCheckingVoucherByWarehouseModel model)
        {
            var query = _checkingVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate) &&
                x.WarehouseId == model.WarehouseId);

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.ReportingDate);
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
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Code, ReportingDate, CreateAt"));
            }

            var data = query.Select(x => new SearchCheckingVoucherViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Description = x.Description,
                ReportingDate = x.ReportingDate,
                Warehouse = x.Warehouse != null
                    ? new FetchWarehouseViewModel
                    {
                        Id = x.WarehouseId,
                        Name = x.Warehouse.Name
                    }
                    : null,
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult SearchCheckingVoucherAllWarehouse(SearchCheckingVoucherAllWarehouseModel model)
        {
            var query = _checkingVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
                (string.IsNullOrWhiteSpace(model.Code) || x.Code.Contains(model.Code)) &&
                (model.FromDate == null || x.ReportingDate >= model.FromDate) &&
                (model.ToDate == null || x.ReportingDate <= model.ToDate));

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.ReportingDate);
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
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(
                        MessageConstant.OrderByInvalid.WithValues("Code, ReportingDate, CreateAt"));
            }

            var data = query.Select(x => new SearchCheckingVoucherViewModel
            {
                Id = x.Id,
                Code = x.Code,
                Description = x.Description,
                ReportingDate = x.ReportingDate,
                Warehouse = x.Warehouse != null
                    ? new FetchWarehouseViewModel
                    {
                        Id = x.WarehouseId,
                        Name = x.Warehouse.Name
                    }
                    : null,
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }
    }
}