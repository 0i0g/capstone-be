using Application.Interfaces;
using Application.RequestModels;
using Application.ViewModels;
using Data.Entities;
using Data_EF;
using Data_EF.Repositories;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Implements;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Utilities.Constants;
using Utilities.Helper;

namespace Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = unitOfWork.User;
            _authTokenRepository = unitOfWork.AuthToken;
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
                .FirstOrDefault();

            if (user == null || user.IsDeleted == true)
            {
                return ApiResponse.NotFound(MessageConstant.AccountNotFound);
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
                    .Decode<IDictionary<string, object>>(token);
                _logger.LogInformation($"Payload: {payload}");

                if (payload.TryGetValue("userId", out var id) &&
                    payload.TryGetValue("permissions", out var permissions) &&
                    payload.TryGetValue("warehouse", out var warehouse) &&
                    payload.TryGetValue("groups", out var groups))
                {
                    authUser = new AuthUser
                    {
                        Id = Guid.Parse((string) id),
                        Permissions = ((JArray) permissions).ToObject<ICollection<string>>(),
                        Warehouse = warehouse != null ? Guid.Parse((string) warehouse) : null,
                        Groups = ((JArray) groups).ToObject<ICollection<AuthUserGroup>>(),
                    };
                }
            }
            catch (Exception e)
            {
                authUser = null;
            }

            return authUser;
        }

        public async Task<IActionResult> GetAccessToken(GetAccessTokenModel model)
        {
            var user = _authTokenRepository
                .GetMany(x =>
                    x.UserId == model.UserId && x.RefreshToken == model.RefreshToken && x.User.IsDeleted == false &&
                    x.User.IsActive == true)
                .Include(x => x.User)
                .ThenInclude(x => x.UserInGroups)
                .ThenInclude(x => x.Group)
                .ThenInclude(x => x.Permissions)
                .Select(x => x.User)
                .FirstOrDefault();

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
            var permissions = user.UserInGroups?.Select(x => x.Group)
                .SelectMany(x => x.Permissions, (_, permission) => permission.Type).Distinct();

            var groups = user.UserInGroups?.Select(x => x.Group).Select(x => new AuthUserGroup()
            {
                Name = x.Name,
                Type = x.Type
            }) ?? new List<AuthUserGroup>();

            const int expiryMinuteDefault = 525600; // 1 year
            var expiryMinuteValue = ConfigurationHelper.Configuration["JWT:ExpiryMinute"];
            int expiryMinute = int.TryParse(expiryMinuteValue, out expiryMinute) ? expiryMinute : expiryMinuteDefault;

            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(ConfigurationHelper.Configuration["JWT:Secret"])
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(expiryMinute).ToUnixTimeSeconds())
                .AddClaim("userId", user.Id.ToString())
                .AddClaim("permissions", permissions)
                .AddClaim("groups", groups)
                .AddClaim("warehouse", user.InWarehouseId)
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