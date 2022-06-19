﻿using Application.RequestModels;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Application.RequestModels.User;
using Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IUserService
    {
        IActionResult GetProfile();

        IActionResult GetPermissions();

        Task<IActionResult> CreateUser(CreateUserModel model);

        Task<IActionResult> UpdateUser(UpdateUserModel model);

        Task<IActionResult> RemoveUser(Guid id);

        IActionResult SearchUser(SearchUserModel model);

        IActionResult FetchUser(FetchModel model);

        IActionResult GetAllUser();

        Task<IActionResult> SelfUpdate(SelfUpdateModel model);

        IActionResult GetAuthUser();
    }
}