using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface ITransferVoucherService
    {
        Task<IActionResult> CreateTransferVoucher(CreateTransferVoucherModel model);

        Task<IActionResult> UpdateTransferVoucher(UpdateTransferVoucherModel model);

        Task<IActionResult> ReceiveTransferVoucher(ReceiveTransferVoucherModel model);

        Task<IActionResult> RemoveMulTransferVoucher(Guid id);

        Task<IActionResult> Lock(Guid id);

        Task<IActionResult> Unlock(Guid id);

        IActionResult GetTransferVoucher(Guid id);

        IActionResult FetchTransferVoucherInbound(FetchModel model);

        IActionResult FetchTransferVoucherOutbound(FetchModel model);

        IActionResult SearchTransferVoucherInInboundWarehouse(SearchTransferVoucherInWarehouseModel model);

        IActionResult SearchTransferVoucherInOutboundWarehouse(SearchTransferVoucherInWarehouseModel model);

        IActionResult SearchTransferVoucherByInboundWarehouse(SearchTransferVoucherByWarehouseModel model);

        IActionResult SearchTransferVoucherByOutboundWarehouse(SearchTransferVoucherByWarehouseModel model);

        IActionResult SearchTransferVoucherAllWarehouse(SearchTransferVoucherAllWarehouseModel model);
    }
}