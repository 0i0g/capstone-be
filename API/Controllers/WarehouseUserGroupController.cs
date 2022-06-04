using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [RequiredWarehouse]
    [Route("warehouse/group")]
    public class WarehouseUserGroupController : BaseController
    {
        private readonly IWarehouseUserGroupService _warehouseUserGroupService;

        public WarehouseUserGroupController(IWarehouseUserGroupService warehouseUserGroupService)
        {
            _warehouseUserGroupService = warehouseUserGroupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserGroup(CreateUserGroupModel model)
        {
            return await _warehouseUserGroupService.CreateUserGroup(model);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserGroup(UpdateUserGroupModel model)
        {
            return await _warehouseUserGroupService.UpdateUserGroup(model);
        }

        [Route("adduser")]
        [HttpPut]
        public async Task<IActionResult> AddUserInToGroup(AddUserInToGroupModel model)
        {
            return await _warehouseUserGroupService.AddUserInToGroup(model);
        }

        [Route("permission")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserGroupPermission(UpdateUserGroupPermissionModel model)
        {
            return await _warehouseUserGroupService.UpdateUserGroupPermission(model);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUserGroup(Guid id)
        {
            return await _warehouseUserGroupService.RemoveUserGroup(id);
        }

        [HttpGet]
        public IActionResult GetUserGroup(Guid id)
        {
            return _warehouseUserGroupService.GetUserGroup(id);
        }

        [Route("search")]
        [HttpPost]
        public IActionResult SearchUserGroup(SearchUserGroupModel model)
        {
            return _warehouseUserGroupService.SearchUserGroup(model);
        }

        [Route("fetch")]
        [HttpPost]
        public IActionResult FetchUserGroup(FetchModel model)
        {
            return _warehouseUserGroupService.FetchUserGroup(model);
        }

        [Route("all")]
        [HttpGet]
        public IActionResult GetAllUserGroup()
        {
            return _warehouseUserGroupService.GetAllUserGroup();
        }
    }
}