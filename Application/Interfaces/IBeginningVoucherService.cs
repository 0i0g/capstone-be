using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IBeginningVoucherService
    {
        Task<IActionResult> CreateBeginningVoucher(CreateBeginningVoucherModel model);

        Task<IActionResult> UpdateBeginningVoucher(UpdateBeginningVoucherModel model);
        
        Task<IActionResult> RemoveBeginningVoucher(Guid id);

        IActionResult GetBeginningVoucher(Guid id);
        
        IActionResult FetchBeginningVoucher(FetchModel model);

        IActionResult SearchBeginningVoucherInWarehouse(SearchBeginningVoucherInWarehouseModel model);
        
        IActionResult SearchBeginningVoucherByWarehouse(SearchBeginningVoucherByWarehouseModel model);
        
        IActionResult SearchBeginningVoucherAllWarehouse(SearchBeginningVoucherAllWarehouseModel model);
    }
}