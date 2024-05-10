using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    // Made internal to skip all tests.
    internal class TPTManyToManyQuerySingleStoreTest : TPTManyToManyQueryRelationalTestBase<TPTManyToManyQuerySingleStoreFixture>
    {
        public TPTManyToManyQuerySingleStoreTest(TPTManyToManyQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;
    }
}
