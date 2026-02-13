using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class MaterializationInterceptionSingleStoreTest : MaterializationInterceptionTestBase<MaterializationInterceptionSingleStoreTest.SingleStoreLibraryContext>
{
    [ConditionalTheory]
    public override async Task Intercept_query_materialization_with_owned_types_projecting_collection(bool async, bool usePooling)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        await base.Intercept_query_materialization_with_owned_types_projecting_collection(async, usePooling);
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

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
