using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class ComplexNavigationsSharedTypeQuerySingleStoreTest : ComplexNavigationsSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQuerySingleStoreTest.ComplexNavigationsSharedTypeQuerySingleStoreFixture>
    {
        // ReSharper disable once UnusedParameter.Local
        public ComplexNavigationsSharedTypeQuerySingleStoreTest(
            ComplexNavigationsSharedTypeQuerySingleStoreFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            // Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "Feature 'Scalar subselect where outer table is not a sharded table' is not supported by SingleStore")]
        public override Task Composite_key_join_on_groupby_aggregate_projecting_only_grouping_key(bool async)
        {
            return base.Composite_key_join_on_groupby_aggregate_projecting_only_grouping_key(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
        }

        public override Task Distinct_take_without_orderby(bool async)
        {
            return AssertQuery(
                async,
                ss => from l1 in ss.Set<Level1>()
                    where l1.Id < 3
                    select (from l3 in ss.Set<Level3>()
                        select l3).Distinct().OrderBy(e => e.Id).Take(1).FirstOrDefault().Name); // Apply OrderBy before Skip
        }

        public override Task Distinct_skip_without_orderby(bool async)
        {
            return AssertQuery(
                async,
                ss => from l1 in ss.Set<Level1>()
                    where l1.Id < 3
                    select (from l3 in ss.Set<Level3>()
                        select l3).Distinct().OrderBy(e => e.Id).Skip(1).FirstOrDefault().Name); // Apply OrderBy before Skip
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect inside HAVING")]
        public override Task Element_selector_with_coalesce_repeated_in_aggregate(bool async)
        {
            return base.Element_selector_with_coalesce_repeated_in_aggregate(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Collection_FirstOrDefault_property_accesses_in_projection(bool async)
        {
            return base.Collection_FirstOrDefault_property_accesses_in_projection(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Contains_over_optional_navigation_with_null_column(bool async)
        {
            return base.Contains_over_optional_navigation_with_null_column(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Contains_over_optional_navigation_with_null_entity_reference(bool async)
        {
            return base.Contains_over_optional_navigation_with_null_entity_reference(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Member_pushdown_with_multiple_collections(bool async)
        {
            return base.Member_pushdown_with_multiple_collections(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Multiple_collection_FirstOrDefault_followed_by_member_access_in_projection(bool async)
        {
            return base.Multiple_collection_FirstOrDefault_followed_by_member_access_in_projection(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task OrderBy_collection_count_ThenBy_reference_navigation(bool async)
        {
            return base.OrderBy_collection_count_ThenBy_reference_navigation(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Project_collection_navigation_count(bool async)
        {
            return base.Project_collection_navigation_count(async);
        }

        [ConditionalTheory(Skip = "SingleStore has no implicit ordering of results by primary key")]
        public override Task Subquery_with_Distinct_Skip_FirstOrDefault_without_OrderBy(bool async)
        {
            return base.Subquery_with_Distinct_Skip_FirstOrDefault_without_OrderBy(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_navigation_property_to_collection(bool async)
        {
            return base.Where_navigation_property_to_collection(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_navigation_property_to_collection2(bool async)
        {
            return base.Where_navigation_property_to_collection2(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_navigation_property_to_collection_of_original_entity_type(bool async)
        {
            return base.Where_navigation_property_to_collection_of_original_entity_type(async);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task SelectMany_subquery_with_custom_projection(bool async)
        {
            return base.SelectMany_subquery_with_custom_projection(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect inside HAVING")]
        public override Task Simple_level1_level2_GroupBy_Having_Count(bool async)
        {
            return base.Simple_level1_level2_GroupBy_Having_Count(async);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task Sum_with_filter_with_include_selector_cast_using_as(bool async)
        {
            return base.Sum_with_filter_with_include_selector_cast_using_as(async);
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/26104")]
        public override Task GroupBy_aggregate_where_required_relationship(bool async)
            => base.GroupBy_aggregate_where_required_relationship(async);

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/26104")]
        public override Task GroupBy_aggregate_where_required_relationship_2(bool async)
            => base.GroupBy_aggregate_where_required_relationship_2(async);

        public override async Task GroupJoin_client_method_in_OrderBy(bool async)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await base.GroupJoin_client_method_in_OrderBy(async));

            AssertSql();
        }

        public override async Task Join_with_result_selector_returning_queryable_throws_validation_error(bool async)
        {
            // Expression cannot be used for return type. Issue #23302.
            await Assert.ThrowsAsync<ArgumentException>(
                () => base.Join_with_result_selector_returning_queryable_throws_validation_error(async));

            AssertSql();
        }

        [ConditionalTheory(Skip = "Does not throw an EqualException, but still does not work.")]
        public override async Task Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(bool async)
        {
            // DefaultIfEmpty on child collection. Issue #19095.
            await Assert.ThrowsAsync<EqualException>(
                async () => await base.Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(async));

            AssertSql();
        }

        public override async Task Method_call_on_optional_navigation_translates_to_null_conditional_properly_for_arguments(bool async)
        {
            await base.Method_call_on_optional_navigation_translates_to_null_conditional_properly_for_arguments(async);

            AssertSql(
                """
                SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`
                FROM `Level1` AS `l`
                LEFT JOIN (
                    SELECT `l0`.`Level1_Optional_Id`, `l0`.`Level2_Name`
                    FROM `Level1` AS `l0`
                    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
                ) AS `t` ON `l`.`Id` = `t`.`Level1_Optional_Id`
                WHERE `t`.`Level2_Name` IS NOT NULL AND (LEFT(`t`.`Level2_Name`, CHAR_LENGTH(`t`.`Level2_Name`)) = `t`.`Level2_Name`)
                """);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class ComplexNavigationsSharedTypeQuerySingleStoreFixture : ComplexNavigationsSharedTypeQueryRelationalFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;
        }
    }
}
