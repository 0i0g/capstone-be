using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IReceiveVoucherService
    {
        Task<IActionResult> CreateReceiveVoucher(CreateReceiveVoucherModel model);

        Task<IActionResult> UpdateReceiveVoucher(UpdateReceiveVoucherModel model);

        Task<IActionResult> RemoveMulReceiveVoucher(Guid id);

        Task<IActionResult> Lock(Guid id);

        Task<IActionResult> Unlock(Guid id);

        IActionResult GetReceiveVoucher(Guid id);

        IActionResult FetchReceiveVoucher(FetchModel model);

        IActionResult SearchReceiveVoucherInWarehouse(SearchReceiveVoucherInWarehouseModel model);

        IActionResult SearchReceiveVoucherByWarehouse(SearchReceiveVoucherByWarehouseModel model);

        IActionResult SearchReceiveVoucherAllWarehouse(SearchReceiveVoucherAllWarehouseModel model);
    }
}