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
        private readonly IProductRepository _productRepository;
        private readonly IQueryable<Product> _productsQueryable;

        public BeginningVoucherService(IServiceProvider provider) : base(provider)
        {
            _beginningVoucherRepository = _unitOfWork.BeginningVoucher;
            _beginningVoucherQueryable =
                _beginningVoucherRepository.GetMany(x => x.IsDeleted != true).Include(x => x.Details);
            _productRepository = _unitOfWork.Product;
            _productsQueryable =
                _productRepository.GetMany(x => x.IsDeleted != true);
        }

        public async Task<IActionResult> CreateBeginningVoucher(CreateBeginningVoucherModel model)
        {
            var duplicateDetail = model.Details.GroupBy(x => x.ProductId).Where(y => y.Count() > 1).ToList();
            if (duplicateDetail.Count > 0)
            {
                return ApiResponse.BadRequest(MessageConstant.DuplicateBeginningVoucherDetailsProduct);
            }

            var newBeginningVoucher = new BeginningVoucher
            {
                ReportingDate = model.ReportingDate,
                Note = model.Note,
            };

            _beginningVoucherRepository.Add(newBeginningVoucher);

            if (model.Details.Count == 0)
                return ApiResponse.BadRequest(MessageConstant.BeginningVoucherDetailEmpty);

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
            }).ToList();

            // TODO use trigger
            // newBeginningVoucher.Details.Select(x =>
            // {
            //     x.ProductName = x.Product.Name;
            //     return x;
            // }).ToList();

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult SearchBeginningVoucher(SearchBeginningVoucherModel model)
        {
            var query = _beginningVoucherQueryable.Where(x =>
                (model.StartDate == null || x.ReportingDate >= model.StartDate) &&
                (model.StartDate == null || x.ReportingDate <= model.EndDate)).AsNoTracking();

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(MessageConstant.OrderByInvalid.WithValues("Name, CreatedAt"));
            }

            var data = query.Select(x => new BeginningVoucherViewModel
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Note = x.Note,
                Details = x.Details.Select(y=> new BeginningVoucherDetailViewModel
                {
                    Id = y.Id,
                    Quantity = y.Quantity,
                    ProductName = y.ProductName,
                }).ToList()
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        // TODO fix logic
        public async Task<IActionResult> UpdateBeginningVoucher(UpdateBeginningVoucherModel model)
        {
            var beginningVoucher = _beginningVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (beginningVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.BeginningVoucherNotFound);
            }

            beginningVoucher.ReportingDate = model.ReportingDate ?? beginningVoucher.ReportingDate;
            beginningVoucher.Note = model.Note ?? beginningVoucher.Note;

            var beginningVoucherDetail =
                beginningVoucher.Details.FirstOrDefault(x => x.Id == model.Detail.Id);
            if (beginningVoucherDetail == null)
            {
                return ApiResponse.NotFound(MessageConstant.BeginningVoucherDetailNotFound);
            }

            beginningVoucherDetail.Quantity = model.Detail.Quantity ?? beginningVoucherDetail.Quantity;

            if (model.Detail.ProductId != null)
            {
                var product = _productsQueryable.FirstOrDefault(x => x.Id == model.Detail.ProductId);
                if (product == null)
                {
                    return ApiResponse.NotFound(MessageConstant.ProductNotFound);
                }

                var failProduct = beginningVoucher.Details.FirstOrDefault(x =>
                    x.ProductId == model.Detail.ProductId && x.Id != model.Detail.Id);
                if (failProduct != null)
                {
                    return ApiResponse.BadRequest(MessageConstant.DuplicateBeginningVoucherDetailsProduct);
                }

                beginningVoucherDetail.ProductId = model.Detail.ProductId ?? beginningVoucherDetail.ProductId;
            }

            _beginningVoucherRepository.Update(beginningVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        // TODO fix logic
        public async Task<IActionResult> AddBeginningVoucherDetail(AddBeginningVoucherDetailModel model)
        {
            var beginningVoucher = _beginningVoucherQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (beginningVoucher == null)
            {
                return ApiResponse.NotFound(MessageConstant.BeginningVoucherNotFound);
            }

            var product = _productsQueryable.FirstOrDefault(x => x.Id == model.Detail.ProductId);
            if (product == null)
            {
                return ApiResponse.NotFound(MessageConstant.ProductNotFound);
            }

            var failProduct = beginningVoucher.Details.FirstOrDefault(x => x.ProductId == model.Detail.ProductId);
            if (failProduct != null)
            {
                return ApiResponse.BadRequest(MessageConstant.DuplicateBeginningVoucherDetailsProduct);
            }

            beginningVoucher.Details.Add(new BeginningVoucherDetail
            {
                Quantity = model.Detail.Quantity,
                VoucherId = beginningVoucher.Id,
                ProductId = model.Detail.ProductId,
                ProductName = product.Name
            });
            _beginningVoucherRepository.Update(beginningVoucher);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public Task<IActionResult> RemoveBeginningVoucher(Guid id)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetBeginningVoucher(Guid id)
        {
            var beginningVoucher = _beginningVoucherQueryable.Select(x => new BeginningVoucherViewModel
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Note = x.Note,
                Details = x.Details.Select(y => new BeginningVoucherDetailViewModel
                {
                    Id = y.Id,
                    Quantity = y.Quantity,
                    ProductName = y.ProductName
                }).ToList()
            }).FirstOrDefault(x => x.Id == id);

            if (beginningVoucher == null) return ApiResponse.NotFound(MessageConstant.BeginningVoucherNotFound);

            return ApiResponse.Ok(beginningVoucher);
        }

        public IActionResult GetAllBeginningVouchers()
        {
            var beginningVouchers = _beginningVoucherQueryable.Select(x => new BeginningVoucherViewModel
            {
                Id = x.Id,
                Code = x.Code,
                ReportingDate = x.ReportingDate,
                Note = x.Note,
                Details = x.Details.Select(y => new BeginningVoucherDetailViewModel
                {
                    Id = y.Id,
                    Quantity = y.Quantity,
                    ProductName = y.ProductName
                }).ToList()
            }).ToList();

            return ApiResponse.Ok(beginningVouchers);
        }
    }
}