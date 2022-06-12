using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IBeginningVoucherService
    {
        Task<IActionResult> CreateBeginningVoucher(CreateBeginningVoucherModel model);

        IActionResult SearchBeginningVoucher(SearchBeginningVoucherModel model);

        Task<IActionResult> UpdateBeginningVoucher(UpdateBeginningVoucherModel model);

        Task<IActionResult> AddBeginningVoucherDetail(AddBeginningVoucherDetailModel model);
        
        Task<IActionResult> RemoveBeginningVoucher(RemoveModel model);

        IActionResult GetBeginningVoucher(Guid id);

        IActionResult GetAllBeginningVouchers();
    }
}