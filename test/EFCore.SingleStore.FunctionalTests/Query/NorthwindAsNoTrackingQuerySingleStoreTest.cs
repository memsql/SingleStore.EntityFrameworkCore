using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindAsNoTrackingQuerySingleStoreTest : NorthwindAsNoTrackingQueryTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindAsNoTrackingQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
