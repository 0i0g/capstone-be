using Application.ViewModels;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Utilities.Constants;

namespace API.Configurations.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthenticationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly bool _warehouseRequired;

        public AuthenticationAttribute(bool warehouseRequired = false)
        {
            _warehouseRequired = warehouseRequired;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Items["CurrentUser"] is not AuthUser user)
            {
                context.Result = ApiResponse.Unauthorized();
                return;
            }

            if (_warehouseRequired && (user.Warehouse == null || user.Warehouse == Guid.Empty))
            {
                context.Result = ApiResponse.Forbidden(MessageConstant.AccountNotInAnyWarehouse);
            }
        }
    }
}