using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.Category;
using Application.ViewModels.Product;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISumProductRepository _sumProductRepository;
        private readonly IQueryable<Product> _productsQueryable;
        private readonly IQueryable<Category> _categoriesQueryable;
        private readonly IUploadService _uploadService;

        public ProductService(IServiceProvider provider) : base(provider)
        {
            _productRepository = _unitOfWork.Product;
            _sumProductRepository = _unitOfWork.SumProduct;
            _productsQueryable = _productRepository.GetMany(x => x.IsDeleted != true).Include(x => x.Category);
            _categoriesQueryable = _unitOfWork.Category.GetMany(x => x.IsDeleted != true);
            _uploadService = provider.GetService<IUploadService>();
        }

        public async Task<IActionResult> CreateProduct(CreateProductModel model)
        {
            var product = _productsQueryable.FirstOrDefault(x => x.Name == model.Name);
            if (product != null)
            {
                return ApiResponse.BadRequest(MessageConstant.ProductNameExisted);
            }

            if (model.Category != null && _categoriesQueryable.FirstOrDefault(x => x.Id == model.Category) == null)
            {
                return ApiResponse.BadRequest(MessageConstant.CategoryNotFound);
            }

            // upload image
            var fileName = await _uploadService.UploadFile(model.Image);

            var newProduct = new Product
            {
                Name = model.Name,
                Description = model.Description,
                OnHandMin = model.OnHandMin,
                OnHandMax = model.OnHandMax,
                Image = fileName,
                CategoryId = model.Category
            };

            _productRepository.Add(newProduct);

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult SearchProducts(SearchProductsModel model)
        {
            var query = _productsQueryable.Where(x =>
                string.IsNullOrWhiteSpace(model.Name) || x.Name.Contains(model.Name)).AsNoTracking();

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
                case "NAME":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Name).ThenByDescending(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.Name).ThenByDescending(x => x.CreatedAt);
                    break;
                case "CREATEDAT":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    return ApiResponse.BadRequest(MessageConstant.OrderByInvalid.WithValues("Name, CreatedAt"));
            }

            var data = query.Select(x => new
            {
                x.Id,
                x.Code,
                x.Name,
                x.Description,
                x.IsActive,
                x.OnHandMin,
                x.OnHandMax,
                Categories = x.Category != null ? new {x.Category.Id, x.Category.Name} : null
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult FetchProducts(FetchModel model)
        {
            var products = _productsQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Name.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            return ApiResponse.Ok(products);
        }

        public async Task<IActionResult> UpdateProduct(UpdateProductModel model)
        {
            var product = _productsQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (product == null) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);

            if (model.Name != null && model.Name != product.Name)
            {
                var productConflictName = _productsQueryable.FirstOrDefault(x => x.Name == model.Name);
                if (productConflictName != null)
                {
                    return ApiResponse.BadRequest(MessageConstant.ProductNameExisted);
                }
            }

            product.Name = model.Name ?? product.Name;
            product.IsActive = model.IsActive ?? product.IsActive;
            product.Description = model.Description ?? product.Description;
            product.OnHandMin = model.OnHandMin ?? product.OnHandMin;
            product.OnHandMax = model.OnHandMax ?? product.OnHandMax;
            product.CategoryId = model.Category;

            if (model.Image != null)
            {
                _uploadService.DeleteFile(product.Image);
                var fileName = await _uploadService.UploadFile(model.Image);
                product.Image = fileName;
            }

            _productRepository.Update(product);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public Task<IActionResult> RemoveProduct(RemoveModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> RemoveMulProduct(List<Guid> ids)
        {
            var products = _productsQueryable.Where(x => ids.Contains(x.Id)).ToList();
            products.ForEach(x => x.IsDeleted = true);

            _productRepository.UpdateRange(products);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult GetProduct(Guid id)
        {
            var product = _productsQueryable.Select(x => new
            {
                x.Id,
                x.Code,
                x.Name,
                x.IsActive,
                x.Description,
                x.OnHandMin,
                x.OnHandMax,
                Categories = x.Category != null ? new {x.Category.Id, x.Category.Name} : null,
                x.Image
            }).FirstOrDefault(x => x.Id == id);
            if (product == null) return ApiResponse.BadRequest(MessageConstant.ProductNotFound);

            return ApiResponse.Ok(product);
        }

        public IActionResult GetAllProducts()
        {
            var products = _productsQueryable.Select(x => new
            {
                x.Id,
                x.Code,
                x.Name,
                x.IsActive,
                x.Description,
                x.OnHandMin,
                x.OnHandMax,
                Categories = x.Category != null ? new {x.Category.Id, x.Category.Name} : null,
            }).ToList();

            return ApiResponse.Ok(products);
        }

        public IActionResult SearchSumProduct(SearchSumProductModel model, bool isInCurrentWarehouse)
        {
            if (isInCurrentWarehouse)
            {
                model.Id = CurrentUser.Warehouse;
            }

            var query = _sumProductRepository.GetMany(x =>
                    (model.Id == null || x.WarehouseId == model.Id) &&
                    (model.Name == null || x.ProductName.Contains(model.Name)))
                .AsNoTracking()
                .GroupBy(x => new {x.ProductId, x.ProductName}).Select(x => new SumProductViewModel
                {
                    ProductId = x.Key.ProductId,
                    ProductName = x.Key.ProductName,
                    Quantity = x.Sum(y => y.Quantity)
                });

            switch (model.OrderByName)
            {
                case "":
                    query = query.OrderByDescending(x => x.ProductName);
                    break;
                case "NAME":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.ProductName)
                        : query.OrderByDescending(x => x.ProductName);
                    break;
                case "QUANTITY":
                    query = model.IsSortAsc
                        ? query.OrderBy(x => x.Quantity)
                        : query.OrderByDescending(x => x.Quantity);
                    break;
                default:
                    return ApiResponse.BadRequest(MessageConstant.OrderByInvalid.WithValues("Name, Quantity"));
            }

            var data = query
                .ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }
    }
}