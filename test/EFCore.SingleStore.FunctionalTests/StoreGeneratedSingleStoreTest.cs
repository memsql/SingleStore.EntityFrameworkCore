using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class StoreGeneratedSingleStoreTest : StoreGeneratedSingleStoreTestBase<StoreGeneratedSingleStoreTest.StoreGeneratedSingleStoreFixture>
    {
        public StoreGeneratedSingleStoreTest(StoreGeneratedSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class StoreGeneratedSingleStoreFixture : StoreGeneratedSingleStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => builder
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(
                        b => b.Default(WarningBehavior.Throw)
                            .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
                            .Ignore(RelationalEventId.BoolWithDefaultWarning));

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.Entity<Gumball>(
                    b =>
                    {
                        b.Property(e => e.Id).UseSingleStoreIdentityColumn();
                        b.Property(e => e.Identity).HasMaxLength(500).HasDefaultValue("Banana Joe");
                        b.Property(e => e.IdentityReadOnlyBeforeSave).HasMaxLength(500).HasDefaultValue("Doughnut Sheriff");
                        b.Property(e => e.IdentityReadOnlyAfterSave).HasMaxLength(500).HasDefaultValue("Anton");
                        b.Property(e => e.AlwaysIdentity).HasMaxLength(500).HasDefaultValue("Banana Joe");
                        b.Property(e => e.AlwaysIdentityReadOnlyBeforeSave).HasMaxLength(500).HasDefaultValue("Doughnut Sheriff");
                        b.Property(e => e.AlwaysIdentityReadOnlyAfterSave).HasMaxLength(500).HasDefaultValue("Anton");
                        b.Property(e => e.Computed).HasMaxLength(500).HasDefaultValue("Alan");
                        b.Property(e => e.ComputedReadOnlyBeforeSave).HasMaxLength(500).HasDefaultValue("Carmen");
                        b.Property(e => e.ComputedReadOnlyAfterSave).HasMaxLength(500).HasDefaultValue("Tina Rex");
                        b.Property(e => e.AlwaysComputed).HasMaxLength(500).HasDefaultValue("Alan");
                        b.Property(e => e.AlwaysComputedReadOnlyBeforeSave).HasMaxLength(500).HasDefaultValue("Carmen");
                        b.Property(e => e.AlwaysComputedReadOnlyAfterSave).HasMaxLength(500).HasDefaultValue("Tina Rex");
                    });

                modelBuilder.Entity<Anais>(
                    b =>
                    {
                        b.Property(e => e.OnAdd).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddUseBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddIgnoreBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddThrowBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddUseBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddIgnoreBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddThrowBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddUseBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddIgnoreBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddThrowBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");

                        b.Property(e => e.OnAddOrUpdate).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateUseBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateIgnoreBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateThrowBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateUseBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateIgnoreBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateThrowBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateUseBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateIgnoreBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateThrowBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");

                        b.Property(e => e.OnUpdate).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateUseBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateIgnoreBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateThrowBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateUseBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateIgnoreBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateThrowBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateUseBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateIgnoreBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateThrowBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                    });

                modelBuilder.Entity<WithBackingFields>(
                    b =>
                    {
                        if (AppConfig.ServerVersion.Supports.GeneratedColumns)
                        {
                            b.Property(e => e.NullableAsNonNullable).HasComputedColumnSql("1");
                            b.Property(e => e.NonNullableAsNullable).HasComputedColumnSql("1");
                        }
                        else
                        {
                            b.Property(e => e.NullableAsNonNullable).HasDefaultValue(1);
                            b.Property(e => e.NonNullableAsNullable).HasDefaultValue(1);
                        }
                    });

                modelBuilder.Entity<WithNullableBackingFields>(
                    b =>
                    {
                        b.Property(e => e.NullableBackedBoolTrueDefault).HasDefaultValue(true);
                        b.Property(e => e.NullableBackedIntNonZeroDefault).HasDefaultValue(-1);
                        b.Property(e => e.NullableBackedBoolFalseDefault).HasDefaultValue(false);
                        b.Property(e => e.NullableBackedIntZeroDefault).HasDefaultValue(0);
                    });

                modelBuilder.Entity<WithObjectBackingFields>(
                    b =>
                    {
                        b.Property(e => e.NullableBackedBoolTrueDefault).HasDefaultValue(true);
                        b.Property(e => e.NullableBackedIntNonZeroDefault).HasDefaultValue(-1);
                        b.Property(e => e.NullableBackedBoolFalseDefault).HasDefaultValue(false);
                        b.Property(e => e.NullableBackedIntZeroDefault).HasDefaultValue(0);
                    });

                modelBuilder.Entity<NonStoreGenDependent>().Property(e => e.HasTemp).HasDefaultValue(777);

                // modelBuilder.Entity<CompositePrincipal>().Property(e => e.Id).UseSingleStoreIdentityColumn();

                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Anais>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Darwin>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Gumball>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<OptionalCategory>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<StoreGenPrincipal>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<WithBackingFields>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<WithNullableBackingFields>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<WithObjectBackingFields>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Species>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<OptionalProduct>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");

                base.OnModelCreating(modelBuilder, context);
            }
        }
    }
}
