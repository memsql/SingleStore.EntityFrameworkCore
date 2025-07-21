using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
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
            Expression<Func<Customer, bool>> expectedPredicate;
            if (AppConfig.ServerVersion.Version.Major >= 9)
            {
                // starting from 9.0 SingleStore's standard collation is utf8mb4_bin
                // it is case‑sensitive, so only uppercase M will match the LIKE pattern
                expectedPredicate = c => c.ContactName.Contains("M");
            }
            else
            {
                // older collations are case‑insensitive
                expectedPredicate = c => c.ContactName.Contains("M") || c.ContactName.Contains("m");
            }

            await AssertCount(
                async,
                ss => ss.Set<Customer>(),
                ss => ss.Set<Customer>(),
                c => EF.Functions.Like(c.ContactName, "%M%"),
                expectedPredicate);

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
