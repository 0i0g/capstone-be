using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Application.RequestModels.UserGroup;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IUserGroupService
    {
        Task<IActionResult> CreateUserGroup(CreateUserGroupModel model);
        
        Task<IActionResult> UpdateUserGroup(UpdateUserGroupModel model);

        Task<IActionResult> UpdateUserGroupPermission(UpdateUserGroupPermissionModel model);

        Task<IActionResult> RemoveUserGroup(Guid id);

        IActionResult GetUserGroup(Guid id);

        IActionResult SearchUserGroup(SearchUserGroupModel model);

        IActionResult FetchUserGroup(FetchModel model);

        IActionResult GetAllUserGroup();
    }
}