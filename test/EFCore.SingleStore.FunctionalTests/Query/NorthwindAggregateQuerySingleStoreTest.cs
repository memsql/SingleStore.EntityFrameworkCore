using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindAggregateQuerySingleStoreTest : NorthwindAggregateOperatorsQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindAggregateQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Collection_Last_member_access_in_projection_translated(bool async)
        {
            return base.Collection_Last_member_access_in_projection_translated(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Collection_LastOrDefault_member_access_in_projection_translated(bool async)
        {
            return base.Collection_LastOrDefault_member_access_in_projection_translated(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task First_inside_subquery_gets_client_evaluated(bool async)
        {
            return base.First_inside_subquery_gets_client_evaluated(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task FirstOrDefault_inside_subquery_gets_server_evaluated(bool async)
        {
            return base.FirstOrDefault_inside_subquery_gets_server_evaluated(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Multiple_collection_navigation_with_FirstOrDefault_chained_projecting_scalar(bool async)
        {
            return base.Multiple_collection_navigation_with_FirstOrDefault_chained_projecting_scalar(async);
        }

        public override async Task Average_over_max_subquery_is_client_eval(bool async)
        {
            await AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Max(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

            AssertSql(
                $@"@__p_0='3'
SELECT AVG(CAST((
    SELECT AVG({SingleStoreTestHelpers.CastAsDouble(@"5 + (
        SELECT MAX(`o0`.`ProductID`)
        FROM `Order Details` AS `o0`
        WHERE `o`.`OrderID` = `o0`.`OrderID`)")})
    FROM `Orders` AS `o`
    WHERE `t`.`CustomerID` = `o`.`CustomerID`) AS decimal(65,30)))
FROM (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    ORDER BY `c`.`CustomerID`
    LIMIT @__p_0
) AS `t`");
        }

        public override async Task Average_over_nested_subquery_is_client_eval(bool async)
        {
            await AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Average(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

            AssertSql(
                $@"@__p_0='3'
SELECT AVG(CAST((
    SELECT AVG(5.0 + (
        SELECT AVG({SingleStoreTestHelpers.CastAsDouble(@"`o0`.`ProductID`")})
        FROM `Order Details` AS `o0`
        WHERE `o`.`OrderID` = `o0`.`OrderID`))
    FROM `Orders` AS `o`
    WHERE `t`.`CustomerID` = `o`.`CustomerID`) AS decimal(65,30)))
FROM (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    ORDER BY `c`.`CustomerID`
    LIMIT @__p_0
) AS `t`");
        }

        public override async Task Contains_with_local_anonymous_type_array_closure(bool async)
        {
            // Aggregates. Issue #15937.
            await AssertTranslationFailed(() => base.Contains_with_local_anonymous_type_array_closure(async));

            AssertSql();
        }

        public override async Task Contains_with_local_tuple_array_closure(bool async)
            => await AssertTranslationFailed(() => base.Contains_with_local_tuple_array_closure(async));

        public override async Task Sum_with_coalesce(bool async)
        {
            await base.Sum_with_coalesce(async);

            AssertSql(
                @"SELECT COALESCE(SUM(COALESCE(`p`.`UnitPrice`, 0.0)), 0.0)
FROM `Products` AS `p`
WHERE `p`.`ProductID` < 40");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
