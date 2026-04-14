using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class PropertyValuesSingleStoreTest : PropertyValuesTestBase<PropertyValuesSingleStoreTest.PropertyValuesSingleStoreFixture>
    {
        public PropertyValuesSingleStoreTest(PropertyValuesSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public override async Task Values_can_be_reloaded_from_database_for_entity_in_any_state_with_inheritance(EntityState state,
            bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            await base.Values_can_be_reloaded_from_database_for_entity_in_any_state_with_inheritance(state, async);
        }

        public class PropertyValuesSingleStoreFixture : PropertyValuesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<VirtualTeam>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Contact33307>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");

                base.OnModelCreating(modelBuilder, context);
            }
        }
    }
}
