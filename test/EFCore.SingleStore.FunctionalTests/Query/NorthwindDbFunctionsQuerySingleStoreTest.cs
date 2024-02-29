using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public partial class NorthwindDbFunctionsQuerySingleStoreTest : NorthwindDbFunctionsQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindDbFunctionsQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Like_literal(bool async)
        {
            await base.Like_literal(async);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '%M%'");
        }

        public override async Task Like_identity(bool async)
        {
            await base.Like_identity(async);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE `c`.`ContactName`");
        }

        [ConditionalTheory(Skip = "Operator LIKE does not support the standard ESCAPE clause in SingleStore")]
        public override async Task Like_literal_with_escape(bool async)
        {
            await base.Like_literal_with_escape(async);

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '!%' ESCAPE '!'");
        }

        [ConditionalTheory(Skip = "Operator LIKE does not support the standard ESCAPE clause in SingleStore")]
        public override Task Like_all_literals_with_escape(bool async)
        {
            return base.Like_all_literals_with_escape(async);
        }

        protected override string CaseInsensitiveCollation
            => "utf8mb4_general_ci";

        protected override string CaseSensitiveCollation
            => "utf8mb4_bin";

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
