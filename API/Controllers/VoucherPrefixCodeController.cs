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
    public class VoucherPrefixCodeController : BaseController
    {
        private IVoucherPrefixCodeService _voucherPrefixCodeService;
        
        public VoucherPrefixCodeController(IVoucherPrefixCodeService voucherPrefixCodeService)
        {
            _voucherPrefixCodeService = voucherPrefixCodeService;
        }
    }
}
