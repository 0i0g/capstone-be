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
using Application.RequestModels.Auth;
using Application.ViewModels.Permission;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Helper;
using Permission = Data.Entities.Permission;

namespace Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            _logger.LogInformation($"Login with user: {JsonConvert.SerializeObject(model)}");

            // Get user claims
            var user = _userRepository
                .GetMany(x => x.Username == model.Username)
                .Include(x => x.UserInGroups)
                .ThenInclude(x => x.Group)
                .ThenInclude(x => x.Permissions)
                .Include(x => x.UserSetting)
                .Include(x=>x.Avatar)
                .FirstOrDefault();

            if (user == null)
            {
                return ApiResponse.NotFound(MessageConstant.UserNotFound);
            }

            var passwordHashed = PasswordHelper.Hash(model.Password);
            if (user.Password != passwordHashed)
            {
                return ApiResponse.NotFound(MessageConstant.IncorrectPassword);
            }

            if (user.IsActive == false)
            {
                return ApiResponse.NotFound(MessageConstant.UserBanned);
            }

            if (user.Confirmed == false)
            {
                return ApiResponse.NotFound(MessageConstant.UserNotConfirmed);
            }

            // Generate token
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            SaveRefreshToken(user.Id, refreshToken);

            // Save all
            await _unitOfWork.SaveChanges();

            // Response view model
            var authView = new AuthViewModel
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return ApiResponse.Ok(authView);
        }

        /// <summary>
        /// Verifies the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public AuthUser VerifyToken(string token)
        {
            AuthUser authUser = null;
            try
            {
                var payload = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(ConfigurationHelper.Configuration["JWT:Secret"])
                    .MustVerifySignature()
                    .Decode<IDictionary<string, string>>(token);
                _logger.LogInformation($"Payload: {payload}");

                if (payload.TryGetValue("userId", out var id) &&
                    payload.TryGetValue("groups", out var roles) &&
                    payload.TryGetValue("permissions", out var permissions))
                {
                    authUser = new AuthUser
                    {
                        Id = Guid.Parse(id),
                        Roles = roles.Split(','),
                        Permissions = permissions.Split(','),
                    };
                }
            }
            catch (Exception)
            {
                authUser = null;
            }

            return authUser;
        }

        public async Task<IActionResult> GetAccessToken(GetAccessTokenModel model)
        {
            var user = _authTokenRepository
                .GetMany(x => x.UserId == model.UserId && x.RefreshToken == model.RefreshToken)
                .Include(x => x.User)
                .ThenInclude(x => x.UserInGroups)
                .ThenInclude(x => x.Group)
                .ThenInclude(x => x.Permissions)
                .Select(x => x.User)
                .FirstOrDefault(x => x.IsDeleted == false && x.IsActive == true && x.Confirmed == true);

            if (user == null) return ApiResponse.Unauthorized();
            var authView = new AuthViewModel
            {
                UserId = user.Id,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = model.RefreshToken
            };

            await _unitOfWork.SaveChanges();

            return ApiResponse.Ok(authView);
        }

        private string GenerateAccessToken(User user)
        {
            var groups = user.UserInGroups.Select(x => x.Group.Name);
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation($"UserGroup: [{string.Join(',', user.UserInGroups.Select(x => x.Group))}]");

            var permissions = new List<PermissionViewModel>();
            foreach (var group in user.UserInGroups.Select(x => x.Group))
            {
                foreach (var permission in group.Permissions)
                {
                    var perExisted = permissions.FirstOrDefault(x => x.PermissionType == permission.PermissionType);
                    if (perExisted == null)
                    {
                        permissions.Add(new PermissionViewModel
                        {
                            PermissionType = permission.PermissionType,
                            Level = permission.Level
                        });
                    }
                    else
                    {
                        permissions.Remove(perExisted);
                        perExisted.Level |= permission.Level;
                        permissions.Add(perExisted);
                    }
                }
            }

            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation($"Permission: [{string.Join(',', permissions)}]");

            const int expiryMinuteDefault = 525600; // 1 year
            var _groups = string.Join(",", groups);
            var _permissions = string.Join(",", permissions);
            var expiryMinuteValue = ConfigurationHelper.Configuration["JWT:ExpiryMinute"];
            int expiryMinute = int.TryParse(expiryMinuteValue, out expiryMinute) ? expiryMinute : expiryMinuteDefault;

            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(ConfigurationHelper.Configuration["JWT:Secret"])
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(expiryMinute).ToUnixTimeSeconds())
                .AddClaim("userId", user.Id.ToString())
                .AddClaim("groups", _groups)
                .AddClaim("permissions", _permissions)
                .Encode();
        }

        private static string GenerateRefreshToken()
        {
            return GenerateHelper.RandomString(40);
        }

        private void SaveRefreshToken(Guid userId, string refreshToken)
        {
            _authTokenRepository.Add(new AuthToken {Id = Guid.NewGuid(), RefreshToken = refreshToken, UserId = userId});
        }
    }
}