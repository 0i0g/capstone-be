using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("delivery-request-voucher")]
    public class DeliveryRequestVoucherController : BaseController
    {
        private readonly IDeliveryRequestVoucherService _deliveryRequestVoucherService;

        public DeliveryRequestVoucherController(IDeliveryRequestVoucherService deliveryRequestVoucherService)
        {
            _deliveryRequestVoucherService = deliveryRequestVoucherService;
        }

        [Authentication]
        [HttpPost]
        public Task<IActionResult> CreateDeliveryRequestVoucher(CreateDeliveryRequestVoucherModel model)
        {
            return _deliveryRequestVoucherService.CreateDeliveryRequestVoucher(model);
        }

        [Authentication]
        [HttpPut]
        public Task<IActionResult> UpdateDeliveryRequestVoucher(UpdateDeliveryRequestVoucherModel model)
        {
            return _deliveryRequestVoucherService.UpdateDeliveryRequestVoucher(model);
        }

        [Authentication]
        [HttpDelete]
        public Task<IActionResult> RemoveDeliveryRequestVoucher(Guid id)
        {
            return _deliveryRequestVoucherService.RemoveDeliveryRequestVoucher(id);
        }

        // [Route("detail")]
        // [Authentication]
        // [HttpPost]
        // public Task<IActionResult> AddDeliveryRequestVoucherDetail(CreateDeliveryRequestVoucherDetailModel model)
        // {
        //     return _deliveryRequestVoucherService.AddDeliveryRequestVoucherDetail(model);
        // }
        //
        // [Route("detail")]
        // [Authentication]
        // [HttpPut]
        // public Task<IActionResult> UpdateDeliveryRequestVoucherDetail(UpdateDeliveryRequestVoucherDetailModel model)
        // {
        //     return _deliveryRequestVoucherService.UpdateDeliveryRequestVoucherDetail(model);
        // }
        //
        // [Route("detail")]
        // [Authentication]
        // [HttpDelete]
        // public Task<IActionResult> DeleteDeliveryRequestVoucherDetail(Guid id)
        // {
        //     return _deliveryRequestVoucherService.DeleteDeliveryRequestVoucherDetail(id);
        // }

        [Route("lock")]
        [Authentication]
        [HttpPut]
        public Task<IActionResult> Lock(Guid id)
        {
            return _deliveryRequestVoucherService.Lock(id);
        }

        [Route("unlock")]
        [Authentication]
        [HttpPut]
        public Task<IActionResult> Unlock(Guid id)
        {
            return _deliveryRequestVoucherService.Unlock(id);
        }

        // [Route("status")]
        // [Authentication]
        // [HttpPut]
        // public Task<IActionResult> UpdateDeliveryRequestVoucherStatus(UpdateDeliveryRequestVoucherStatusModel model)
        // {
        //     return _deliveryRequestVoucherService.UpdateDeliveryRequestVoucherStatus(model);
        // }

        [Authentication]
        [HttpGet]
        public IActionResult GetDeliveryRequestVoucher(Guid id)
        {
            return _deliveryRequestVoucherService.GetDeliveryRequestVoucher(id);
        }

        [Route("fetch")]
        [Authentication]
        [HttpPost]
        public IActionResult FetchDeliveryRequestVoucher(FetchModel model)
        {
            return _deliveryRequestVoucherService.FetchDeliveryRequestVoucher(model);
        }

        [Route("search")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchDeliveryRequestVoucherInWarehouse(SearchDeliveryRequestVoucherInWarehouseModel model)
        {
            return _deliveryRequestVoucherService.SearchDeliveryRequestVoucherInWarehouse(model);
        }

        [Route("search/warehouse")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchDeliveryRequestVoucherByWarehouse(SearchDeliveryRequestVoucherByWarehouseModel model)
        {
            return _deliveryRequestVoucherService.SearchDeliveryRequestVoucherByWarehouse(model);
        }

        [Route("search/warehouse/all")]
        [Authentication]
        [HttpPost]
        public IActionResult SearchDeliveryRequestVoucherAllWarehouse(
            SearchDeliveryRequestVoucherAllWarehouseModel model)
        {
            return _deliveryRequestVoucherService.SearchDeliveryRequestVoucherAllWarehouse(model);
        }
    }
}