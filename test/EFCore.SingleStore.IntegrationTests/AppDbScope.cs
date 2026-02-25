using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.IntegrationTests
{
    public class AppDbScope : IDisposable
    {
        private static ServiceProvider CreateServiceProvider(DbConnection connection = null, string sessionTimeZone = null)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(builder =>
                    builder
                        .AddConfiguration(AppConfig.Config.GetSection("Logging"))
                        .AddConsole()
                );

            // Register baseline EF services/config (existing behavior)
            Startup.ConfigureEntityFramework(serviceCollection);

            // If a session time zone is requested, re-register AppDb with the override.
            // Last registration wins for GetService<AppDb>().
            if (!string.IsNullOrEmpty(sessionTimeZone))
            {
                serviceCollection.AddDbContext<AppDb>(options =>
                {
                    if (connection != null)
                    {
                        options.UseSingleStore(connection, o => o.SessionTimeZone(sessionTimeZone));
                    }
                    else
                    {
                        options.UseSingleStore(AppConfig.ConnectionString, o => o.SessionTimeZone(sessionTimeZone));
                    }
                });
            }

            return serviceCollection.BuildServiceProvider();
        }

        private static readonly Lazy<ServiceProvider> DefaultLazyServiceProvider = new(() => CreateServiceProvider());

        private IServiceScope _scope;

        public AppDbScope()
        {
            var serviceProvider = DefaultLazyServiceProvider.Value;
            _scope = serviceProvider.CreateScope();
        }

        public AppDbScope(DbConnection connection = null)
        {
            var serviceProvider = CreateServiceProvider(connection);
            _scope = serviceProvider.CreateScope();
        }

        public AppDbScope(string sessionTimeZone, DbConnection connection = null)
        {
            var serviceProvider = CreateServiceProvider(connection, sessionTimeZone);
            _scope = serviceProvider.CreateScope();
        }

        public AppDb AppDb => _scope.ServiceProvider.GetService<AppDb>();

        public void Dispose()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }
}

