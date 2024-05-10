using System.Linq;
using Microsoft.EntityFrameworkCore;
using SingleStoreConnector;
using Xunit;

namespace EntityFrameworkCore.SingleStore.Query
{
    public sealed class SingleStoreCollationTest : TestWithFixture<SingleStoreCollationTest.SingleStoreCollationFixture>
    {
        public SingleStoreCollationTest(SingleStoreCollationFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact(Skip = "Works in SingleStore.")]
        public void Where_with_incompatible_collations_fails()
        {
            using var context = Fixture.CreateContext();

            SetConnectionCharSetAndCollation(context);

            var connection = (SingleStoreConnection)context.Database.GetDbConnection();
            var csb = new SingleStoreConnectionStringBuilder(connection.ConnectionString);

            Assert.Throws<SingleStoreException>(
                () => context.Set<Model.Container>()
                    .Where(e => EF.Functions.Like(e.Name + e.Number, "%Metal%"))
                    .ToList());
        }

        [ConditionalFact(Skip = "SingleStore does not support functionality needed for EF.Functions.Collate.")]
        public void Where_with_incompatible_collations_succeeds_with_explicit_collation_case_sensitive()
        {
            using var context = Fixture.CreateContext();

            SetConnectionCharSetAndCollation(context);

            var metalContainers = context.Set<Model.Container>()
                .Where(e => EF.Functions.Like(EF.Functions.Collate(e.Name, "latin1_general_cs") + e.Number, "%Metal%"))
                .ToList();

            Assert.Single(metalContainers);
            Assert.Equal(3, metalContainers[0].Id);
        }

        [ConditionalFact(Skip = "SingleStore does not support functionality needed for EF.Functions.Collate.")]
        public void Where_with_incompatible_collations_succeeds_with_explicit_collation_case_insensitive()
        {
            using var context = Fixture.CreateContext();

            SetConnectionCharSetAndCollation(context);

            var metalContainers = context.Set<Model.Container>()
                .Where(e => EF.Functions.Like(EF.Functions.Collate(e.Name, "latin1_general_ci") + e.Number, "%Metal%"))
                .OrderBy(e => e.Id)
                .ToList();

            Assert.Equal(2, metalContainers.Count);
            Assert.Equal(1, metalContainers[0].Id);
            Assert.Equal(3, metalContainers[1].Id);
        }

        private static void SetConnectionCharSetAndCollation(SingleStoreCollationFixture.SingleStoreCollationContext context)
        {
            context.Database.OpenConnection();
            var connection = context.Database.GetDbConnection();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SET NAMES 'latin1' COLLATE 'latin1_general_ci';";
                command.ExecuteNonQuery();
            }
        }

        public class SingleStoreCollationFixture : SingleStoreTestFixtureBase<SingleStoreCollationFixture.SingleStoreCollationContext>
        {
            public class SingleStoreCollationContext : ContextBase
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Model.Container>(
                        entity =>
                        {
                            entity.Property(e => e.Name)
                                .UseCollation("latin1_general_cs");

                            entity.HasData(
                                new Model.Container { Id = 1, Name = "Heavymetal", Number = 10},
                                new Model.Container { Id = 2, Name = "Plastic", Number = 20},
                                new Model.Container { Id = 3, Name = "Plastic-Metal-Compound", Number = 30});
                        });
                }
            }
        }

        private static class Model
        {
            public class Container
            {
                public int Id { get ; set; }
                public string Name { get ; set; }
                public int Number { get; set; }
            }
        }
    }
}
