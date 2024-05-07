using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    // Made internal to skip all tests.
    internal class ManyToManyQuerySingleStoreTest : ManyToManyQueryRelationalTestBase<ManyToManyQuerySingleStoreFixture>
    {
        public ManyToManyQuerySingleStoreTest(ManyToManyQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        protected override bool CanExecuteQueryString
            => true;
    }
}
