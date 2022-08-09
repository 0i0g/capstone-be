using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<IActionResult> CreateProduct(CreateProductModel model);

        IActionResult SearchProducts(SearchProductsModel model);

        IActionResult FetchProducts(FetchModel model);

        Task<IActionResult> UpdateProduct(UpdateProductModel model);

        Task<IActionResult> RemoveProduct(RemoveModel model);

        Task<IActionResult> RemoveMulProduct(List<Guid> ids);

        IActionResult GetProduct(Guid id);

        IActionResult GetAllProducts();

        IActionResult SearchSumProduct(SearchSumProductModel model, bool isInCurrentWarehouse);
    }
}