using Application.ViewModels;
using Data.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Utilities.Constants;

namespace API.Configurations.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequiredWarehouseAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Items["CurrentUser"] is not AuthUser )
            {
                context.Result = ApiResponse.Unauthorized();
            }

            var user = context.HttpContext.Items["CurrentUser"] as AuthUser;
            if (user?.Warehouse == null)
            {
                context.Result = ApiResponse.Forbidden(MessageConstant.RequiredWarehouse);
            }
        }
    }
}