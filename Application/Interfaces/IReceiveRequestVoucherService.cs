using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IReceiveRequestVoucherService
    {
        Task<IActionResult> CreateReceiveRequestVoucher(CreateReceiveRequestVoucherModel model);

        Task<IActionResult> UpdateReceiveRequestVoucher(UpdateReceiveRequestVoucherModel model);

        Task<IActionResult> RemoveMulReceiveRequestVoucher(Guid id);

        // Task<IActionResult> AddReceiveRequestVoucherDetail(CreateReceiveRequestVoucherDetailModel model);
        //
        // Task<IActionResult> UpdateReceiveRequestVoucherDetail(UpdateReceiveRequestVoucherDetailModel model);
        //
        // Task<IActionResult> DeleteReceiveRequestVoucherDetail(Guid id);

        Task<IActionResult> Lock(Guid id);

        Task<IActionResult> Unlock(Guid id);

        // Task<IActionResult> UpdateReceiveRequestVoucherStatus(UpdateReceiveRequestVoucherStatusModel model);

        IActionResult GetReceiveRequestVoucher(Guid id);

        IActionResult FetchReceiveRequestVoucher(FetchModel model);
        
        IActionResult SearchReceiveRequestVoucherInWarehouse(SearchReceiveRequestVoucherInWarehouseModel model);
        
        IActionResult SearchReceiveRequestVoucherByWarehouse(SearchReceiveRequestVoucherByWarehouseModel model);
        
        IActionResult SearchReceiveRequestVoucherAllWarehouse(SearchReceiveRequestVoucherAllWarehouseModel model);
    }
}