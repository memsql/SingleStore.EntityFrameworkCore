using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class NorthwindQueryTaggingQuerySingleStoreTest : NorthwindQueryTaggingQueryTestBase<NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindQueryTaggingQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
