using API.Configurations.Middleware;
using Application.Implementations;
using Application.Interfaces;
using Data_EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;

namespace API.Configurations
{
    public static class AppConfiguration
    {
        public static void AddDependenceInjection(this IServiceCollection services)
        {
            #region DI

            services.AddHttpContextAccessor();

            // Every request
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IBeginningVoucherService, BeginningVoucherService>();
            services.AddScoped<IReceiveRequestVoucherService, ReceiveRequestVoucherService>();
            services.AddScoped<IDeliveryRequestVoucherService, DeliveryRequestVoucherService>();
            services.AddScoped<IWarehouseService, WarehouseService>();

            // Every controller and every service
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            #endregion
        }
    }
}