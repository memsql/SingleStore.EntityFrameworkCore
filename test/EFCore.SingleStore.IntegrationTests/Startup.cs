using System;
using System.Buffers;
using EntityFrameworkCore.SingleStore.IntegrationTests.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SingleStoreConnector;
using Newtonsoft.Json;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.IntegrationTests
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            Configuration = AppConfig.Config;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddMvc()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            ConfigureEntityFramework(services);

            services
                .AddLogging(builder =>
                    builder
                        .AddConfiguration(AppConfig.Config.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug()
                )
                .AddIdentity<AppIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDb>()
                .AddDefaultTokenProviders();

            services.AddControllers();
        }

        public static void ConfigureEntityFramework(IServiceCollection services)
        {
            services.AddDbContextPool<AppDb>(
                options => options.UseSingleStore(
                    GetConnectionString(),
                    mysqlOptions =>
                    {
                        mysqlOptions.MaxBatchSize(AppConfig.EfBatchSize);
                        mysqlOptions.UseNewtonsoftJson();

                        if (AppConfig.EfRetryOnFailure > 0)
                        {
                            mysqlOptions.EnableRetryOnFailure(AppConfig.EfRetryOnFailure, TimeSpan.FromSeconds(5), null);
                        }
                    }
            ));
        }

        private static string GetConnectionString()
        {
            var csb = new SingleStoreConnectionStringBuilder(AppConfig.ConnectionString);

            if (AppConfig.EfDatabase != null)
            {
                csb.Database = AppConfig.EfDatabase;
            }

            return csb.ConnectionString;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
