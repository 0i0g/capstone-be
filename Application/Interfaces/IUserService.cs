using Application.RequestModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.RequestModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IUserService
    {
        IActionResult GetProfile();

        IActionResult GetPermissions();

        Task<IActionResult> CreateUser(CreateUserModel model);

        Task<IActionResult> CreateUserInWarehouse(CreateUserModel model);

        Task<IActionResult> UpdateUser(UpdateUserModel model);

        Task<IActionResult> UpdateUserInWarehouse(UpdateUserModel model);

        Task<IActionResult> RemoveUser(Guid id);

        Task<IActionResult> RemoveMulUser(List<Guid> id);

        Task<IActionResult> RemoveMulUserInWarehouse(List<Guid> id);

        IActionResult SearchUser(SearchUserModel model);

        IActionResult SearchUserInWarehouse(SearchUserModel model);

        IActionResult FetchUser(FetchModel model);

        IActionResult FetchUserInWarehouse(FetchModel model);

        IActionResult GetAllUser();

        Task<IActionResult> SelfUpdate(SelfUpdateModel model);

        IActionResult GetAuthUser();

        IActionResult GetUser(Guid id);
    }
}