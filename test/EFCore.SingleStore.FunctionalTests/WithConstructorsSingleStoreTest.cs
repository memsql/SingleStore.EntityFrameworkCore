using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class WithConstructorsSingleStoreTest : WithConstructorsSingleStoreTestBase<WithConstructorsSingleStoreTest.WithConstructorsSingleStoreFixture>
    {
        public WithConstructorsSingleStoreTest(WithConstructorsSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class WithConstructorsSingleStoreFixture : WithConstructorsSingleStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<BlogQuery>()
                    .HasNoKey()
                    .ToSqlQuery("select * from `Blog`");
            }
        }
    }
}
