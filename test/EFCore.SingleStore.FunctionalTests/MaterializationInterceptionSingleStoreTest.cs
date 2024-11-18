using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class MaterializationInterceptionSingleStoreTest : MaterializationInterceptionTestBase,
    IClassFixture<MaterializationInterceptionSingleStoreTest.MaterializationInterceptionSingleStoreFixture>
{
    public MaterializationInterceptionSingleStoreTest(MaterializationInterceptionSingleStoreFixture fixture)
        : base(fixture)
    {
    }

    public class MaterializationInterceptionSingleStoreFixture : SingletonInterceptorsFixtureBase
    {
        protected override string StoreName
            => "MaterializationInterception";

        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<ISingletonInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkSingleStore(), injectedInterceptors);

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            new SingleStoreDbContextOptionsBuilder(base.AddOptions(builder))
                .ExecutionStrategy(d => new SingleStoreExecutionStrategy(d));
            return builder;
        }
    }
}
