using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IActionResult> CreateCategory(CreateCategoryModel model);

        IActionResult SearchCategories(SearchCategoriesModel model);

        IActionResult FetchCategories(FetchModel model);

        Task<IActionResult> UpdateCategory(UpdateCategoryModel model);

        Task<IActionResult> RemoveCategory(RemoveModel model);

        Task<IActionResult> RemoveMulCategory(List<Guid> ids);
        
        IActionResult GetCategory(Guid id);

        IActionResult GetAllCategories();
    }
}