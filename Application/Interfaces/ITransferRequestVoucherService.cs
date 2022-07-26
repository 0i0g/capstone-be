using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface ITransferRequestVoucherService
    {
        Task<IActionResult> CreateTransferRequestVoucher(CreateTransferRequestVoucherModel model);

        Task<IActionResult> UpdateTransferRequestVoucher(UpdateTransferRequestVoucherModel model);

        Task<IActionResult> RemoveMulTransferRequestVoucher(Guid id);

        Task<IActionResult> Lock(Guid id);

        Task<IActionResult> Unlock(Guid id);

        IActionResult GetTransferRequestVoucher(Guid id);

        IActionResult FetchTransferRequestVoucherInbound(FetchModel model);

        IActionResult FetchTransferRequestVoucherOutbound(FetchModel model);

        IActionResult
            SearchTransferRequestVoucherInInboundWarehouse(SearchTransferRequestVoucherInWarehouseModel model);

        IActionResult SearchTransferRequestVoucherInOutboundWarehouse(
            SearchTransferRequestVoucherInWarehouseModel model);

        IActionResult
            SearchTransferRequestVoucherByInboundWarehouse(SearchTransferRequestVoucherByWarehouseModel model);

        IActionResult SearchTransferRequestVoucherByOutboundWarehouse(
            SearchTransferRequestVoucherByWarehouseModel model);

        IActionResult SearchTransferRequestVoucherAllWarehouse(SearchTransferRequestVoucherAllWarehouseModel model);
    }
}