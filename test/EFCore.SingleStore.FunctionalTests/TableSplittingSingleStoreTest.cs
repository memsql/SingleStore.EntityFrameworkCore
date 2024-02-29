using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.TransportationModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
    public class TableSplittingSingleStoreTest : TableSplittingTestBase
    {
        public TableSplittingSingleStoreTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Engine>().ToTable("Vehicles")
                .Property(e => e.Computed).HasComputedColumnSql("1", stored: true);
        }

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
