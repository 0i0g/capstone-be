using System;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class CustomerController : BaseController
    {
        private ICustomerService _customerService;
        
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        [PermissionRequired("Permission.Customer.Create")]
        [Route("customer")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerModel model)
        {
            return await _customerService.CreateCustomer(model);
        }
        
        [PermissionRequired("Permission.Customer.Read")]
        [Route("customer/search")]
        [HttpPost]
        public IActionResult SearchCustomers(SearchCustomersModel model)
        {
            return _customerService.SearchCustomers(model);
        }

        [PermissionRequired("Permission.Customer.Read")]
        [Route("customer/fetch")]
        [HttpPost]
        public IActionResult FetchCustomers(FetchModel model)
        {
            return _customerService.FetchCustomers(model);
        }

        [PermissionRequired("Permission.Customer.Update")]
        [Route("customer")]
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerModel model)
        {
            return await _customerService.UpdateCustomer(model);
        }

        [PermissionRequired("Permission.Customer.Delete")]
        [Route("customer")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCustomer(RemoveModel model)
        {
            return await _customerService.RemoveCustomer(model);
        }

        [PermissionRequired("Permission.Customer.Read")]
        [Route("customer")]
        [HttpGet]
        public IActionResult GetCustomer(Guid id)
        {
            return _customerService.GetCustomer(id);
        }
        
        [PermissionRequired("Permission.Customer.Read")]
        [Route("customer/all")]
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            return _customerService.GetAllCustomers();
        }
    }
}
