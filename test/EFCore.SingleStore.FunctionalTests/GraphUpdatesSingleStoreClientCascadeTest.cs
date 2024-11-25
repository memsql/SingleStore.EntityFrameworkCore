using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

// Made internal to skip all tests.
internal class GraphUpdatesSingleStoreClientCascadeTest : GraphUpdatesSingleStoreTestBase<GraphUpdatesSingleStoreClientCascadeTest.SingleStoreFixture>
{
    public GraphUpdatesSingleStoreClientCascadeTest(SingleStoreFixture fixture)
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
