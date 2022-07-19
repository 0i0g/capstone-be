using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.RequestModels.User;


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

        [Route("user/profile")]
        [HttpGet]
        public IActionResult GetProfile()
        {
            return _userService.GetProfile();
        }

        [Route("user/permissions")]
        [HttpGet]
        public IActionResult GetPermissions()
        {
            return _userService.GetPermissions();
        }

        [Route("user")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            return await _userService.CreateUser(model);
        }

        [Route("admin/user/create")]
        [HttpPost]
        public async Task<IActionResult> CreateUserInWarehouse(CreateUserModel model)
        {
            return await _userService.CreateUserInWarehouse(model);
        }

        [Route("user")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            return await _userService.UpdateUser(model);
        }

        [Route("admin/user/update")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserInWarehouse(UpdateUserModel model)
        {
            return await _userService.UpdateUserInWarehouse(model);
        }

        // [HttpDelete]
        // public async Task<IActionResult> RemoveUser(Guid id)
        // {
        //     return await _userService.RemoveUser(id);
        // }   
        [Route("user")]
        [HttpDelete]
        public async Task<IActionResult> RemoveMulUser(List<Guid> ids)
        {
            return await _userService.RemoveMulUser(ids);
        }

        [Route("admin/user/removes")]
        [HttpDelete]
        public async Task<IActionResult> RemoveMulUserInWarehouse(List<Guid> ids)
        {
            return await _userService.RemoveMulUserInWarehouse(ids);
        }
        
        [Route("user/search")]
        [HttpPost]
        public IActionResult SearchUser(SearchUserModel model)
        {
            return _userService.SearchUser(model);
        }

        [Route("user/fetch")]
        [HttpPost]
        public IActionResult FetchUser(FetchModel model)
        {
            return _userService.FetchUser(model);
        }

        [Route("admin/user/fetch")]
        [HttpPost]
        public IActionResult FetchUserInWarehouse(FetchModel model)
        {
            return _userService.FetchUserInWarehouse(model);
        }

        [Route("user/all")]
        [HttpGet]
        public IActionResult GetAllUser()
        {
            return _userService.GetAllUser();
        }

        [Route("user/me")]
        [HttpPut]
        public async Task<IActionResult> SelfUpdate(SelfUpdateModel model)
        {
            return await _userService.SelfUpdate(model);
        }
        
        [Route("user/authuser")]
        [HttpGet]
        public IActionResult GetAuthUser()
        {
            return  _userService.GetAuthUser();
        }

        [Route("user")]
        [HttpGet]
        public IActionResult GetUser(Guid id)
        {
            return  _userService.GetUser(id);
        }
    }
}