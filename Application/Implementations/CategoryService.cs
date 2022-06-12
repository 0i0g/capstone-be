using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Application.ViewModels.Category;
using Data.Entities;
using Data_EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Constants;
using Utilities.Extensions;

namespace Application.Implementations
{
    public class CategoryService: BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IQueryable<Category> _categoriesQueryable;
        
        public CategoryService(IServiceProvider provider) : base(provider)
        {
            _categoryRepository = _unitOfWork.Category;
            _categoriesQueryable = _categoryRepository.GetMany(x => x.IsDeleted != true).Include(x=>x.ProductCategories);
        }

        public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
        {
            var category = _categoriesQueryable.FirstOrDefault(x => x.Name == model.Name);
            if (category != null)
            {
                return ApiResponse.BadRequest(MessageConstant.CategoryNameExisted);
            }
            
            var newCategory = new Category
            {
                Name = model.Name,
                Description = model.Description,
            };

            _categoryRepository.Add(newCategory);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public IActionResult SearchCategories(SearchCategoriesModel model)
        {
            var query = _categoriesQueryable.Where(x =>
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

            var data = _categoriesQueryable.Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductsNum = x.ProductCategories.Count
            }).ToPagination(model.PageIndex, model.PageSize);

            return ApiResponse.Ok(data);
        }

        public IActionResult FetchCategories(FetchModel model)
        {
            var categories = _categoriesQueryable.AsNoTracking().Where(x =>
                    string.IsNullOrWhiteSpace(model.Keyword) || x.Name.Contains(model.Keyword))
                .Take(model.Size).Select(x => new FetchCategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            return ApiResponse.Ok(categories);
        }

        public async Task<IActionResult> UpdateCategory(UpdateCategoryModel model)
        {
            var category = _categoriesQueryable.FirstOrDefault(x => x.Id == model.Id);
            if (category == null) return ApiResponse.BadRequest(MessageConstant.CategoryNotFound);
            
            if (model.Name != null && model.Name != category.Name)
            {
                var categoryConflictName = _categoriesQueryable.FirstOrDefault(x => x.Name == model.Name);
                if (categoryConflictName != null)
                {
                    return ApiResponse.BadRequest(MessageConstant.CategoryNameExisted);
                }
            }

            category.Name = model.Name ?? category.Name;
            category.Description = model.Description ?? category.Description;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok();
        }

        public Task<IActionResult> RemoveCategory(RemoveModel model)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetCategory(Guid id)
        {
            var category = _categoriesQueryable.Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductsNum = x.ProductCategories.Count
            }).FirstOrDefault(x => x.Id == id);
            if (category == null) return ApiResponse.BadRequest(MessageConstant.CategoryNotFound);

            return ApiResponse.Ok(category);
        }

        public IActionResult GetAllCategories()
        {
            var categories = _categoriesQueryable.Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductsNum = x.ProductCategories.Count
            }).ToList();

            return ApiResponse.Ok(categories);
        }
    }
}