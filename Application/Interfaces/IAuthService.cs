using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<IActionResult> Login();

        AuthUser VerifyToken(string token);

        Task<IActionResult> GetAccessToken();
    }
}
