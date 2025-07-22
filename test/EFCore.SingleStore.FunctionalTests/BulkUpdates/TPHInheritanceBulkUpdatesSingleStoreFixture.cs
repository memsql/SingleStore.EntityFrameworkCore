using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.BulkUpdates;

public class TPHInheritanceBulkUpdatesSingleStoreFixture : TPHInheritanceBulkUpdatesFixture
{
    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
        // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
        modelBuilder.Entity<Animal>()
            .Property(e => e.Id)
            .HasColumnType("bigint");

        modelBuilder.Entity<Country>()
            .Property(e => e.Id)
            .HasColumnType("bigint");

        modelBuilder.Entity<Drink>()
            .Property(e => e.Id)
            .HasColumnType("bigint");

        base.OnModelCreating(modelBuilder, context);
    }
}
