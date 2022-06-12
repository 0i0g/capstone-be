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
    [Route("voucher/prefix")]
    public class VoucherPrefixCodeController : BaseController
    {
        private IVoucherPrefixCodeService _voucherPrefixCodeService;

        public VoucherPrefixCodeController(IVoucherPrefixCodeService voucherPrefixCodeService)
        {
            _voucherPrefixCodeService = voucherPrefixCodeService;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVoucherPrefixCode(UpdateVoucherPrefixCodeModel model)
        {
            return await _voucherPrefixCodeService.UpdateVoucherPrefixCode(model);
        }

        [HttpGet]
        public IActionResult GetAllVoucherPrefix()
        {
            return _voucherPrefixCodeService.GetAllVoucherPrefix();
        }
    }
}