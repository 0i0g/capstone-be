using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Configurations;
using API.Configurations.Filter;
using API.Configurations.Middleware;
using Data_EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Utilities.Helper;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigurationHelper.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConfigurationHelper.Configuration.GetConnectionString("Default"),
                    x => x.MigrationsAssembly("Data-EF")));

            services.AddDependenceInjection();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // https://jasonwatmore.com/post/2021/10/12/net-return-enum-as-string-from-api
            // https://code-maze.com/global-error-handling-aspnetcore/
            services
                .AddControllers(options => { options.Filters.Add<ValidationFilter>(int.MinValue); })
                .AddNewtonsoftJson(option =>
                {
                    option.SerializerSettings.ContractResolver = new RequiredPropertiesContractResolver();
                    option.UseCamelCasing(true);
                })

                // TODO check if can remove it below, use 1 Json option style
                .AddJsonOptions(options =>
                {
                    // serialize enums as strings in api responses (e.g. Role)
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                    // ignore omitted parameters on models to enable optional params (e.g. CurrentUser update)
                    // options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine(env.EnvironmentName);
            if (env.IsDevelopment())
            {
            }

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            app.UseCors(x => x
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());

            app.UseHttpsRedirection();

            if (!ConfigurationHelper.Configuration.GetValue<bool>("ShowServerErrors"))
            {
                app.UseMiddleware<ErrorHandlerMiddleware>();
            }

            app.UseMiddleware<JwtMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}