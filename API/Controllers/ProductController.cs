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
    public class ProductController : BaseController
    {
        private IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
    }
}
