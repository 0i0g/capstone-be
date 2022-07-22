using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("delivery-voucher")]
    public class DeliveryVoucherController : BaseController
    {
        private readonly IDeliveryVoucherService _deliveryVoucherService;

        public DeliveryVoucherController(IDeliveryVoucherService deliveryVoucherService)
        {
            _deliveryVoucherService = deliveryVoucherService;
        }
        
        [Authentication]
        [HttpPost]
        public Task<IActionResult> CreateDeliveryVoucher(CreateDeliveryVoucherModel model)
        {
            return _deliveryVoucherService.CreateDeliveryVoucher(model);
        }

        [Authentication]
        [HttpPut]
        public Task<IActionResult> UpdateDeliveryVoucher(UpdateDeliveryVoucherModel model)
        {
            return _deliveryVoucherService.UpdateDeliveryVoucher(model);
        }

        [Authentication]
        [HttpDelete]
        public Task<IActionResult> RemoveMulDeliveryVoucher(Guid id)
        {
            return _deliveryVoucherService.RemoveMulDeliveryVoucher(id);
        }

        [Route("lock")]
        [Authentication]
        [HttpPut]
        public Task<IActionResult> Lock(Guid id)
        {
            return _deliveryVoucherService.Lock(id);
        }

        [Route("unlock")]
        [Authentication]
        [HttpPut]
        public Task<IActionResult> Unlock(Guid id)
        {
            return _deliveryVoucherService.Unlock(id);
        }

        [Authentication]
        [HttpGet]
        public IActionResult GetDeliveryVoucher(Guid id)
        {
            return _deliveryVoucherService.GetDeliveryVoucher(id);
        }

        [Route("fetch")]
        [Authentication]
        [HttpPost]
        public IActionResult FetchDeliveryVoucher(FetchModel model)
        {
            return _deliveryVoucherService.FetchDeliveryVoucher(model);
        }

        [Route("search")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchDeliveryVoucherInWarehouse(SearchDeliveryVoucherInWarehouseModel model)
        {
            return _deliveryVoucherService.SearchDeliveryVoucherInWarehouse(model);
        }

        [Route("search/warehouse")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchDeliveryVoucherByWarehouse(SearchDeliveryVoucherByWarehouseModel model)
        {
            return _deliveryVoucherService.SearchDeliveryVoucherByWarehouse(model);
        }

        [Route("search/all")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchDeliveryVoucherAllWarehouse(
            SearchDeliveryVoucherAllWarehouseModel model)
        {
            return _deliveryVoucherService.SearchDeliveryVoucherAllWarehouse(model);
        }
    }
}