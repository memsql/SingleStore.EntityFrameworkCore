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

public class MaterializationInterceptionSingleStoreTest :
    MaterializationInterceptionTestBase<MaterializationInterceptionSingleStoreTest.SingleStoreLibraryContext>,
    IClassFixture<MaterializationInterceptionSingleStoreTest.MaterializationInterceptionSingleStoreFixture>
{
    public MaterializationInterceptionSingleStoreTest(MaterializationInterceptionSingleStoreFixture fixture)
        : base(fixture)
    {
    }

    public class SingleStoreLibraryContext : LibraryContext
    {
        public SingleStoreLibraryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<TestEntity30244>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<TestEntity30244>().OwnsMany(
                e => e.Settings, b =>
                {
                    b.Property<long>("Id").HasColumnType("bigint");
                });

            modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings);

            // TODO: https://github.com/npgsql/efcore.pg/issues/2548
            // modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings, b => b.ToJson());
        }
    }

    public override LibraryContext CreateContext(IEnumerable<ISingletonInterceptor> interceptors, bool inject)
        => new SingleStoreLibraryContext(Fixture.CreateOptions(interceptors, inject));

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
