using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IFixingVoucherService
    {
        Task<IActionResult> CreateFixingVoucher(CreateFixingVoucherModel model);

        Task<IActionResult> UpdateFixingVoucher(UpdateFixingVoucherModel model);
        
        Task<IActionResult> RemoveMulFixingVoucher(Guid id);

        IActionResult GetFixingVoucher(Guid id);
        
        IActionResult FetchFixingVoucher(FetchModel model);

        IActionResult SearchFixingVoucherInWarehouse(SearchFixingVoucherInWarehouseModel model);
        
        IActionResult SearchFixingVoucherByWarehouse(SearchFixingVoucherByWarehouseModel model);
        
        IActionResult SearchFixingVoucherAllWarehouse(SearchFixingVoucherAllWarehouseModel model);
    }
}