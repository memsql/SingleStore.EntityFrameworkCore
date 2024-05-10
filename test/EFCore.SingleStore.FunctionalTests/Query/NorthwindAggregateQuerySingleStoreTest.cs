using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
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

        [ConditionalTheory]
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
