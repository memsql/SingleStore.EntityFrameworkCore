using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FiltersInheritanceQuerySingleStoreFixture : InheritanceQuerySingleStoreFixture
    {
        protected override bool EnableFilters => true;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Country>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Drink>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Animal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        }
    }
}
