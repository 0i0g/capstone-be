using Application.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Data.Entities;

namespace API.Configurations.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PermissionRequiredAttribute : ValidationAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public PermissionRequiredAttribute(params string[] permissions)
        {
            _permissions = permissions.Select(x => x.ToLower()).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var user = context.HttpContext.Items["CurrentUser"] as AuthUser;
                var pers = user!.Permissions.Select(x => x.ToLower());
                if (!pers.Any(per => _permissions.Contains(per)))
                {
                    context.Result = ApiResponse.Forbidden();
                }
            }
            catch (Exception e)
            {
                context.Result = ApiResponse.Unauthorized();
            }
        }
    }
}