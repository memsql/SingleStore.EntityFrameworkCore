using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Collections.Generic;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class PropertyValuesSingleStoreTest : PropertyValuesTestBase<PropertyValuesSingleStoreTest.PropertyValuesSingleStoreFixture>
    {
        public PropertyValuesSingleStoreTest(PropertyValuesSingleStoreFixture fixture)
            : base(fixture)
        {
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

                base.OnModelCreating(modelBuilder, context);
            }
        }
    }
}
