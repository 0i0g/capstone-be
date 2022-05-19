using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.RequestModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<IActionResult> Login(UserLoginModel model);

        AuthUser VerifyToken(string token);

        Task<IActionResult> GetAccessToken(GetAccessTokenModel model);
    }
}
