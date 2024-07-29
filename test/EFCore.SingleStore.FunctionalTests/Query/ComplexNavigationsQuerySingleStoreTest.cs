using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class ComplexNavigationsQuerySingleStoreTest : ComplexNavigationsQueryRelationalTestBase<ComplexNavigationsQuerySingleStoreFixture>
    {
        public ComplexNavigationsQuerySingleStoreTest(ComplexNavigationsQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

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

        [ConditionalTheory(Skip = "SingleStore has no implicit ordering of results by primary key")]
        public override Task Distinct_skip_without_orderby(bool async)
        {
            return base.Distinct_skip_without_orderby(async);
        }

        [ConditionalTheory(Skip = "SingleStore has no implicit ordering of results by primary key")]
        public override Task Distinct_take_without_orderby(bool async)
        {
            return base.Distinct_take_without_orderby(async);
        }

        [ConditionalFact(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
        public override void Member_pushdown_chain_3_levels_deep()
        {
            base.Member_pushdown_chain_3_levels_deep();
        }

        [ConditionalFact(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
        public override void Member_pushdown_with_collection_navigation_in_the_middle()
        {
            base.Member_pushdown_with_collection_navigation_in_the_middle();
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

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task SelectMany_subquery_with_custom_projection(bool async)
        {
            return base.SelectMany_subquery_with_custom_projection(async);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task Sum_with_filter_with_include_selector_cast_using_as(bool async)
        {
            return base.Sum_with_filter_with_include_selector_cast_using_as(async);
        }

        public override async Task GroupJoin_client_method_in_OrderBy(bool async)
        {
            await AssertTranslationFailedWithDetails(
                () => base.GroupJoin_client_method_in_OrderBy(async),
                CoreStrings.QueryUnableToTranslateMethod(
                    "Microsoft.EntityFrameworkCore.Query.ComplexNavigationsQueryTestBase<EntityFrameworkCore.SingleStore.FunctionalTests.Query.ComplexNavigationsQuerySingleStoreFixture>",
                    "ClientMethodNullableInt"));

            AssertSql();
        }

        public override async Task Join_with_result_selector_returning_queryable_throws_validation_error(bool async)
        {
            // Expression cannot be used for return type. Issue #23302.
            await Assert.ThrowsAsync<ArgumentException>(
                () => base.Join_with_result_selector_returning_queryable_throws_validation_error(async));

            AssertSql();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override async Task Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(bool async)
        {
            // DefaultIfEmpty on child collection. Issue #19095.
            await Assert.ThrowsAsync<EqualException>(
                async () => await base.Nested_SelectMany_correlated_with_join_table_correctly_translated_to_apply(async));

            AssertSql(
                @"SELECT `t0`.`l1Name`, `t0`.`l2Name`, `t0`.`l3Name`
FROM `LevelOne` AS `l`
LEFT JOIN LATERAL (
    SELECT `t`.`l1Name`, `t`.`l2Name`, `t`.`l3Name`
    FROM `LevelTwo` AS `l0`
    LEFT JOIN `LevelThree` AS `l1` ON `l0`.`Id` = `l1`.`Id`
    JOIN LATERAL (
        SELECT `l`.`Name` AS `l1Name`, `l1`.`Name` AS `l2Name`, `l3`.`Name` AS `l3Name`
        FROM `LevelFour` AS `l2`
        LEFT JOIN `LevelThree` AS `l3` ON `l2`.`OneToOne_Optional_PK_Inverse4Id` = `l3`.`Id`
        WHERE `l1`.`Id` IS NOT NULL AND (`l1`.`Id` = `l2`.`OneToMany_Optional_Inverse4Id`)
    ) AS `t` ON TRUE
    WHERE `l`.`Id` = `l0`.`OneToMany_Optional_Inverse2Id`
) AS `t0` ON TRUE");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
