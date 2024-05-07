using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindKeylessEntitiesQuerySingleStoreTest : NorthwindKeylessEntitiesQueryRelationalTestBase<NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindKeylessEntitiesQuerySingleStoreTest(NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [ConditionalFact(Skip = "https://github.com/dotnet/efcore/issues/21627")]
        public override void KeylessEntity_with_nav_defining_query()
            => base.KeylessEntity_with_nav_defining_query();
    }
}
