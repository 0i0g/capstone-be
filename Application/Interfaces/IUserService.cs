using Application.RequestModels;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IUserService
    {
        IActionResult GetProfile();

        //Task<IActionResult> SelfUpdate(SelfUpdateModel model);

        //Task<ApiResponse> Activate(Guid id);

        //Task<IActionResult> Deactivate(Guid id);
    }
}
