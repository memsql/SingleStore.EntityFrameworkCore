using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindNavigationsQuerySingleStoreTest : NorthwindNavigationsQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindNavigationsQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Collection_orderby_nav_prop_count(bool async)
        {
            return base.Collection_orderby_nav_prop_count(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Collection_select_nav_prop_all(bool async)
        {
            return base.Collection_select_nav_prop_all(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Navigation_in_subquery_referencing_outer_query(bool async)
        {
            return base.Navigation_in_subquery_referencing_outer_query(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Navigation_with_collection_with_nullable_type_key(bool async)
        {
            return base.Navigation_with_collection_with_nullable_type_key(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Project_single_scalar_value_subquery_in_query_with_optional_navigation_works(bool async)
        {
            return base.Project_single_scalar_value_subquery_in_query_with_optional_navigation_works(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Project_single_scalar_value_subquery_is_properly_inlined(bool async)
        {
            return base.Project_single_scalar_value_subquery_is_properly_inlined(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_collection_FirstOrDefault_project_single_column1(bool async)
        {
            return base.Select_collection_FirstOrDefault_project_single_column1(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_collection_FirstOrDefault_project_single_column2(bool async)
        {
            return base.Select_collection_FirstOrDefault_project_single_column2(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Collection_select_nav_prop_sum(bool async)
        {
            return base.Collection_select_nav_prop_sum(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Collection_select_nav_prop_sum_plus_one(bool async)
        {
            return base.Collection_select_nav_prop_sum_plus_one(async);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        public override Task Where_subquery_on_navigation(bool async)
        {
            return base.Where_subquery_on_navigation(async);
        }

        [ConditionalTheory(Skip = "Issue #573")]
        public override Task Where_subquery_on_navigation2(bool async)
        {
            return base.Where_subquery_on_navigation2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count(bool async)
        {
            return base.Navigation_in_subquery_referencing_outer_query_with_client_side_result_operator_and_count(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
