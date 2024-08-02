using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public partial class GearsOfWarQuerySingleStoreTest : GearsOfWarQueryRelationalTestBase<GearsOfWarQuerySingleStoreFixture>
    {
        public GearsOfWarQuerySingleStoreTest(GearsOfWarQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        public override Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
        {
            var dto = SingleStoreTestHelpers.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));
            var start = dto.AddDays(-1);
            var end = dto.AddDays(1);
            var dates = new[] { dto };

            return AssertQuery(
                async,
                ss => ss.Set<Mission>().Where(
                    m => start <= m.Timeline.Date && m.Timeline < end && dates.Contains(m.Timeline)));
        }

        public override Task Where_datetimeoffset_milliseconds_parameter_and_constant(bool async)
        {
            var dateTimeOffset = SingleStoreTestHelpers.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));

            // Literal where clause
            var p = Expression.Parameter(typeof(Mission), "i");
            var dynamicWhere = Expression.Lambda<Func<Mission, bool>>(
                Expression.Equal(
                    Expression.Property(p, "Timeline"),
                    Expression.Constant(dateTimeOffset)
                ), p);

            return AssertCount(
                async,
                ss => ss.Set<Mission>().Where(dynamicWhere),
                ss => ss.Set<Mission>().Where(m => m.Timeline == dateTimeOffset));
        }

        [ConditionalTheory(Skip = "TODO: Does not work as expected, probably due to some test definition issues.")]
        public override async Task DateTimeOffsetNow_minus_timespan(bool async)
        {
            var timeSpan = new TimeSpan(10000); // <-- changed from 1000 to 10000 ticks

            await AssertQuery(
                async,
                ss => ss.Set<Mission>().Where(e => e.Timeline > DateTimeOffset.Now - timeSpan));

            AssertSql(
                """
                @__timeSpan_0='00:00:00.0010000' (DbType = DateTimeOffset)

                SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
                FROM `Missions` AS `m`
                WHERE `m`.`Timeline` > (UTC_TIMESTAMP() - @__timeSpan_0)
                """);
        }

        // TODO: Implement strategy as discussed with @roji (including emails) for EF Core 5.
        [ConditionalTheory(Skip = "#996")]
        public override Task Client_member_and_unsupported_string_Equals_in_the_same_query(bool async)
        {
            return base.Client_member_and_unsupported_string_Equals_in_the_same_query(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_firstordefault(bool async)
        {
            return base.Select_subquery_distinct_firstordefault(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty1(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Where_subquery_distinct_first_boolean(bool async)
        {
            return base.Where_subquery_distinct_first_boolean(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Where_subquery_distinct_singleordefault_boolean1(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean1(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_distinct_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_distinct_lastordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_lastordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_distinct_last_boolean(bool async)
        {
            return base.Where_subquery_distinct_last_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Concat_with_collection_navigations(bool async)
        {
            return base.Concat_with_collection_navigations(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Select_navigation_with_concat_and_count(bool async)
        {
            return base.Select_navigation_with_concat_and_count(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Union_with_collection_navigations(bool async)
        {
            return base.Union_with_collection_navigations(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_concat_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_concat_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_join_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_join_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_left_join_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_left_join_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects in project list")]
        public override Task Where_subquery_union_firstordefault_boolean(bool async)
        {
            return base.Where_subquery_union_firstordefault_boolean(async);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool async)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(async);
        }

        [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
        public override Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool async)
        {
            return base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql")]
        public override Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(bool async)
        {
            return base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql")]
        public override Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(bool async)
        {
            return base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Conditional_expression_with_test_being_simplified_to_constant_complex(bool isAsync)
        {
            return base.Conditional_expression_with_test_being_simplified_to_constant_complex(isAsync);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Correlated_collection_with_complex_OrderBy(bool async)
        {
            return base.Correlated_collection_with_complex_OrderBy(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Correlated_collection_with_very_complex_order_by(bool async)
        {
            return base.Correlated_collection_with_very_complex_order_by(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Correlated_collections_with_FirstOrDefault(bool async)
        {
            return base.Correlated_collections_with_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task FirstOrDefault_on_empty_collection_of_DateTime_in_subquery(bool async)
        {
            return base.FirstOrDefault_on_empty_collection_of_DateTime_in_subquery(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task FirstOrDefault_over_int_compared_to_zero(bool async)
        {
            return base.FirstOrDefault_over_int_compared_to_zero(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_collection_OrderBy_aggregate(bool async)
        {
            return base.Include_collection_OrderBy_aggregate(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_collection_with_complex_OrderBy2(bool async)
        {
            return base.Include_collection_with_complex_OrderBy2(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_collection_with_complex_OrderBy3(bool async)
        {
            return base.Include_collection_with_complex_OrderBy3(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_with_complex_order_by(bool async)
        {
            return base.Include_with_complex_order_by(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_complex_orderings(bool async)
        {
            return base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_complex_orderings(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Optional_navigation_with_collection_composite_key(bool async)
        {
            return base.Optional_navigation_with_collection_composite_key(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Project_one_value_type_converted_to_nullable_from_empty_collection(bool async)
        {
            return base.Project_one_value_type_converted_to_nullable_from_empty_collection(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Project_one_value_type_from_empty_collection(bool async)
        {
            return base.Project_one_value_type_from_empty_collection(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Query_with_complex_let_containing_ordering_and_filter_projecting_firstOrDefault_element_of_let(bool async)
        {
            return base.Query_with_complex_let_containing_ordering_and_filter_projecting_firstOrDefault_element_of_let(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_boolean(bool async)
        {
            return base.Select_subquery_boolean(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_boolean_with_pushdown(bool async)
        {
            return base.Select_subquery_boolean_with_pushdown(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_int_with_inside_cast_and_coalesce(bool async)
        {
            return base.Select_subquery_int_with_inside_cast_and_coalesce(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_int_with_outside_cast_and_coalesce(bool async)
        {
            return base.Select_subquery_int_with_outside_cast_and_coalesce(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_int_with_pushdown_and_coalesce(bool async)
        {
            return base.Select_subquery_int_with_pushdown_and_coalesce(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_int_with_pushdown_and_coalesce2(bool async)
        {
            return base.Select_subquery_int_with_pushdown_and_coalesce2(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_boolean_empty(bool async)
        {
            return base.Select_subquery_boolean_empty(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_boolean_empty_with_pushdown(bool async)
        {
            return base.Select_subquery_boolean_empty_with_pushdown(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_distinct_singleordefault_boolean2(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean2(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_distinct_singleordefault_boolean_empty2(bool async)
        {
            return base.Select_subquery_distinct_singleordefault_boolean_empty2(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_subquery_boolean(bool async)
        {
            return base.Where_subquery_boolean(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_subquery_boolean_with_pushdown(bool async)
        {
            return base.Where_subquery_boolean_with_pushdown(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_subquery_distinct_singleordefault_boolean2(bool async)
        {
            return base.Where_subquery_distinct_singleordefault_boolean2(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_projecting_single_constant_bool(bool async)
        {
            return base.Select_subquery_projecting_single_constant_bool(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_projecting_single_constant_int(bool async)
        {
            return base.Select_subquery_projecting_single_constant_int(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Select_subquery_projecting_single_constant_string(bool async)
        {
            return base.Select_subquery_projecting_single_constant_string(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Set_operator_with_navigation_in_projection_groupby_aggregate(bool async)
        {
            return base.Set_operator_with_navigation_in_projection_groupby_aggregate(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Where_contains_on_navigation_with_composite_keys(bool async)
        {
            return base.Where_contains_on_navigation_with_composite_keys(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Filter_on_subquery_projecting_one_value_type_from_empty_collection(bool async)
        {
            return base.Filter_on_subquery_projecting_one_value_type_from_empty_collection(async);
        }

        [ConditionalTheory(Skip = "Another LATERAL JOIN bug in MySQL. Grouping leads to unexpected result set.")]
        public override Task Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(bool async)
        {
            return base.Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);
        }
    }
}
