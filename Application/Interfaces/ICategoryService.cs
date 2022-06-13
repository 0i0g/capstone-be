using System;
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

        IActionResult GetCategory(Guid id);

        IActionResult GetAllCategories();
    }
}