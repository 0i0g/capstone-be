using System;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Utilities.Helper;

namespace API.Controllers
{
    [Authentication]
    public class WarehouseController : BaseController
    {
        private IWarehouseService _warehouseService;
        
        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [Route("warehouse")]
        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(CreateWarehouseModel model)
        {
            return await _warehouseService.CreateWarehouse(model);
        }
        
        [Route("warehouse/search")]
        [HttpPost]
        public IActionResult SearchWarehouses(SearchWarehousesModel model)
        {
            return _warehouseService.SearchWarehouses(model);
        }

        [Route("warehouse/fetch")]
        [HttpPost]
        public IActionResult FetchWarehouses(FetchModel model)
        {
            return _warehouseService.FetchWarehouses(model);
        }

        [Route("warehouse")]
        [HttpPut]
        public async Task<IActionResult> UpdateWarehouse(UpdateWarehouseModel model)
        {
            return await _warehouseService.UpdateWarehouse(model);
        }

        [Route("warehouse")]
        [HttpDelete]
        public async Task<IActionResult> RemoveWarehouse(RemoveModel model)
        {
            return await _warehouseService.RemoveWarehouse(model);
        }

        [Route("warehouse")]
        [HttpGet]
        public IActionResult GetWarehouse(Guid id)
        {
            return _warehouseService.GetWarehouse(id);
        }
        
        [Route("warehouse/all")]
        [HttpGet]
        public IActionResult GetAllWarehouses()
        {
            return _warehouseService.GetAllWarehouses();
        }
    }
}
