using Application.ViewModels;
using Data.Entities;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Constants;

namespace API.Configurations.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _role;

        public AuthorizeRoleAttribute(params string[] role)
        {
            this._role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Items["CurrentUser"] is not AuthUser)
            {
                context.Result = ApiResponse.Unauthorized();
                return;
            }

            var forbiddenMessage = "You need" + (_role != null ? $" Role `{_role}`" : "") + " to perform this action";

            if (context.HttpContext.Items["CurrentUser"] is AuthUser user && _role!.Length != 0 && !user.Roles.Any(r => _role.Contains(r)))
            {
                context.Result =
                    ApiResponse.Forbidden(MessageConstant.RolePermissionForbidden.WithValues(forbiddenMessage));
            }
        }
    }
}