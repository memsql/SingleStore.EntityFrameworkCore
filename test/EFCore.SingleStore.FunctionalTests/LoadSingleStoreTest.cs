using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class LoadSingleStoreTest : LoadTestBase<LoadSingleStoreTest.LoadSingleStoreFixture>
    {
        public LoadSingleStoreTest(LoadSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public override void Lazy_loading_uses_field_access_when_abstract_base_class_navigation()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            base.Lazy_loading_uses_field_access_when_abstract_base_class_navigation();
        }

        public class LoadSingleStoreFixture : LoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base.AddOptions(builder);

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<RootClass>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            }
        }
    }
}
