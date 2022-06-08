using Application.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Data.Entities;

namespace API.Configurations.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RoleRequiredAttribute : ValidationAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleRequiredAttribute(params string[] roles)
        {
            _roles = roles.Select(x => x.ToLower()).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var user = context.HttpContext.Items["CurrentUser"] as AuthUser;
                var groups = user!.Groups.Select(x => $"{x.Type.ToString()}.{x.Name.ToString()}".ToLower());
                if (!groups.Any(group => _roles.Contains(group)))
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