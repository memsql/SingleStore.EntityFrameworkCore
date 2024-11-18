using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using SingleStoreConnector;
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

        public override async Task KeylessEntity_with_nav_defining_query(bool async)
        {
            // FromSql mapping. Issue #21627.
            await Assert.ThrowsAsync<SingleStoreException>(() => base.KeylessEntity_with_nav_defining_query(async));

            AssertSql(
                @"SELECT `c`.`CompanyName`, `c`.`OrderCount`, `c`.`SearchTerm`
FROM `CustomerQueryWithQueryFilter` AS `c`
WHERE `c`.`OrderCount` > 0");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
