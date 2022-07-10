using System;
using System.Threading.Tasks;
using API.Configurations.Filter;
using Application.Interfaces;
using Application.RequestModels;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserGroupController : BaseController
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        #region System

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Create")]
        [Route("system/group")]
        [HttpPost]
        public async Task<IActionResult> CreateUserGroupSystem(CreateUserGroupModel model)
        {
            return await _userGroupService.CreateUserGroup(model, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Update")]
        [Route("system/group")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserGroupSystem(UpdateUserGroupModel model)
        {
            return await _userGroupService.UpdateUserGroup(model, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Update")]
        [Route("system/group/adduser")]
        [HttpPost]
        public async Task<IActionResult> AddUserInToGroupSystem(AddUserInToGroupModel model)
        {
            return await _userGroupService.AddUserInToGroup(model, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Delete")]
        [Route("system/group")]
        [HttpDelete]
        public async Task<IActionResult> RemoveUserGroupSystem(RemoveModel model)
        {
            return await _userGroupService.RemoveUserGroup(model, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Read")]
        [Route("system/group")]
        [HttpGet]
        public IActionResult GetUserGroupSystem(Guid id)
        {
            return _userGroupService.GetUserGroup(id, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Read")]
        [Route("system/group/search")]
        [HttpPost]
        public IActionResult SearchUserGroupSystem(SearchUserGroupModel model)
        {
            return _userGroupService.SearchUserGroup(model, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Read")]
        [Route("system/group/fetch")]
        [HttpPost]
        public IActionResult FetchUserGroupSystem(FetchModel model)
        {
            return _userGroupService.FetchUserGroup(model, EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Read")]
        [Route("system/group/all")]
        [HttpGet]
        public IActionResult GetAllUserGroupSystem()
        {
            return _userGroupService.GetAllUserGroup(EnumUserGroupType.System);
        }

        [Authentication]
        // [PermissionRequired("Permission.UserGroupSystem.Read")]
        [Route("system/group/allpermission")]
        [HttpGet]
        public IActionResult GetAllPermissionSystem()
        {
            return _userGroupService.GetAllPermission(EnumUserGroupType.System);
        }

        #endregion

        #region Warehouse

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Create")]
        [Route("warehouse/group")]
        [HttpPost]
        public async Task<IActionResult> CreateUserGroupWarehouse(CreateUserGroupModel model)
        {
            return await _userGroupService.CreateUserGroup(model, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Update")]
        [Route("warehouse/group")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserGroupWarehouse(UpdateUserGroupModel model)
        {
            return await _userGroupService.UpdateUserGroup(model, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Update")]
        [Route("warehouse/group/adduser")]
        [HttpPost]
        public async Task<IActionResult> AddUserInToGroupWarehouse(AddUserInToGroupModel model)
        {
            return await _userGroupService.AddUserInToGroup(model, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Delete")]
        [Route("warehouse/group")]
        [HttpDelete]
        public async Task<IActionResult> RemoveUserGroupWarehouse(RemoveModel model)
        {
            return await _userGroupService.RemoveUserGroup(model, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Read")]
        [Route("warehouse/group")]
        [HttpGet]
        public IActionResult GetUserGroupWarehouse(Guid id)
        {
            return _userGroupService.GetUserGroup(id, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Read")]
        [Route("warehouse/group/search")]
        [HttpPost]
        public IActionResult SearchUserGroupWarehouse(SearchUserGroupModel model)
        {
            return _userGroupService.SearchUserGroup(model, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Read")]
        [Route("warehouse/group/fetch")]
        [HttpPost]
        public IActionResult FetchUserGroupWarehouse(FetchModel model)
        {
            return _userGroupService.FetchUserGroup(model, EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Read")]
        [Route("warehouse/group/all")]
        [HttpGet]
        public IActionResult GetAllUserGroupWarehouse()
        {
            return _userGroupService.GetAllUserGroup(EnumUserGroupType.Warehouse);
        }

        [Authentication(warehouseRequired: true)]
        // [PermissionRequired("Permission.UserGroupWarehouse.Read")]
        [Route("warehouse/group/allpermission")]
        [HttpGet]
        public IActionResult GetAllPermissionWarehouse()
        {
            return _userGroupService.GetAllPermission(EnumUserGroupType.Warehouse);
        }

        #endregion
    }
}