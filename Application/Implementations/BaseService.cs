using System;
using Data.Entities;
using Data_EF;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Implementations
{
    public class BaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IServiceProvider provider)
        {
            _unitOfWork = provider.GetService<IUnitOfWork>();
            _httpContextAccessor = provider.GetService<IHttpContextAccessor>();
        }

        public AuthUser CurrentUser => (AuthUser)_httpContextAccessor.HttpContext.Items["CurrentUser"];
    }
}