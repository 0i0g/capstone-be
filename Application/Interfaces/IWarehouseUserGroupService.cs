using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IWarehouseUserGroupService
    {
        Task<IActionResult> CreateUserGroup(CreateUserGroupModel model);
        
        Task<IActionResult> UpdateUserGroup(UpdateUserGroupModel model);
        
        Task<IActionResult> AddUserInToGroup(AddUserInToGroupModel model);
        
        Task<IActionResult> UpdateUserGroupPermission(UpdateUserGroupPermissionModel model);

        Task<IActionResult> RemoveUserGroup(Guid id);

        IActionResult GetUserGroup(Guid id);

        IActionResult SearchUserGroup(SearchUserGroupModel model);

        IActionResult FetchUserGroup(FetchModel model);

        IActionResult GetAllUserGroup();
    }
}