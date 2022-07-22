using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IDeliveryVoucherService
    {
        Task<IActionResult> CreateDeliveryVoucher(CreateDeliveryVoucherModel model);

        Task<IActionResult> UpdateDeliveryVoucher(UpdateDeliveryVoucherModel model);

        Task<IActionResult> RemoveMulDeliveryVoucher(Guid id);

        Task<IActionResult> Lock(Guid id);

        Task<IActionResult> Unlock(Guid id);

        IActionResult GetDeliveryVoucher(Guid id);

        IActionResult FetchDeliveryVoucher(FetchModel model);

        IActionResult SearchDeliveryVoucherInWarehouse(SearchDeliveryVoucherInWarehouseModel model);

        IActionResult SearchDeliveryVoucherByWarehouse(SearchDeliveryVoucherByWarehouseModel model);

        IActionResult SearchDeliveryVoucherAllWarehouse(SearchDeliveryVoucherAllWarehouseModel model);
    }
}