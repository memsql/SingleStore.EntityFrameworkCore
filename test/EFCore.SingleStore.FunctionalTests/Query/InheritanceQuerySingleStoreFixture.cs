using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class InheritanceQuerySingleStoreFixture : InheritanceQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

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

#pragma warning disable CS0618 // Type or member is obsolete
            modelBuilder.Entity<AnimalQuery>()
                .HasNoKey()
                .ToQuery(
                    () => context.Set<AnimalQuery>()
                        .FromSqlRaw(@"SELECT * FROM `Animals`"));
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
