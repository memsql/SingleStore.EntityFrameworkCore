using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class GearsOfWarQuerySingleStoreFixture : GearsOfWarQueryRelationalFixture, IQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);

            new SingleStoreDbContextOptionsBuilder(optionsBuilder)
                .EnableIndexOptimizedBooleanColumns(true);

            return optionsBuilder;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<Weapon>().HasIndex(e => e.IsAutomatic);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<LocustHighCommand>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Mission>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Squad>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Weapon>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Faction>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        }

        public new ISetSource GetExpectedData()
        {
            var data = (GearsOfWarData)base.GetExpectedData();

            foreach (var mission in data.Missions)
            {
                mission.Timeline = SingleStoreTestHelpers.GetExpectedValue(mission.Timeline);
            }

            return data;
        }
    }
}
