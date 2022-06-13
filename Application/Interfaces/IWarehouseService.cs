using Application.RequestModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<IActionResult> CreateWarehouse(CreateWarehouseModel model);

        IActionResult SearchWarehouses(SearchWarehousesModel model);

        IActionResult FetchWarehouses(FetchModel model);

        Task<IActionResult> UpdateWarehouse(UpdateWarehouseModel model);

        Task<IActionResult> RemoveWarehouse(RemoveModel model);

        IActionResult GetWarehouse(Guid id);

        IActionResult GetAllWarehouses();
    }
}
