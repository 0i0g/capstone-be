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
    [Route("beginning-voucher")]
    [Authentication]
    public class BeginningVoucherController : BaseController
    {
        private readonly IBeginningVoucherService _beginningVoucherService;

        public BeginningVoucherController(IBeginningVoucherService beginningVoucherService)
        {
            _beginningVoucherService = beginningVoucherService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserGroupSystem(CreateBeginningVoucherModel model)
        {
            return await _beginningVoucherService.CreateBeginningVoucher(model);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBeginningVoucher(UpdateBeginningVoucherModel model)
        {
            return await _beginningVoucherService.UpdateBeginningVoucher(model);
        }

        // [Route("details")]
        // [HttpPut]
        // public async Task<IActionResult> AddBeginningVoucherDetail(AddBeginningVoucherDetailModel model)
        // {
        //     return await _beginningVoucherService.♥AddBeginningVoucherDetail(model);
        // }

        [HttpDelete]
        public async Task<IActionResult> RemoveMulBeginningVoucher(Guid id)
        {
            return await _beginningVoucherService.RemoveMulBeginningVoucher(id);
        }
        
        [HttpGet]
        public IActionResult GetBeginningVoucher(Guid id)
        {
            return _beginningVoucherService.GetBeginningVoucher(id);
        }
        
        [Route("fetch")]
        [HttpPost]
        public IActionResult FetchBeginningVoucher(FetchModel model)
        {
            return _beginningVoucherService.FetchBeginningVoucher(model);
        }

        [Route("search")]
        [HttpPost]
        public IActionResult SearchBeginningVoucherInWarehouse(SearchBeginningVoucherInWarehouseModel model)
        {
            return _beginningVoucherService.SearchBeginningVoucherInWarehouse(model);
        }
        
        [Route("search/warehouse")]
        [HttpPost]
        public IActionResult SearchBeginningVoucherByWarehouse(SearchBeginningVoucherByWarehouseModel model)
        {
            return _beginningVoucherService.SearchBeginningVoucherByWarehouse(model);
        }
        
        [Route("search/all")]
        [HttpPost]
        public IActionResult SearchBeginningVoucherAllWarehouse(SearchBeginningVoucherAllWarehouseModel model)
        {
            return _beginningVoucherService.SearchBeginningVoucherAllWarehouse(model);
        }
    }
}