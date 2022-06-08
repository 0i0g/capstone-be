using System;
using System.Threading.Tasks;
using Application.RequestModels;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IUserGroupService
    {
        Task<IActionResult> CreateUserGroup(CreateUserGroupModel model, EnumUserGroupType type);

        Task<IActionResult> UpdateUserGroup(UpdateUserGroupModel model, EnumUserGroupType type);

        Task<IActionResult> AddUserInToGroup(AddUserInToGroupModel model, EnumUserGroupType type);

        Task<IActionResult> RemoveUserGroup(RemoveModel model, EnumUserGroupType type);

        IActionResult GetUserGroup(Guid id, EnumUserGroupType type);

        IActionResult SearchUserGroup(SearchUserGroupModel model, EnumUserGroupType type);

        IActionResult FetchUserGroup(FetchModel model, EnumUserGroupType type);

        IActionResult GetAllUserGroup(EnumUserGroupType type);

        IActionResult GetAllPermission(EnumUserGroupType type);
    }
}