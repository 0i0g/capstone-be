using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authentication]
    [Route("transfer-voucher")]
    public class TransferVoucherController : BaseController
    {
        private readonly ITransferVoucherService _transferVoucherService;

        public TransferVoucherController(ITransferVoucherService transferVoucherService)
        {
            _transferVoucherService = transferVoucherService;
        }
        
        [HttpPost]
        public Task<IActionResult> CreateTransferVoucher(CreateTransferVoucherModel model)
        {
            return _transferVoucherService.CreateTransferVoucher(model);
        }
        
        [HttpPut]
        public Task<IActionResult> UpdateTransferVoucher(UpdateTransferVoucherModel model)
        {
            return _transferVoucherService.UpdateTransferVoucher(model);
        }
        
        [Route("receive")]
        [HttpPut]
        public Task<IActionResult> ReceiveTransferVoucher(ReceiveTransferVoucherModel model)
        {
            return _transferVoucherService.ReceiveTransferVoucher(model);
        }
        
        [HttpDelete]
        public Task<IActionResult> RemoveMulTransferVoucher(Guid id)
        {
            return _transferVoucherService.RemoveMulTransferVoucher(id);
        }
        
        [Route("lock")]
        [HttpPut]
        public Task<IActionResult> Lock(Guid id)
        {
            return _transferVoucherService.Lock(id);
        }

        [Route("unlock")]
        [HttpPut]
        public Task<IActionResult> Unlock(Guid id)
        {
            return _transferVoucherService.Unlock(id);
        }
        
        [HttpGet]
        public IActionResult GetTransferVoucher(Guid id)
        {
            return _transferVoucherService.GetTransferVoucher(id);
        }

        [Route("fetch/inbound")]
        [HttpPost]
        public IActionResult FetchTransferVoucherInbound(FetchModel model)
        {
            return _transferVoucherService.FetchTransferVoucherInbound(model);
        }
        
        [Route("fetch/outbound")]
        [HttpPost]
        public IActionResult FetchTransferVoucherOutbound(FetchModel model)
        {
            return _transferVoucherService.FetchTransferVoucherOutbound(model);
        }
        
        [Route("search/inbound")]
        [HttpPost]
        public IActionResult SearchTransferVoucherInInboundWarehouse(SearchTransferVoucherInWarehouseModel model)
        {
            return _transferVoucherService.SearchTransferVoucherInInboundWarehouse(model);
        }
        
        [Route("search/outbound")]
        [HttpPost]
        public IActionResult SearchTransferVoucherInOutboundWarehouse(SearchTransferVoucherInWarehouseModel model)
        {
            return _transferVoucherService.SearchTransferVoucherInOutboundWarehouse(model);
        }
        
        [Route("search/warehouse/inbound")]
        [HttpPost]
        public IActionResult SearchTransferVoucherByInboundWarehouse(SearchTransferVoucherByWarehouseModel model)
        {
            return _transferVoucherService.SearchTransferVoucherByInboundWarehouse(model);
        }
        
        [Route("search/warehouse/outbound")]
        [HttpPost]
        public IActionResult SearchTransferVoucherByOutboundWarehouse(SearchTransferVoucherByWarehouseModel model)
        {
            return _transferVoucherService.SearchTransferVoucherByOutboundWarehouse(model);
        }
        
        [Route("search/all")]
        [HttpPost]
        public IActionResult SearchTransferVoucherAllWarehouse(SearchTransferVoucherAllWarehouseModel model)
        {
            return _transferVoucherService.SearchTransferVoucherAllWarehouse(model);
        }
    }
}