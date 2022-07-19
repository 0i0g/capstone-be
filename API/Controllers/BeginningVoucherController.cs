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
    public class BeginningVoucherController : BaseController
    {
        private IBeginningVoucherService _beginningVoucherService;

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
        public async Task<IActionResult> RemoveBeginningVoucher(Guid id)
        {
            return await _beginningVoucherService.RemoveBeginningVoucher(id);
        }

        [Route("search")]
        [HttpPost]
        public IActionResult SearchBeginningVoucher(SearchBeginningVoucherModel model)
        {
            return _beginningVoucherService.SearchBeginningVoucher(model);
        }

        [HttpGet]
        public IActionResult GetBeginningVoucher(Guid id)
        {
            return _beginningVoucherService.GetBeginningVoucher(id);
        }

        [Route("all")]
        [HttpGet]
        public IActionResult GetAllBeginningVouchers()
        {
            return _beginningVoucherService.GetAllBeginningVouchers();
        }
    }
}