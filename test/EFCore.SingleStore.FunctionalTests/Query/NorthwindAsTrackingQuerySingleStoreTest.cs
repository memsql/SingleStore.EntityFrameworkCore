using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindAsTrackingQuerySingleStoreTest : NorthwindAsTrackingQueryTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindAsTrackingQuerySingleStoreTest(NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
