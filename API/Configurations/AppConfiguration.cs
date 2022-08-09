using API.Configurations.Middleware;
using Application.Implementations;
using Application.Interfaces;
using Data_EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Utilities.Helper;

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
            services.AddScoped<ICheckingVoucherService, CheckingVoucherService>();
            services.AddScoped<IFixingVoucherService, FixingVoucherService>();
            services.AddScoped<ITransferRequestVoucherService, TransferRequestVoucherService>();
            services.AddScoped<IDeliveryVoucherService, DeliveryVoucherService>();
            services.AddScoped<IReceiveVoucherService, ReceiveVoucherService>();
            services.AddScoped<ITransferVoucherService, TransferVoucherService>();

            // Every controller and every service
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            #endregion
        }

        public static void AddTrigger(this IServiceCollection services)
        {
            #region Trigger

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(ConfigurationHelper.Configuration.GetConnectionString("Default"),
                    x => x.MigrationsAssembly("Data-EF"));
            });

            #endregion
        }

        public static void AddExecuteSqlRaw(this IApplicationBuilder app, AppDbContext dbContext)
        {
            #region Function

            // dbContext.Database.ExecuteSqlRaw(
            //     File.ReadAllText(DataHelper.MapPath("SqlQuery/Functions/function.sql")));

            #endregion

            #region Procedure

            dbContext.Database.ExecuteSqlRaw(
                File.ReadAllText(DataHelper.MapPath("SqlQuery/Procedures/sp_get_voucher_code.sql")));

            #endregion

            #region Trigger

            dbContext.Database.ExecuteSqlRaw(
                File.ReadAllText(DataHelper.MapPath("SqlQuery/Triggers/tr_add_beginning_voucher_custom_code.sql")));
            dbContext.Database.ExecuteSqlRaw(
                File.ReadAllText(DataHelper.MapPath("SqlQuery/Triggers/tr_add_product_custom_code.sql")));

            #endregion

            #region View

            dbContext.Database.ExecuteSqlRaw(File.ReadAllText(DataHelper.MapPath("SqlQuery/Views/vw_sum_product.sql")));

            #endregion
        }
    }
}