using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data.Enums;
using Data_EF;
using Data_EF.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utilities.Constants;
using Utilities.Extensions;
using Utilities.Helper;

namespace Application.Implementations
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IServiceProvider provider, ILogger<UserService> logger) : base(provider)
        {
            _userRepository = _unitOfWork.User;
            _logger = logger;
        }

        public IActionResult GetProfile()
        {
            var user = _userRepository.GetMany(x => x.Id == CurrentUser.Id).Select(x => new ProfileViewModel
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Avatar = x.Avatar != null ? x.Avatar.GetUrl() : null
            }).FirstOrDefault();

            if (user == null) return ApiResponse.NotFound(MessageConstant.ProfileNotFound);

            return ApiResponse.Ok(user);
        }
    }
}