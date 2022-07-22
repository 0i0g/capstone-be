using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface ICheckingVoucherService
    {
        Task<IActionResult> CreateCheckingVoucher(CreateCheckingVoucherModel model);

        Task<IActionResult> UpdateCheckingVoucher(UpdateCheckingVoucherModel model);
        
        Task<IActionResult> RemoveMulCheckingVoucher(Guid id);

        IActionResult GetCheckingVoucher(Guid id);
        
        IActionResult FetchCheckingVoucher(FetchModel model);

        IActionResult SearchCheckingVoucherInWarehouse(SearchCheckingVoucherInWarehouseModel model);
        
        IActionResult SearchCheckingVoucherByWarehouse(SearchCheckingVoucherByWarehouseModel model);
        
        IActionResult SearchCheckingVoucherAllWarehouse(SearchCheckingVoucherAllWarehouseModel model);
    }
}