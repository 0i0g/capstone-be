using System;
using System.Collections.Generic;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class CategoryController : BaseController
    {
        private ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        [Route("category")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryModel model)
        {
            return await _categoryService.CreateCategory(model);
        }
        
        // [PermissionRequired("Permission.Category.Read")]
        [Route("category/search")]
        [HttpPost]
        public IActionResult SearchCategories(SearchCategoriesModel model)
        {
            return _categoryService.SearchCategories(model);
        }

        // [PermissionRequired("Permission.Category.Read")]
        [Route("category/fetch")]
        [HttpPost]
        public IActionResult FetchCategories(FetchModel model)
        {
            return _categoryService.FetchCategories(model);
        }

        // [PermissionRequired("Permission.Category.Update")]
        [Route("category")]
        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryModel model)
        {
            return await _categoryService.UpdateCategory(model);
        }

        [Route("category")]
        [HttpDelete]
        public async Task<IActionResult> RemoveMulCategory(List<Guid> ids)
        {
            return await _categoryService.RemoveMulCategory(ids);
        }

        // [PermissionRequired("Permission.Category.Read")]
        [Route("category")]
        [HttpGet]
        public IActionResult GetCategory(Guid id)
        {
            return _categoryService.GetCategory(id);
        }
        
        // [PermissionRequired("Permission.Category.Read")]
        [Route("category/all")]
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            return _categoryService.GetAllCategories();
        }
    }
}
