using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class FindSingleStoreTest : FindTestBase<FindSingleStoreTest.FindSingleStoreFixture>
    {
        public FindSingleStoreTest(FindSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
            => context.Set<TEntity>().Find(keyValues);

        protected override ValueTask<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
            => context.Set<TEntity>().FindAsync(keyValues);

        public class FindSingleStoreFixture : FindFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
        }
    }
}
