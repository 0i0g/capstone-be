using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("receive-voucher-request")]
    public class ReceiveRequestVoucherController : BaseController
    {
        private readonly IReceiveRequestVoucherService _receiveRequestVoucherService;

        public ReceiveRequestVoucherController(IReceiveRequestVoucherService receiveRequestVoucherService)
        {
            _receiveRequestVoucherService = receiveRequestVoucherService;
        }

        [Authentication]
        [HttpPost]
        public Task<IActionResult> CreateReceiveRequestVoucher(CreateReceiveRequestVoucherModel model)
        {
            return _receiveRequestVoucherService.CreateReceiveRequestVoucher(model);
        }

        [Authentication]
        [HttpPut]
        public Task<IActionResult> UpdateReceiveRequestVoucher(UpdateReceiveRequestVoucherModel model)
        {
            return _receiveRequestVoucherService.UpdateReceiveRequestVoucher(model);
        }

        [Authentication]
        [HttpDelete]
        public Task<IActionResult> RemoveReceiveRequestVoucher(Guid id)
        {
            return _receiveRequestVoucherService.RemoveReceiveRequestVoucher(id);
        }

        // [Route("detail")]
        // [Authentication]
        // [HttpPost]
        // public Task<IActionResult> AddReceiveRequestVoucherDetail(CreateReceiveRequestVoucherDetailModel model)
        // {
        //     return _receiveRequestVoucherService.AddReceiveRequestVoucherDetail(model);
        // }
        //
        // [Route("detail")]
        // [Authentication]
        // [HttpPut]
        // public Task<IActionResult> UpdateReceiveRequestVoucherDetail(UpdateReceiveRequestVoucherDetailModel model)
        // {
        //     return _receiveRequestVoucherService.UpdateReceiveRequestVoucherDetail(model);
        // }
        //
        // [Route("detail")]
        // [Authentication]
        // [HttpDelete]
        // public Task<IActionResult> DeleteReceiveRequestVoucherDetail(Guid id)
        // {
        //     return _receiveRequestVoucherService.DeleteReceiveRequestVoucherDetail(id);
        // }

        [Route("lock")]
        [Authentication]
        [HttpPut]
        public Task<IActionResult> Lock(Guid id)
        {
            return _receiveRequestVoucherService.Lock(id);
        }

        [Route("unlock")]
        [Authentication]
        [HttpPut]
        public Task<IActionResult> Unlock(Guid id)
        {
            return _receiveRequestVoucherService.Unlock(id);
        }

        // [Route("status")]
        // [Authentication]
        // [HttpPut]
        // public Task<IActionResult> UpdateReceiveRequestVoucherStatus(UpdateReceiveRequestVoucherStatusModel model)
        // {
        //     return _receiveRequestVoucherService.UpdateReceiveRequestVoucherStatus(model);
        // }

        [Authentication]
        [HttpGet]
        public IActionResult GetReceiveRequestVoucher(Guid id)
        {
            return _receiveRequestVoucherService.GetReceiveRequestVoucher(id);
        }

        [Route("fetch")]
        [Authentication]
        [HttpPost]
        public IActionResult FetchReceiveRequestVoucher(FetchModel model)
        {
            return _receiveRequestVoucherService.FetchReceiveRequestVoucher(model);
        }
        
        [Route("search")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchReceiveRequestVoucherInWarehouse(SearchReceiveRequestVoucherInWarehouseModel model)
        {
            return _receiveRequestVoucherService.SearchReceiveRequestVoucherInWarehouse(model);
        }
        
        [Route("search/warehouse")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchReceiveRequestVoucherByWarehouse(SearchReceiveRequestVoucherByWarehouseModel model)
        {
            return _receiveRequestVoucherService.SearchReceiveRequestVoucherByWarehouse(model);
        }
        
        [Route("search/all")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchReceiveRequestVoucherAllWarehouse(SearchReceiveRequestVoucherAllWarehouseModel model)
        {
            return _receiveRequestVoucherService.SearchReceiveRequestVoucherAllWarehouse(model);
        }
    }
}