using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Utilities.Helper;

namespace API.Controllers
{
    public class CustomerController : BaseController
    {
        private ICustomerService _customerService;
        
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
    }
}
