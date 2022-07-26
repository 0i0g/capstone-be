using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authentication]
    [Route("receive-voucher")]
    public class ReceiveVoucherController : BaseController
    {
        private readonly IReceiveVoucherService _receiveVoucherService;

        public ReceiveVoucherController(IReceiveVoucherService receiveVoucherService)
        {
            _receiveVoucherService = receiveVoucherService;
        }
        
        [HttpPost]
        public Task<IActionResult> CreateReceiveVoucher(CreateReceiveVoucherModel model)
        {
            return _receiveVoucherService.CreateReceiveVoucher(model);
        }

        [HttpPut]
        public Task<IActionResult> UpdateReceiveVoucher(UpdateReceiveVoucherModel model)
        {
            return _receiveVoucherService.UpdateReceiveVoucher(model);
        }

        [HttpDelete]
        public Task<IActionResult> RemoveMulReceiveVoucher(Guid id)
        {
            return _receiveVoucherService.RemoveMulReceiveVoucher(id);
        }

        [Route("lock")]
        [HttpPut]
        public Task<IActionResult> Lock(Guid id)
        {
            return _receiveVoucherService.Lock(id);
        }

        [Route("unlock")]
        [HttpPut]
        public Task<IActionResult> Unlock(Guid id)
        {
            return _receiveVoucherService.Unlock(id);
        }

        [HttpGet]
        public IActionResult GetReceiveVoucher(Guid id)
        {
            return _receiveVoucherService.GetReceiveVoucher(id);
        }

        [Route("fetch")]
        [HttpPost]
        public IActionResult FetchReceiveVoucher(FetchModel model)
        {
            return _receiveVoucherService.FetchReceiveVoucher(model);
        }

        [Route("search")]
        [HttpPost]
        public IActionResult SearchReceiveVoucherInWarehouse(SearchReceiveVoucherInWarehouseModel model)
        {
            return _receiveVoucherService.SearchReceiveVoucherInWarehouse(model);
        }

        [Route("search/warehouse")]
        [HttpPost]
        public IActionResult SearchReceiveVoucherByWarehouse(SearchReceiveVoucherByWarehouseModel model)
        {
            return _receiveVoucherService.SearchReceiveVoucherByWarehouse(model);
        }

        [Route("search/all")]
        [HttpPost]
        public IActionResult SearchReceiveVoucherAllWarehouse(
            SearchReceiveVoucherAllWarehouseModel model)
        {
            return _receiveVoucherService.SearchReceiveVoucherAllWarehouse(model);
        }
    }
}