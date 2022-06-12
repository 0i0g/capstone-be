using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IVoucherPrefixCodeService
    {
        Task<IActionResult> UpdateVoucherPrefixCode(UpdateVoucherPrefixCodeModel model);

        IActionResult GetAllVoucherPrefix();
    }
}