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
    [Route("checking-voucher")]
    [Authentication]
    public class CheckingVoucherController : BaseController
    {
        private ICheckingVoucherService _checkingVoucherService;

        public CheckingVoucherController(ICheckingVoucherService checkingVoucherService)
        {
            _checkingVoucherService = checkingVoucherService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckingVoucher(CreateCheckingVoucherModel model)
        {
            return await _checkingVoucherService.CreateCheckingVoucher(model);
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateCheckingVoucher(UpdateCheckingVoucherModel model)
        {
            return await _checkingVoucherService.UpdateCheckingVoucher(model);
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveCheckingVoucher(Guid id)
        {
            return await _checkingVoucherService.RemoveCheckingVoucher(id);
        }

        [HttpGet]
        public IActionResult GetCheckingVoucher(Guid id)
        {
            return _checkingVoucherService.GetCheckingVoucher(id);
        }

        [Route("fetch")]
        [HttpPost]
        public IActionResult FetchCheckingVoucher(FetchModel model)
        {
            return _checkingVoucherService.FetchCheckingVoucher(model);
        }

        [Route("search")]
        [HttpPost]
        public IActionResult SearchCheckingVoucherInWarehouse(SearchCheckingVoucherInWarehouseModel model)
        {
            return _checkingVoucherService.SearchCheckingVoucherInWarehouse(model);
        }

        [Route("search/warehouse")]
        [HttpPost]
        public IActionResult SearchCheckingVoucherByWarehouse(SearchCheckingVoucherByWarehouseModel model)
        {
            return _checkingVoucherService.SearchCheckingVoucherByWarehouse(model);
        }

        [Route("search/all")]
        [HttpPost]
        public IActionResult SearchCheckingVoucherAllWarehouse(SearchCheckingVoucherAllWarehouseModel model)
        {
            return _checkingVoucherService.SearchCheckingVoucherAllWarehouse(model);
        }
    }
}
