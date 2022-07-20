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
    [Route("fixing-voucher")]
    [Authentication]
    public class FixingVoucherController : BaseController
    {
        private IFixingVoucherService _fixingVoucherService;
        
        public FixingVoucherController(IFixingVoucherService fixingVoucherService)
        {
            _fixingVoucherService = fixingVoucherService;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateFixingVoucher(CreateFixingVoucherModel model)
        {
            return await _fixingVoucherService.CreateFixingVoucher(model);
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateFixingVoucher(UpdateFixingVoucherModel model)
        {
            return await _fixingVoucherService.UpdateFixingVoucher(model);
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveFixingVoucher(Guid id)
        {
            return await _fixingVoucherService.RemoveFixingVoucher(id);
        }

        [HttpGet]
        public IActionResult GetFixingVoucher(Guid id)
        {
            return _fixingVoucherService.GetFixingVoucher(id);
        }

        [Route("fetch")]
        [HttpPost]
        public IActionResult FetchFixingVoucher(FetchModel model)
        {
            return _fixingVoucherService.FetchFixingVoucher(model);
        }

        [Route("search")]
        [HttpPost]
        public IActionResult SearchFixingVoucherInWarehouse(SearchFixingVoucherInWarehouseModel model)
        {
            return _fixingVoucherService.SearchFixingVoucherInWarehouse(model);
        }

        [Route("search/warehouse")]
        [HttpPost]
        public IActionResult SearchFixingVoucherByWarehouse(SearchFixingVoucherByWarehouseModel model)
        {
            return _fixingVoucherService.SearchFixingVoucherByWarehouse(model);
        }

        [Route("search/all")]
        [HttpPost]
        public IActionResult SearchFixingVoucherAllWarehouse(SearchFixingVoucherAllWarehouseModel model)
        {
            return _fixingVoucherService.SearchFixingVoucherAllWarehouse(model);
        }
    }
}
