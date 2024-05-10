using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FunkyDataQuerySingleStoreTest : FunkyDataQueryTestBase<FunkyDataQuerySingleStoreTest.FunkyDataQuerySingleStoreFixture>
    {
        public FunkyDataQuerySingleStoreTest(FunkyDataQuerySingleStoreFixture fixture)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class FunkyDataQuerySingleStoreFixture : FunkyDataQueryFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;
        }
    }
}
