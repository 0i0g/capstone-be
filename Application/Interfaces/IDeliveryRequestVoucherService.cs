using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IDeliveryRequestVoucherService
    {
        Task<IActionResult> CreateDeliveryRequestVoucher(CreateDeliveryRequestVoucherModel model);

        Task<IActionResult> UpdateDeliveryRequestVoucher(UpdateDeliveryRequestVoucherModel model);

        Task<IActionResult> RemoveDeliveryRequestVoucher(Guid id);

        // Task<IActionResult> AddDeliveryRequestVoucherDetail(CreateDeliveryRequestVoucherDetailModel model);
        //
        // Task<IActionResult> UpdateDeliveryRequestVoucherDetail(UpdateDeliveryRequestVoucherDetailModel model);
        //
        // Task<IActionResult> DeleteDeliveryRequestVoucherDetail(Guid id);

        Task<IActionResult> Lock(Guid id);

        Task<IActionResult> Unlock(Guid id);

        // Task<IActionResult> UpdateDeliveryRequestVoucherStatus(UpdateDeliveryRequestVoucherStatusModel model);

        IActionResult GetDeliveryRequestVoucher(Guid id);

        IActionResult FetchDeliveryRequestVoucher(FetchModel model);

        IActionResult SearchDeliveryRequestVoucherInWarehouse(SearchDeliveryRequestVoucherInWarehouseModel model);

        IActionResult SearchDeliveryRequestVoucherByWarehouse(SearchDeliveryRequestVoucherByWarehouseModel model);

        IActionResult SearchDeliveryRequestVoucherAllWarehouse(SearchDeliveryRequestVoucherAllWarehouseModel model);
    }
}