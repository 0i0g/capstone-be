using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data_EF;
using Data_EF.Repositories;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Helper;

namespace Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userRepository = unitOfWork.User;
            _authTokenRepository = unitOfWork.AuthToken;
            _userSettingRepository = unitOfWork.UserSetting;
            _logger = logger;
        }

        /// <summary>
        /// Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public async Task<IActionResult> Login()
        {
            return ApiResponse.Ok();
        }

        /// <summary>
        /// Verifies the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public AuthUser VerifyToken(string token)
        {
            return new AuthUser();
        }

        public async Task<IActionResult> GetAccessToken()
        {
            return ApiResponse.Ok();
        }

        private static string GenerateAccessToken(User user, Guid buId)
        {
            return "";
        }

        private static string GenerateRefreshToken()
        {
            return GenerateHelper.RandomString(40);
        }

        private void SaveRefreshToken(Guid userId, string refreshToken)
        {
            _authTokenRepository.Add(new AuthToken { Id = Guid.NewGuid(), RefreshToken = refreshToken, UserId = userId });
        }
    }
}
