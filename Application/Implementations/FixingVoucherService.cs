using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.FixingVoucher;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class FixingVoucherService : BaseService, IFixingVoucherService
    {
        private readonly IFixingVoucherRepository _fixingVoucherRepository;
        private readonly IQueryable<FixingVoucher> _fixingVoucherQueryable;
        private readonly IQueryable<FixingVoucher> _fixingVoucherAllWarehouseQueryable;
        private readonly IProductRepository _productRepository;
        private readonly IQueryable<Product> _productsQueryable;

        public FixingVoucherService(IServiceProvider provider) : base(provider)
        {
            _fixingVoucherRepository = _unitOfWork.FixingVoucher;
            _fixingVoucherQueryable =
                _fixingVoucherRepository.GetMany(x => x.IsDeleted != true && x.WarehouseId == CurrentUser.Warehouse)
                    .Include(x => x.Details);
            _fixingVoucherAllWarehouseQueryable = _fixingVoucherRepository.GetMany(x => x.IsDeleted != true)
                .Include(x => x.Details);
            _productRepository = _unitOfWork.Product;
            _productsQueryable =
                _productRepository.GetMany(x => x.IsDeleted != true  && x.IsActive == true);
        }

        public async Task<IActionResult> CreateFixingVoucher(CreateFixingVoucherModel model)
        {
            if (CurrentUser.Warehouse == null)
            {
                return ApiResponse.BadRequest(MessageConstant.RequiredWarehouse);
            }

            var newFixingVoucher = new FixingVoucher
            {
                ReportingDate = model.ReportingDate,
                Description = model.Description,
                WarehouseId = CurrentUser.Warehouse!.Value
            };

            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.FixingVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateFixingVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                newFixingVoucher.Details = model.Details.Select(x => new FixingVoucherDetail
                {
                    Quantity = x.Quantity,
                    Type = x.Type,
                    VoucherId = newFixingVoucher.Id,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y=>y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _fixingVoucherRepository.Add(newFixingVoucher);

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> UpdateFixingVoucher(UpdateFixingVoucherModel model)
        {
            var fixingVoucher =
                _fixingVoucherQueryable.Include(x => x.Details).FirstOrDefault(x => x.Id == model.Id);
            if (fixingVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.FixingVoucherNotFound);
            }

            fixingVoucher.ReportingDate = model.ReportingDate ?? fixingVoucher.ReportingDate;
            fixingVoucher.Description = model.Description ?? fixingVoucher.Description;
            
            if (model.Details != null)
            {
                if (model.Details.Count == 0)
                    return ApiResponse.BadRequest(MessageConstant.FixingVoucherDetailEmpty);

                var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
                if (duplicateDetail.Count > 0)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateFixingVoucherDetailsProduct);
                }

                var products = _productsQueryable.Where(x => model.Details.Select(y => y.ProductId).Contains(x.Id))
                    .Select(x => x.Id).ToList();
                var failProducts = model.Details.Select(x => x.ProductId).Except(products).ToList();
                if (failProducts.Count > 0)
                {
                    return ApiResponse.NotFound(
                        MessageConstant.ProductsInRangeNotFound.WithValues(string.Join(", ", failProducts)));
                }

                fixingVoucher.Details.Clear();
                fixingVoucher.Details = model.Details.Select(x => new FixingVoucherDetail
                {
                    VoucherId = fixingVoucher.Id,
                    Quantity = x.Quantity,
                    Type = x.Type,
                    ProductId = x.ProductId,
                    ProductName = _productsQueryable.FirstOrDefault(y=>y.Id == x.ProductId)?.Name
                }).ToList();
            }

            _fixingVoucherRepository.Update(fixingVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public async Task<IActionResult> RemoveMulFixingVoucher(Guid id)
        {
            var fixingVoucher =
                _fixingVoucherQueryable.Include(x => x.Details).FirstOrDefault(x => x.Id == id);
            if (fixingVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.FixingVoucherNotFound);
            }

            fixingVoucher.IsDeleted = true;
            _fixingVoucherRepository.Update(fixingVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetFixingVoucher(Guid id)
        {
            var fixingVoucher = _fixingVoucherQueryable.Include(x => x.Details).ThenInclude(x => x.Product)
                .Select(x => new FixingVoucherViewModel
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
                        : x.Details.Select(y => new FixingVoucherDetailViewModel
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            Type = y.Type,
                            ProductName = y.ProductName,
                            Product = new FetchProductViewModel()
                            {
                                Id = y.ProductId,
                                Name = y.Product.Name
                            },
                        }).OrderBy(y => y.ProductName).ToList(),
                }).FirstOrDefault(x => x.Id == id);

            if (fixingVoucher == null) return ApiResponse.NotFound(MessageConstant.FixingVoucherNotFound);

            return ApiResponse.Ok(fixingVoucher);
        }

        public IActionResult FetchFixingVoucher(FetchModel model)
        {
            var fixingVoucher = _fixingVoucherQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Code.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchFixingVoucherViewModel
                {
                    Id = x.Id,
                    Code = x.Code
                }).ToList();

            return ApiResponse.Ok(fixingVoucher);
        }

        public IActionResult SearchFixingVoucherInWarehouse(SearchFixingVoucherInWarehouseModel model)
        {
            var query = _fixingVoucherQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchFixingVoucherViewModel
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

        public IActionResult SearchFixingVoucherByWarehouse(SearchFixingVoucherByWarehouseModel model)
        {
            var query = _fixingVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchFixingVoucherViewModel
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

        public IActionResult SearchFixingVoucherAllWarehouse(SearchFixingVoucherAllWarehouseModel model)
        {
            var query = _fixingVoucherAllWarehouseQueryable.AsNoTracking().Where(x =>
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

            var data = query.Select(x => new SearchFixingVoucherViewModel
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