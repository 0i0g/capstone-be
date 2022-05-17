using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Configurations.Middleware
{
    public class JwtMiddleware
    {
        #region Properties

        private readonly RequestDelegate _next;

        #endregion

        #region Constructor

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Invoke

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authService">The user service.</param>
        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            var token = context.Request.Headers["authorization"].FirstOrDefault();
            if (token != null)
            {
                if (token.Contains(" "))
                {
                    token = token.Split(' ')[1];
                }
                context.Items["CurrentUser"] = authService.VerifyToken(token);
            }

            await _next(context);
        }

        #endregion
    }
}
