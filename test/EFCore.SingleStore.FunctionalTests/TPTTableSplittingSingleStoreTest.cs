using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class TPTTableSplittingSingleStoreTest : TPTTableSplittingTestBase
    {
        public TPTTableSplittingSingleStoreTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        protected override void OnSharedModelCreating(ModelBuilder modelBuilder)
        {
            base.OnSharedModelCreating(modelBuilder);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<MeterReading>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        }
    }
}
