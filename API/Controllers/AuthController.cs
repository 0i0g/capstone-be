﻿using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            return ApiResponse.Ok("Welcome to Warehouse Management");
        }

        [Route("auth/login")]
        [HttpPost]
        public async Task<IActionResult> Login()
        {
            return await _authService.Login();
        }

        [Route("auth/getaccesstoken")]
        [HttpPost]
        public async Task<IActionResult> GetAccessToken()
        {
            return await _authService.GetAccessToken();
        }
    }
}
