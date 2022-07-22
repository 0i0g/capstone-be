using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authentication]
    [Route("transfer-request-voucher")]
    public class TransferRequestVoucherController: BaseController
    {
        private readonly ITransferRequestVoucherService _transferRequestVoucherService;

        public TransferRequestVoucherController(ITransferRequestVoucherService transferRequestVoucherService)
        {
            _transferRequestVoucherService = transferRequestVoucherService;
        }
        
        [HttpPost]
        public Task<IActionResult> CreateTransferRequestVoucher(CreateTransferRequestVoucherModel model)
        {
            return _transferRequestVoucherService.CreateTransferRequestVoucher(model);
        }
        
        [HttpPut]
        public Task<IActionResult> UpdateTransferRequestVoucher(UpdateTransferRequestVoucherModel model)
        {
            return _transferRequestVoucherService.UpdateTransferRequestVoucher(model);
        }
        
        [HttpDelete]
        public Task<IActionResult> RemoveMulTransferRequestVoucher(Guid id)
        {
            return _transferRequestVoucherService.RemoveMulTransferRequestVoucher(id);
        }
        
        [Route("lock")]
        [HttpPut]
        public Task<IActionResult> Lock(Guid id)
        {
            return _transferRequestVoucherService.Lock(id);
        }

        [Route("unlock")]
        [HttpPut]
        public Task<IActionResult> Unlock(Guid id)
        {
            return _transferRequestVoucherService.Unlock(id);
        }
        
        [HttpGet]
        public IActionResult GetTransferRequestVoucher(Guid id)
        {
            return _transferRequestVoucherService.GetTransferRequestVoucher(id);
        }

        [Route("fetch/outbound")]
        [HttpPost]
        public IActionResult FetchTransferRequestVoucherOutbound(FetchModel model)
        {
            return _transferRequestVoucherService.FetchTransferRequestVoucherOutbound(model);
        }
        
        [Route("fetch/inbound")]
        [HttpPost]
        public IActionResult FetchTransferRequestVoucherInbound(FetchModel model)
        {
            return _transferRequestVoucherService.FetchTransferRequestVoucherInbound(model);
        }
        
        [Route("search")]
        [HttpPost]
        public IActionResult SearchTransferRequestVoucherInWarehouse(SearchTransferRequestVoucherInWarehouseModel model)
        {
            return _transferRequestVoucherService.SearchTransferRequestVoucherInWarehouse(model);
        }
        
        [Route("search/warehouse/inbound")]
        [HttpPost]
        public IActionResult SearchTransferRequestVoucherByInboundWarehouse(SearchTransferRequestVoucherByWarehouseModel model)
        {
            return _transferRequestVoucherService.SearchTransferRequestVoucherByInboundWarehouse(model);
        }
        
        [Route("search/warehouse/outbound")]
        [HttpPost]
        public IActionResult SearchTransferRequestVoucherByOutboundWarehouse(SearchTransferRequestVoucherByWarehouseModel model)
        {
            return _transferRequestVoucherService.SearchTransferRequestVoucherByOutboundWarehouse(model);
        }
        
        [Route("search/all")]
        [HttpPost]
        public IActionResult SearchTransferRequestVoucherAllWarehouse(SearchTransferRequestVoucherAllWarehouseModel model)
        {
            return _transferRequestVoucherService.SearchTransferRequestVoucherAllWarehouse(model);
        }
    
    }
}