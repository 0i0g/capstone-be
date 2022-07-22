using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.BeginningVoucher;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class BeginningVoucherService : BaseService, IBeginningVoucherService
    {
        private readonly IBeginningVoucherRepository _beginningVoucherRepository;
        private readonly IQueryable<BeginningVoucher> _beginningVoucherQueryable;
        private readonly IQueryable<BeginningVoucher> _beginningVoucherAllWarehouseQueryable;
        private readonly IProductRepository _productRepository;
        private readonly IQueryable<Product> _productsQueryable;

        public BeginningVoucherService(IServiceProvider provider) : base(provider)
        {
            _beginningVoucherRepository = _unitOfWork.BeginningVoucher;
            _beginningVoucherQueryable =
                _beginningVoucherRepository.GetMany(x => x.IsDeleted != true && x.WarehouseId == CurrentUser.Warehouse)
                    .Include(x => x.Details);
            _beginningVoucherAllWarehouseQueryable = _beginningVoucherRepository.GetMany(x => x.IsDeleted != true)
                .Include(x => x.Details);
            _productRepository = _unitOfWork.Product;
            _productsQueryable =
                _productRepository.GetMany(x => x.IsDeleted != true && x.IsActive == true);
        }

        public async Task<IActionResult> CreateBeginningVoucher(CreateBeginningVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            var newBeginningVoucher = new BeginningVoucher
            {
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                WarehouseId = CurrentUser.Warehouse!.Value
            };

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.BeginningVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateBeginningVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                newBeginningVoucher.Details = model.Details.Select(x => new BeginningVoucherDetail
                {
                    Quantity = x.Quantity,
                    VoucherId = newBeginningVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _beginningVoucherRepository.Add(newBeginningVoucher);

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult SearchBeginningVoucherInWarehouse(SearchBeginningVoucherInWarehouseModel model)
        {
            var query = _beginningVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchBeginningVoucherViewModel
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

        public IActionResult SearchBeginningVoucherByWarehouse(SearchBeginningVoucherByWarehouseModel model)
        {
            var query = _beginningVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchBeginningVoucherViewModel
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

        public IActionResult SearchBeginningVoucherAllWarehouse(SearchBeginningVoucherAllWarehouseModel model)
        {
            var query = _beginningVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchBeginningVoucherViewModel
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

        public async Task<IActionResult> UpdateBeginningVoucher(UpdateBeginningVoucherModel model)
        {
            var beginningVoucher =
                _beginningVoucherQueryable.Include(x => x.Details).FirstOrDefault(x => x.Id == model.Id);
            if (beginningVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.BeginningVoucherNotFound);
            }

            beginningVoucher.ReportingDate = model.ReportingDate ?? beginningVoucher.ReportingDate;
            beginningVoucher.Description = model.Description ?? beginningVoucher.Description;

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.BeginningVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateBeginningVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                beginningVoucher.Details.Clear();
                beginningVoucher.Details = model.Details.Select(x => new BeginningVoucherDetail
                {
                    VoucherId = beginningVoucher.Id,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y => y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _beginningVoucherRepository.Update(beginningVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulBeginningVoucher(Guid id)
        {
            var beginningVoucher = _beginningVoucherQueryable.FirstOrDefault(x => x.Id == id);
            if (beginningVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.BeginningVoucherNotFound);
            }

            beginningVoucher.IsDeleted = true;
            _beginningVoucherRepository.Update(beginningVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetBeginningVoucher(Guid id)
        {
            var beginningVoucher = _beginningVoucherQueryable.Include(x => x.Details).ThenInclude(x => x.Product)
                .Select(x => new BeginningVoucherViewModel
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
                        : x.Details.Select(y => new BeginningVoucherDetailViewModel
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            ProductName = y.ProductName,
                            Product = new FetchProductViewModel()
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name
                            },
                        }).OrderBy(y => y.ProductName).ToList(),
                }).FirstOrDefault(x => x.Id == id);

            if (beginningVoucher == null) return ApiResponse.NotFound(MessageConstant.BeginningVoucherNotFound);

            return ApiResponse.Ok(beginningVoucher);
        }

        public IActionResult FetchBeginningVoucher(FetchModel model)
        {
            var beginningVouchers = _beginningVoucherQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchBeginningVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(beginningVouchers);
        }
    }
}