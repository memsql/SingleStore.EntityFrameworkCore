using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindQueryFiltersQuerySingleStoreTest : NorthwindQueryFiltersQueryTestBase<
        NorthwindQuerySingleStoreFixture<NorthwindQueryFiltersCustomizer>>
    {
        public NorthwindQueryFiltersQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NorthwindQueryFiltersCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Count_query(bool async)
        {
            await base.Count_query(async);


        AssertSql(
"""
@__ef_filter__TenantPrefix_0_rewritten='B%' (Size = 40)

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CompanyName` LIKE @__ef_filter__TenantPrefix_0_rewritten
""");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
