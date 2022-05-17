using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;

namespace API.Configurations.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) 
            {
                var result = ApiResponse.InternalServerError(MessageConstant.InternalServerError);
                await result.ExecuteResultAsync(new ActionContext
                {
                    HttpContext = context
                });
            }
        }
    }
}
