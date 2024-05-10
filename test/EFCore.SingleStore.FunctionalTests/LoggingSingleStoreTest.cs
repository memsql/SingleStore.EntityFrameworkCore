using System;
using System.Reflection;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // TODO: Reenable once this issue has been fixed in EF Core upstream.
    // Skip because LoggingTestBase uses the wrong order:
    // Wrong:   DefaultOptions + "NoTracking"
    // Correct: "NoTracking" + DefaultOptions
    // The order in LoggingRelationalTestBase<,> is correct though.
    internal class LoggingSingleStoreTest : LoggingRelationalTestBase<SingleStoreDbContextOptionsBuilder, SingleStoreOptionsExtension>
    {
        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            IServiceCollection services,
            Action<RelationalDbContextOptionsBuilder<SingleStoreDbContextOptionsBuilder, SingleStoreOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder()
                .UseInternalServiceProvider(services.AddEntityFrameworkSingleStore().BuildServiceProvider(validateScopes: true))
                .UseSingleStore("Database=DummyDatabase", relationalAction);

        protected override string ProviderName => "EntityFrameworkCore.SingleStore";

        protected override string ProviderVersion => typeof(SingleStoreOptionsExtension).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        protected override string DefaultOptions => $"ServerVersion {AppConfig.ServerVersion} ";
    }
}
