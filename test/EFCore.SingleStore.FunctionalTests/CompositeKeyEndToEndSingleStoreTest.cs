using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{

    public class CompositeKeyEndToEndSingleStoreTest : CompositeKeyEndToEndTestBase<CompositeKeyEndToEndSingleStoreTest.CompositeKeyEndToEndSingleStoreFixture>
    {
        public CompositeKeyEndToEndSingleStoreTest(CompositeKeyEndToEndSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public override async Task Can_use_generated_values_in_composite_key_end_to_end()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.Can_use_generated_values_in_composite_key_end_to_end();
        }

        public class CompositeKeyEndToEndSingleStoreFixture : CompositeKeyEndToEndFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Unicorn>()
                    .Property(e => e.Id1)
                    .HasColumnType("bigint");
            }
        }
    }
}
