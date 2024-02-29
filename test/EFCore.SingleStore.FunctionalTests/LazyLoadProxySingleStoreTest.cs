using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class LazyLoadProxySingleStoreTest : LazyLoadProxyTestBase<LazyLoadProxySingleStoreTest.LoadSingleStoreFixture>
    {
        public LazyLoadProxySingleStoreTest(LoadSingleStoreFixture fixture)
            : base(fixture)
        {
            ClearLog();
        }

        public override void Top_level_projection_track_entities_before_passing_to_client_method()
        {
            base.Top_level_projection_track_entities_before_passing_to_client_method();
            RecordLog();

            Assert.Equal(
                @"SELECT `p`.`Id`, `p`.`AlternateId`, `p`.`Discriminator`
FROM `Parent` AS `p`
ORDER BY `p`.`Id`
LIMIT 1

@__p_0='707' (Nullable = true)

SELECT `s`.`Id`, `s`.`ParentId`
FROM `Single` AS `s`
WHERE `s`.`ParentId` = @__p_0",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        protected override void RecordLog() =>
            Sql = Fixture.TestSqlLoggerFactory.Sql;

        private string Sql { get; set; }

        public class LoadSingleStoreFixture : LoadFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Applicant>()
                    .Property(e => e.ApplicantId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Blog>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Entity>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Pyrson>()
                    .Property(e => e.PyrsonId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Nose>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Address>()
                    .Property(e => e.AddressId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<NonVirtualOneToManyOwner>()
                    .OwnsMany(o => o.Addresses, a =>
                    {
                        a.Property<long>("Id").HasColumnName("Id").HasColumnType("bigint");
                    });
                modelBuilder.Entity<VirtualOneToManyOwner>()
                    .OwnsMany(o => o.Addresses, a =>
                    {
                        a.Property<long>("Id").HasColumnName("Id").HasColumnType("bigint");
                    });
                modelBuilder.Entity<ExplicitLazyLoadNonVirtualOneToManyOwner>()
                    .OwnsMany(o => o.Addresses, a =>
                    {
                        a.Property<long>("Id").HasColumnName("Id").HasColumnType("bigint");
                    });
                modelBuilder.Entity<ExplicitLazyLoadVirtualOneToManyOwner>()
                    .OwnsMany(o => o.Addresses, a =>
                    {
                        a.Property<long>("Id").HasColumnName("Id").HasColumnType("bigint");
                    });
            }
        }
    }
}
