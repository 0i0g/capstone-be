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
    public class AuthController : BaseController
    {
        private IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Welcome()
        {
            return ApiResponse.Ok($"Welcome to {ConfigurationHelper.Configuration.GetValue<string>("AppName")}");
        }

       
        [Route("auth/login")]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            return await _authService.Login(model);
        }

        [Route("auth/getaccesstoken")]
        [HttpPost]
        public async Task<IActionResult> GetAccessToken(GetAccessTokenModel model)
        {
            return await _authService.GetAccessToken(model);
        }
    }
}
