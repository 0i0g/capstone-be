using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace API.Controllers
{
    [Authentication]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("profile")]
        [HttpGet]
        public IActionResult GetProfile()
        {
            return _userService.GetProfile();
        }
    }
}
