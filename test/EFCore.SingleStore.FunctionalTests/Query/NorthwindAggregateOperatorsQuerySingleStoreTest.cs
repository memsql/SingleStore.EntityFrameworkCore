using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindAggregateOperatorsQuerySingleStoreTest : NorthwindAggregateOperatorsQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindAggregateOperatorsQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Average_over_max_subquery_is_client_eval(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Max(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Average_over_nested_subquery_is_client_eval(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Average(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

        // TODO: Implement TranslatePrimitiveCollection.
        public override async Task Contains_with_local_anonymous_type_array_closure(bool async)
        {
            // Aggregates. Issue #15937.
            // await AssertTranslationFailed(() => base.Contains_with_local_anonymous_type_array_closure(async));

            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Contains_with_local_anonymous_type_array_closure(async));

            AssertSql();
        }

        // TODO: Implement TranslatePrimitiveCollection
        public override async Task Contains_with_local_tuple_array_closure(bool async)
        {
            // await AssertTranslationFailed(() => base.Contains_with_local_tuple_array_closure(async));

            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Contains_with_local_tuple_array_closure(async));
        }

        public override async Task Contains_with_local_enumerable_inline(bool async)
        {
            // Issue #31776
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await base.Contains_with_local_enumerable_inline(async));

            AssertSql();
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

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Average_over_subquery_is_client_eval(bool async)
        {
            return base.Average_over_max_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Max_over_nested_subquery_is_client_eval(bool async)
        {
            return base.Max_over_nested_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Max_over_subquery_is_client_eval(bool async)
        {
            return base.Max_over_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Max_over_sum_subquery_is_client_eval(bool async)
        {
            return base.Max_over_sum_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Min_over_max_subquery_is_client_eval(bool async)
        {
            return base.Min_over_max_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Min_over_nested_subquery_is_client_eval(bool async)
        {
            return base.Min_over_nested_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Min_over_subquery_is_client_eval(bool async)
        {
            return base.Min_over_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Sum_on_float_column_in_subquery(bool async)
        {
            return base.Sum_on_float_column_in_subquery(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Sum_over_min_subquery_is_client_eval(bool async)
        {
            return base.Sum_over_min_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Sum_over_nested_subquery_is_client_eval(bool async)
        {
            return base.Sum_over_nested_subquery_is_client_eval(async);
        }

        [ConditionalTheory(Skip = "Feature 'Subselect in aggregate functions' is not supported by SingleStore")]
        public override Task Sum_over_subquery_is_client_eval(bool async)
        {
            return base.Sum_over_subquery_is_client_eval(async);
        }

        public override async Task Contains_inside_Average_without_GroupBy(bool async)
        {
            var cities = new[] { "London", "Berlin" };

            await AssertAverage(
                async,
                ss => ss.Set<Customer>(),
                selector: c => cities.Contains(c.City) ? 1.0 : 0.0,
                asserter: (e, a) => Assert.Equal(e, a, 0.00001)); // expected: 0.076923076923076927, MySQL actual: 0.076920000000000002

            AssertSql(
                """
                SELECT AVG(CASE
                    WHEN `c`.`City` IN ('London', 'Berlin') THEN 1.0
                    ELSE 0.0
                END)
                FROM `Customers` AS `c`
                """);
        }

        protected override bool CanExecuteQueryString
            => true;

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
