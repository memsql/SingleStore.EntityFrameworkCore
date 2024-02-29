using System.Linq;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests.
    internal class GraphUpdatesSingleStoreTest
    {
        public class ClientCascade : GraphUpdatesSingleStoreTestBase<ClientCascade.SingleStoreFixture>
        {
            public ClientCascade(SingleStoreFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public class SingleStoreFixture : GraphUpdatesSingleStoreFixtureBase
            {
                public override bool NoStoreCascades
                    => true;

                protected override string StoreName { get; } = "GraphClientCascadeUpdatesTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    base.OnModelCreating(modelBuilder, context);

                    foreach (var foreignKey in modelBuilder.Model
                        .GetEntityTypes()
                        .SelectMany(e => e.GetDeclaredForeignKeys())
                        .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade))
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
                    }
                }
            }
        }

        public class ClientNoAction : GraphUpdatesSingleStoreTestBase<ClientNoAction.SingleStoreFixture>
        {
            public ClientNoAction(SingleStoreFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public class SingleStoreFixture : GraphUpdatesSingleStoreFixtureBase
            {
                public override bool ForceClientNoAction
                    => true;

                protected override string StoreName { get; } = "GraphClientNoActionUpdatesTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    base.OnModelCreating(modelBuilder, context);

                    foreach (var foreignKey in modelBuilder.Model
                        .GetEntityTypes()
                        .SelectMany(e => e.GetDeclaredForeignKeys()))
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.ClientNoAction;
                    }
                }
            }
        }

        // TODO: UseIdentityColumns()
        // public class Identity : GraphUpdatesSingleStoreTestBase<Identity.SingleStoreFixture>
        // {
        //     public Identity(SingleStoreFixture fixture)
        //         : base(fixture)
        //     {
        //     }
        //
        //     protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        //         => facade.UseTransaction(transaction.GetDbTransaction());
        //
        //     public class SingleStoreFixture : GraphUpdatesSingleStoreFixtureBase
        //     {
        //         protected override string StoreName { get; } = "GraphIdentityUpdatesTest";
        //
        //         protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        //         {
        //             modelBuilder.UseIdentityColumns();
        //
        //             base.OnModelCreating(modelBuilder, context);
        //         }
        //     }
        // }

        public abstract class GraphUpdatesSingleStoreTestBase<TFixture> : GraphUpdatesTestBase<TFixture>
            where TFixture : GraphUpdatesSingleStoreTestBase<TFixture>.GraphUpdatesSingleStoreFixtureBase, new()
        {
            protected GraphUpdatesSingleStoreTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override IQueryable<Root> ModifyQueryRoot(IQueryable<Root> query)
                => query.AsSplitQuery();

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class GraphUpdatesSingleStoreFixtureBase : GraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory
                    => (TestSqlLoggerFactory)ListLoggerFactory;

                protected override ITestStoreFactory TestStoreFactory
                    => SingleStoreTestStoreFactory.Instance;

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    base.OnModelCreating(modelBuilder, context);

                    modelBuilder.Entity<AccessState>(
                        b =>
                        {
                            b.Property(e => e.AccessStateId).ValueGeneratedNever();
                            b.HasData(new AccessState {AccessStateId = 1});
                        });

                    modelBuilder.Entity<Cruiser>(
                        b =>
                        {
                            b.Property(e => e.IdUserState).HasDefaultValue(1);
                            b.HasOne(e => e.UserState).WithMany(e => e.Users).HasForeignKey(e => e.IdUserState);
                        });
                }
            }
        }
    }
}
