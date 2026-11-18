using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public partial class NorthwindMiscellaneousQuerySingleStoreTest : NorthwindMiscellaneousQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindMiscellaneousQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        public override async Task Take_Skip(bool async)
        {
            await base.Take_Skip(async);

        AssertSql(
"""
@p='10'
@p0='5'

SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactName`
    LIMIT @p
) AS `c0`
ORDER BY `c0`.`ContactName`
LIMIT 18446744073709551610 OFFSET @p0
""");
        }

        [ConditionalTheory]
        public override async Task Select_expression_references_are_updated_correctly_with_subquery(bool async)
        {
            await base.Select_expression_references_are_updated_correctly_with_subquery(async);

            AssertSql(
"""
@nextYear='2017'

SELECT DISTINCT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` IS NOT NULL AND (EXTRACT(year FROM `o`.`OrderDate`) < @nextYear)
""");
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Entity_equality_orderby_subquery(bool async)
        {
            // Ordering in the base test is arbitrary.
            return AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.OrderBy(o => o.OrderID).FirstOrDefault()).ThenBy(c => c.CustomerID),
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.FirstOrDefault() == null ? (int?)null : c.Orders.OrderBy(o => o.OrderID).FirstOrDefault().OrderID).ThenBy(c => c.CustomerID),
                assertOrder: true);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task All_top_level_subquery(bool async)
        {
            return base.All_top_level_subquery(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task All_top_level_subquery_ef_property(bool async)
        {
            return base.All_top_level_subquery_ef_property(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Anonymous_subquery_orderby(bool async)
        {
            return base.Anonymous_subquery_orderby(async);
        }

        public override Task Checked_context_with_arithmetic_does_not_fail(bool isAsync)
        {
            checked
            {
                return AssertQuery(
                    isAsync,
                    ss => ss.Set<OrderDetail>()
                        .Where(w => w.Quantity + 1 == 5 && w.Quantity - 1 == 3 && w.Quantity * 1 == w.Quantity)
                        .OrderBy(o => o.OrderID).ThenBy(o => o.ProductID),
                    assertOrder: true,
                    elementAsserter: (e, a) => { AssertEqual(e, a); });
            }
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Complex_nested_query_properly_binds_to_grandparent_when_parent_returns_scalar_result(bool async)
        {
            return base.Complex_nested_query_properly_binds_to_grandparent_when_parent_returns_scalar_result(async);
        }

        public override Task Convert_to_nullable_on_nullable_value_is_ignored(bool async)
        {
            return AssertQuery(
                async,
                ss => ss.Set<Order>().OrderBy(o => o.OrderID).Select(o => new Order {OrderDate = o.OrderDate.Value}));
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task DTO_subquery_orderby(bool async)
        {
            return base.DTO_subquery_orderby(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Entity_equality_on_subquery_with_null_check(bool async)
        {
            return base.Entity_equality_on_subquery_with_null_check(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Entity_equality_orderby_descending_subquery_composite_key(bool async)
        {
            return base.Entity_equality_orderby_descending_subquery_composite_key(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task First_on_collection_in_projection(bool async)
        {
            return base.First_on_collection_in_projection(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task FirstOrDefault_with_predicate_nested(bool async)
        {
            return base.FirstOrDefault_with_predicate_nested(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Let_entity_equality_to_null(bool async)
        {
            return base.Let_entity_equality_to_null(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Let_entity_equality_to_other_entity(bool async)
        {
            return base.Let_entity_equality_to_other_entity(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task OrderBy_any(bool async)
        {
            return base.OrderBy_any(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task OrderBy_correlated_subquery1(bool async)
        {
            return base.OrderBy_correlated_subquery1(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Pending_selector_in_cardinality_reducing_method_is_applied_before_expanding_collection_navigation_member(
            bool async)
        {
            return base.Pending_selector_in_cardinality_reducing_method_is_applied_before_expanding_collection_navigation_member(async);
        }

        public override Task Select_expression_date_add_milliseconds_below_the_range(bool async)
        {
            return AssertQuery(
                async,
                ss => ss.Set<Order>().OrderBy(o => o.OrderID).Where(o => o.OrderDate != null)
                    .Select(o => new Order {OrderDate = o.OrderDate.Value.AddMilliseconds(-1000000000000)}));
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
        public override Task Select_Where_Subquery_Deep_First(bool async)
        {
            return base.Select_Where_Subquery_Deep_First(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
        public override Task Select_Where_Subquery_Deep_Single(bool async)
        {
            return base.Select_Where_Subquery_Deep_Single(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Select_Where_Subquery_Equality(bool async)
        {
            return base.Select_Where_Subquery_Equality(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Subquery_member_pushdown_does_not_change_original_subquery_model(bool async)
        {
            return base.Subquery_member_pushdown_does_not_change_original_subquery_model(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Subquery_member_pushdown_does_not_change_original_subquery_model2(bool async)
        {
            return base.Subquery_member_pushdown_does_not_change_original_subquery_model2(async);
        }

        public override Task Using_string_Equals_with_StringComparison_throws_informative_error(bool async)
        {
            return AssertTranslationFailedWithDetails(
                () => AssertQuery(
                    async,
                    ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("ALFKI", StringComparison.InvariantCulture))),
                SingleStoreStrings.QueryUnableToTranslateMethodWithStringComparison(nameof(String), nameof(string.Equals),
                    nameof(SingleStoreDbContextOptionsBuilder.EnableStringComparisonTranslations)));
        }

        public override Task Using_static_string_Equals_with_StringComparison_throws_informative_error(bool async)
        {
            return AssertTranslationFailedWithDetails(
                () => AssertQuery(
                    async,
                    ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "ALFKI", StringComparison.InvariantCulture))),
                SingleStoreStrings.QueryUnableToTranslateMethodWithStringComparison(nameof(String), nameof(string.Equals),
                    nameof(SingleStoreDbContextOptionsBuilder.EnableStringComparisonTranslations)));
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_multiple_elements_First(bool async)
        {
            return base.Where_query_composition_entity_equality_multiple_elements_First(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_multiple_elements_FirstOrDefault(bool async)
        {
            return base.Where_query_composition_entity_equality_multiple_elements_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_multiple_elements_Single(bool async)
        {
            return base.Where_query_composition_entity_equality_multiple_elements_Single(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_multiple_elements_SingleOrDefault(bool async)
        {
            return base.Where_query_composition_entity_equality_multiple_elements_SingleOrDefault(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_one_element_First(bool async)
        {
            return base.Where_query_composition_entity_equality_one_element_First(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_one_element_FirstOrDefault(bool async)
        {
            return base.Where_query_composition_entity_equality_one_element_FirstOrDefault(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_one_element_Single(bool async)
        {
            return base.Where_query_composition_entity_equality_one_element_Single(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Where_query_composition_entity_equality_one_element_SingleOrDefault(bool async)
        {
            return base.Where_query_composition_entity_equality_one_element_SingleOrDefault(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Subquery_is_not_null_translated_correctly(bool async)
        {
            return base.Subquery_is_not_null_translated_correctly(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Subquery_is_null_translated_correctly(bool async)
        {
            return base.Subquery_is_not_null_translated_correctly(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Dependent_to_principal_navigation_equal_to_null_for_subquery(bool async)
        {
            return base.Dependent_to_principal_navigation_equal_to_null_for_subquery(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        public override Task Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_parameter(bool async)
        {
            return base.Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_parameter(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        public override Task Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_constant_one(bool async)
        {
            return base.Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_constant_one(async);
        }

        public override Task Where_query_composition2(bool async)
        {
            return AssertQuery(
                async,
                ss => from e1 in ss.Set<Employee>().OrderBy(e => e.EmployeeID).Take(3)
                      where e1.FirstName
                          == (from e2 in ss.Set<Employee>().OrderBy(e => e.EmployeeID)
                              select new { Foo = e2 }).First().Foo.FirstName
                      select e1);
        }

        public override Task Where_query_composition2_FirstOrDefault(bool async)
        {
            return AssertQuery(
                async,
                ss => from e1 in ss.Set<Employee>().OrderBy(e => e.EmployeeID).Take(3)
                    where e1.FirstName
                          == (from e2 in ss.Set<Employee>().OrderBy(e => e.EmployeeID)
                              select e2).FirstOrDefault().FirstName
                    select e1);
        }

        public override Task Where_query_composition2_FirstOrDefault_with_anonymous(bool async)
        {
            return AssertQuery(
                async,
                ss => from e1 in ss.Set<Employee>().OrderBy(e => e.EmployeeID).Take(3)
                    where e1.FirstName
                          == (from e2 in ss.Set<Employee>().OrderBy(e => e.EmployeeID)
                              select new { Foo = e2 }).FirstOrDefault().Foo.FirstName
                    select e1);
        }

        /// <summary>
        /// Needs explicit ordering of ProductIds to work with MariaDB.
        /// </summary>
        public override async Task Projection_skip_collection_projection(bool async)
        {
            // await base.Projection_skip_collection_projection(async);
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.OrderID < 10300)
                    .OrderBy(o => o.OrderID)
                    .Select(o => new { Item = o })
                    .Skip(5)
                    .Select(e => new { e.Item.OrderID, ProductIds = e.Item.OrderDetails.OrderBy(od => od.ProductID).Select(od => od.ProductID).ToList() }), // added .OrderBy(od => od.ProductID)
                assertOrder: true,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.OrderID, a.OrderID);
                    AssertCollection(e.ProductIds, a.ProductIds, ordered: true, elementAsserter: (ie, ia) => Assert.Equal(ie, ia));
                });

            AssertSql(
                """
                @p='5'

                SELECT `o1`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
                FROM (
                    SELECT `o`.`OrderID`
                    FROM `Orders` AS `o`
                    WHERE `o`.`OrderID` < 10300
                    ORDER BY `o`.`OrderID`
                    LIMIT 18446744073709551610 OFFSET @p
                ) AS `o1`
                LEFT JOIN `Order Details` AS `o0` ON `o1`.`OrderID` = `o0`.`OrderID`
                ORDER BY `o1`.`OrderID`, `o0`.`ProductID`
                """);
        }

        /// <summary>
        /// Needs explicit ordering of ProductIds to work with MariaDB.
        /// </summary>
        public override async Task Projection_skip_take_collection_projection(bool async)
        {
            // await base.Projection_skip_take_collection_projection(async);
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.OrderID < 10300)
                    .OrderBy(o => o.OrderID)
                    .Select(o => new { Item = o })
                    .Skip(5)
                    .Take(10)
                    .Select(e => new { e.Item.OrderID, ProductIds = e.Item.OrderDetails.OrderBy(od => od.ProductID).Select(od => od.ProductID).ToList() }), // added .OrderBy(od => od.ProductID)
                assertOrder: true,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.OrderID, a.OrderID);
                    AssertCollection(e.ProductIds, a.ProductIds, ordered: true, elementAsserter: (ie, ia) => Assert.Equal(ie, ia));
                });

            AssertSql(
                """
                @p0='10'
                @p='5'

                SELECT `o1`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
                FROM (
                    SELECT `o`.`OrderID`
                    FROM `Orders` AS `o`
                    WHERE `o`.`OrderID` < 10300
                    ORDER BY `o`.`OrderID`
                    LIMIT @p0 OFFSET @p
                ) AS `o1`
                LEFT JOIN `Order Details` AS `o0` ON `o1`.`OrderID` = `o0`.`OrderID`
                ORDER BY `o1`.`OrderID`, `o0`.`ProductID`
                """);
        }

        /// <summary>
        /// Needs explicit ordering of ProductIds to work with MariaDB.
        /// </summary>
        public override async Task Projection_take_collection_projection(bool async)
        {
            // await base.Projection_take_collection_projection(async);
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.OrderID < 10300)
                    .OrderBy(o => o.OrderID)
                    .Select(o => new { Item = o })
                    .Take(10)
                    .Select(e => new { e.Item.OrderID, ProductIds = e.Item.OrderDetails.OrderBy(od => od.ProductID).Select(od => od.ProductID).ToList() }), // added .OrderBy(od => od.ProductID)
                assertOrder: true,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.OrderID, a.OrderID);
                    AssertCollection(e.ProductIds, a.ProductIds, ordered: true, elementAsserter: (ie, ia) => Assert.Equal(ie, ia));
                });

            AssertSql(
                """
                @p='10'

                SELECT `o1`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
                FROM (
                    SELECT `o`.`OrderID`
                    FROM `Orders` AS `o`
                    WHERE `o`.`OrderID` < 10300
                    ORDER BY `o`.`OrderID`
                    LIMIT @p
                ) AS `o1`
                LEFT JOIN `Order Details` AS `o0` ON `o1`.`OrderID` = `o0`.`OrderID`
                ORDER BY `o1`.`OrderID`, `o0`.`ProductID`
                """);
        }

        public override Task Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(bool async)
        {
            if (AppConfig.ServerVersion.Supports.OuterApply)
            {
                // SingleStore.Data.SingleStoreClient.SingleStoreException: Reference 'CustomerID' not supported (forward reference in item list)
                return Assert.ThrowsAsync<SingleStoreException>(
                    () => base.Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(async));
            }
            else
            {
                // The LINQ expression 'OUTER APPLY ...' could not be translated. Either...
                return Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(async));
            }
        }

        public override async Task Client_code_using_instance_method_throws(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInMethodInstance(
                    "EntityFrameworkCore.SingleStore.FunctionalTests.Query.NorthwindMiscellaneousQuerySingleStoreTest",
                    "InstanceMethod"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_method_throws(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_using_instance_in_static_method(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInMethodArgument(
                    "EntityFrameworkCore.SingleStore.FunctionalTests.Query.NorthwindMiscellaneousQuerySingleStoreTest",
                    "StaticMethod"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_in_static_method(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_using_instance_in_anonymous_type(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInTree(
                    "EntityFrameworkCore.SingleStore.FunctionalTests.Query.NorthwindMiscellaneousQuerySingleStoreTest"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_in_anonymous_type(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_unknown_method(bool async)
        {
            await AssertTranslationFailedWithDetails(
                () => base.Client_code_unknown_method(async),
                CoreStrings.QueryUnableToTranslateMethod(
                    "Microsoft.EntityFrameworkCore.Query.NorthwindMiscellaneousQueryTestBase<EntityFrameworkCore.SingleStore.FunctionalTests.Query.NorthwindQuerySingleStoreFixture<Microsoft.EntityFrameworkCore.TestUtilities.NoopModelCustomizer>>",
                    nameof(UnknownMethod)));

            AssertSql();
        }

        public override async Task Entity_equality_through_subquery_composite_key(bool async)
        {
            var message = (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Entity_equality_through_subquery_composite_key(async))).Message;

            Assert.Equal(
                CoreStrings.EntityEqualityOnCompositeKeyEntitySubqueryNotSupported("==", nameof(OrderDetail)),
                message);

            AssertSql();
        }

        public override async Task Max_on_empty_sequence_throws(bool async)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Max_on_empty_sequence_throws(async));

            AssertSql(
                @"SELECT (
    SELECT MAX(`o`.`OrderID`)
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`) AS `Max`
FROM `Customers` AS `c`");
        }

        public override async Task
            Select_DTO_constructor_distinct_with_collection_projection_translated_to_server_with_binding_after_client_eval(bool async)
        {
            using var context = CreateContext();
            var actualQuery = context.Set<Order>()
                .Where(o => o.OrderID < 10300)
                .Select(o => new { A = new OrderCountDTO(o.CustomerID), o.CustomerID })
                .Distinct()
                .Select(e => new { e.A, Orders = context.Set<Order>().Where(o => o.CustomerID == e.CustomerID)
                    .OrderBy(o => o.OrderID) // <-- added
                    .ToList() });

            var actual = async
                ? (await actualQuery.ToListAsync()).OrderBy(e => e.A.Id).ToList()
                : actualQuery.ToList().OrderBy(e => e.A.Id).ToList();

            var expected = Fixture.GetExpectedData().Set<Order>()
                .Where(o => o.OrderID < 10300)
                .Select(o => new { A = new OrderCountDTO(o.CustomerID), o.CustomerID })
                .Distinct()
                .Select(e => new { e.A, Orders = Fixture.GetExpectedData().Set<Order>().Where(o => o.CustomerID == e.CustomerID)
                    .OrderBy(o => o.OrderID) // <-- added
                    .ToList() })
                .ToList().OrderBy(e => e.A.Id).ToList();

            Assert.Equal(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].A.Id, actual[i].A.Id);
                Assert.True(expected[i].Orders?.SequenceEqual(actual[i].Orders) ?? true);
            }

            AssertSql(
                """
                SELECT `o0`.`CustomerID`, `o1`.`OrderID`, `o1`.`CustomerID`, `o1`.`EmployeeID`, `o1`.`OrderDate`
                FROM (
                    SELECT DISTINCT `o`.`CustomerID`
                    FROM `Orders` AS `o`
                    WHERE `o`.`OrderID` < 10300
                ) AS `o0`
                LEFT JOIN `Orders` AS `o1` ON `o0`.`CustomerID` = `o1`.`CustomerID`
                ORDER BY `o0`.`CustomerID`, `o1`.`OrderID`
                """);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task DefaultIfEmpty_Sum_over_collection_navigation(bool async)
        {
            return base.DefaultIfEmpty_Sum_over_collection_navigation(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override async Task Parameter_collection_Contains_with_projection_and_ordering(bool async)
        {
#if EFCORE_DEBUG_BUILD
            // GroupBy debug assert. Issue #26104.
            Assert.StartsWith(
                "Missing alias in the list",
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Parameter_collection_Contains_with_projection_and_ordering(async))).Message);
#else
            await base.Parameter_collection_Contains_with_projection_and_ordering(async);

            AssertSql(
                """
                SELECT `o`.`Quantity` AS `Key`, (
                    SELECT MAX(`o3`.`OrderDate`)
                    FROM `Order Details` AS `o2`
                    INNER JOIN `Orders` AS `o3` ON `o2`.`OrderID` = `o3`.`OrderID`
                    WHERE `o2`.`OrderID` IN (10248, 10249) AND (`o`.`Quantity` = `o2`.`Quantity`)) AS `MaxTimestamp`
                FROM `Order Details` AS `o`
                WHERE `o`.`OrderID` IN (10248, 10249)
                GROUP BY `o`.`Quantity`
                ORDER BY (
                    SELECT MAX(`o3`.`OrderDate`)
                    FROM `Order Details` AS `o2`
                    INNER JOIN `Orders` AS `o3` ON `o2`.`OrderID` = `o3`.`OrderID`
                    WHERE `o2`.`OrderID` IN (10248, 10249) AND (`o`.`Quantity` = `o2`.`Quantity`))
                """);
#endif
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed")]
        public override Task All_on_distinct(bool async)
        {
            return base.All_on_distinct(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
        public override async Task Subquery_with_navigation_inside_inline_collection(bool async)
        {
            await base.Subquery_with_navigation_inside_inline_collection(async);

            AssertSql("");
        }

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => SingleStoreTestHelpers.AssertAllMethodsOverridden(GetType());

        public override async Task Projecting_collection_split(bool async)
        {
            await base.Projecting_collection_split(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`
""",
                //
                """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`, `c`.`CustomerID`
FROM `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Projecting_collection_then_include_split(bool async)
        {
            await base.Projecting_collection_then_include_split(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`
""",
                //
                """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`, `c`.`CustomerID`
FROM `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`, `o`.`OrderID`
""",
                //
                """
SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`, `c`.`CustomerID`, `o`.`OrderID`
FROM `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
INNER JOIN `Order Details` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`, `o`.`OrderID`
""");
        }

        public override async Task Multiple_context_instances(bool async)
        {
            await base.Multiple_context_instances(async);

            AssertSql();
        }

        public override async Task Multiple_context_instances_2(bool async)
        {
            await base.Multiple_context_instances_2(async);

            AssertSql();
        }

        public override async Task Multiple_context_instances_set(bool async)
        {
            await base.Multiple_context_instances_set(async);

            AssertSql();
        }

        public override async Task Multiple_context_instances_parameter(bool async)
        {
            await base.Multiple_context_instances_parameter(async);

            AssertSql();
        }

        public override async Task Query_when_evaluatable_queryable_method_call_with_repository(bool async)
        {
            await base.Query_when_evaluatable_queryable_method_call_with_repository(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`)
""",
                //
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`)
""",
                //
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`)
""");
        }

        public override async Task Lifting_when_subquery_nested_order_by_simple(bool async)
        {
            await base.Lifting_when_subquery_nested_order_by_simple(async);

            AssertSql(
                """
@p='2'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
INNER JOIN (
    SELECT DISTINCT `c1`.`CustomerID`
    FROM (
        SELECT `c`.`CustomerID`
        FROM `Customers` AS `c`
        ORDER BY `c`.`CustomerID`
        LIMIT @p
    ) AS `c1`
    CROSS JOIN `Customers` AS `c0`
) AS `s` ON `o`.`CustomerID` = `s`.`CustomerID`
ORDER BY `s`.`CustomerID`
""");
        }

        public override async Task Lifting_when_subquery_nested_order_by_anonymous(bool async)
        {
            await base.Lifting_when_subquery_nested_order_by_anonymous(async);

            AssertSql(
                """
@p='2'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
INNER JOIN (
    SELECT DISTINCT `c1`.`CustomerID`
    FROM (
        SELECT `c`.`CustomerID`
        FROM `Customers` AS `c`
        ORDER BY `c`.`CustomerID`
        LIMIT @p
    ) AS `c1`
    CROSS JOIN `Customers` AS `c0`
) AS `s` ON `o`.`CustomerID` = `s`.`CustomerID`
ORDER BY `s`.`CustomerID`
""");
        }

        public override async Task Local_dictionary(bool async)
        {
            await base.Local_dictionary(async);

            AssertSql(
                """
@p='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @p
LIMIT 2
""");
        }

        public override void Query_composition_against_ienumerable_set()
        {
            base.Query_composition_against_ienumerable_set();

            AssertSql(
                """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
""");
        }

        public override async Task Shaper_command_caching_when_parameter_names_different(bool async)
        {
            await base.Shaper_command_caching_when_parameter_names_different(async);

            AssertSql(
                """
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""",
                //
                """
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
        }

        public override async Task Can_convert_manually_build_expression_with_default(bool async)
        {
            await base.Can_convert_manually_build_expression_with_default(async);

            AssertSql(
                """
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`City` IS NOT NULL
""",
                //
                """
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`City` IS NOT NULL
""");
        }

        public override async Task Entity_equality_self(bool async)
        {
            await base.Entity_equality_self(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
""");
        }

        public override async Task Entity_equality_local(bool async)
        {
            await base.Entity_equality_local(async);

            AssertSql(
                """
@entity_equality_local_CustomerID='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @entity_equality_local_CustomerID
""");
        }

        public override async Task Entity_equality_local_composite_key(bool async)
        {
            await base.Entity_equality_local_composite_key(async);

            AssertSql(
                """
@entity_equality_local_OrderID='10248' (Nullable = true)
@entity_equality_local_ProductID='11' (Nullable = true)

SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE (`o`.`OrderID` = @entity_equality_local_OrderID) AND (`o`.`ProductID` = @entity_equality_local_ProductID)
""");
        }

        public override async Task Entity_equality_local_double_check(bool async)
        {
            await base.Entity_equality_local_double_check(async);

            AssertSql(
                """
@entity_equality_local_CustomerID='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` = @entity_equality_local_CustomerID) AND (@entity_equality_local_CustomerID = `c`.`CustomerID`)
""");
        }

        public override async Task Join_with_entity_equality_local_on_both_sources(bool async)
        {
            await base.Join_with_entity_equality_local_on_both_sources(async);

            AssertSql(
                """
@entity_equality_local_CustomerID='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` = @entity_equality_local_CustomerID
) AS `c1` ON `c`.`CustomerID` = `c1`.`CustomerID`
WHERE `c`.`CustomerID` = @entity_equality_local_CustomerID
""");
        }

        public override async Task Entity_equality_local_inline(bool async)
        {
            await base.Entity_equality_local_inline(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ANATR'
""");
        }

        public override async Task Entity_equality_local_inline_composite_key(bool async)
        {
            await base.Entity_equality_local_inline_composite_key(async);

            AssertSql(
                """
SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE (`o`.`OrderID` = 10248) AND (`o`.`ProductID` = 11)
""");
        }

        public override async Task Entity_equality_null(bool async)
        {
            await base.Entity_equality_null(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE FALSE
""");
        }

        public override async Task Entity_equality_null_composite_key(bool async)
        {
            await base.Entity_equality_null_composite_key(async);

            AssertSql(
                """
SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
WHERE FALSE
""");
        }

        public override async Task Entity_equality_not_null(bool async)
        {
            await base.Entity_equality_not_null(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
""");
        }

        public override async Task Entity_equality_not_null_composite_key(bool async)
        {
            await base.Entity_equality_not_null_composite_key(async);

            AssertSql(
                """
SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
""");
        }

        public override async Task Entity_equality_through_nested_anonymous_type_projection(bool async)
        {
            await base.Entity_equality_through_nested_anonymous_type_projection(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`CustomerID` IS NOT NULL
""");
        }

        public override async Task Entity_equality_through_DTO_projection(bool async)
        {
            await base.Entity_equality_through_DTO_projection(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`CustomerID` IS NOT NULL
""");
        }

        public override async Task Entity_equality_through_subquery(bool async)
        {
            await base.Entity_equality_through_subquery(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
        }

        public override async Task Entity_equality_through_include(bool async)
        {
            await base.Entity_equality_through_include(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE FALSE
""");
        }

        public override async Task Entity_equality_orderby(bool async)
        {
            await base.Entity_equality_orderby(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`
""");
        }

        public override async Task Entity_equality_orderby_descending_composite_key(bool async)
        {
            await base.Entity_equality_orderby_descending_composite_key(async);

            AssertSql(
                """
SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
ORDER BY `o`.`OrderID` DESC, `o`.`ProductID` DESC
""");
        }

        public override async Task Queryable_simple(bool async)
        {
            await base.Queryable_simple(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
        }

        public override async Task Queryable_simple_anonymous(bool async)
        {
            await base.Queryable_simple_anonymous(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
        }

        public override async Task Queryable_simple_anonymous_projection_subquery(bool async)
        {
            await base.Queryable_simple_anonymous_projection_subquery(async);

            AssertSql(
                """
@p='91'

SELECT `c`.`City`
FROM `Customers` AS `c`
LIMIT @p
""");
        }

        public override async Task Queryable_simple_anonymous_subquery(bool async)
        {
            await base.Queryable_simple_anonymous_subquery(async);

            AssertSql(
                """
@p='91'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
LIMIT @p
""");
        }

        public override async Task Queryable_reprojection(bool async)
        {
            await base.Queryable_reprojection(async);

            AssertSql();
        }

        public override async Task Queryable_nested_simple(bool async)
        {
            await base.Queryable_nested_simple(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
        }

        public override async Task Take_simple(bool async)
        {
            await base.Take_simple(async);

            AssertSql(
                """
@p='10'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`
LIMIT @p
""");
        }

        public override async Task Take_simple_parameterized(bool async)
        {
            await base.Take_simple_parameterized(async);

            AssertSql(
                """
@p='10'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`
LIMIT @p
""");
        }

        public override async Task Take_simple_projection(bool async)
        {
            await base.Take_simple_projection(async);

            AssertSql(
                """
@p='10'

SELECT `c`.`City`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`
LIMIT @p
""");
        }

        public override async Task Take_subquery_projection(bool async)
        {
            await base.Take_subquery_projection(async);

            AssertSql(
                """
@p='2'

SELECT `c`.`City`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`
LIMIT @p
""");
        }

        public override async Task Skip(bool async)
        {
            await base.Skip(async);

            AssertSql(
                """
@p='5'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`
LIMIT 18446744073709551610 OFFSET @p
""");
        }

        public override async Task Skip_no_orderby(bool async)
        {
            await base.Skip_no_orderby(async);

            AssertSql(
                """
@p='5'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
LIMIT 18446744073709551610 OFFSET @p
""");
        }

        public override async Task Skip_orderby_const(bool async)
        {
            await base.Skip_orderby_const(async);

            AssertSql(
                """
@p='5'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY (SELECT 1)
LIMIT 18446744073709551610 OFFSET @p
""");
        }

        public override async Task Distinct_Skip(bool async)
        {
            await base.Distinct_Skip(async);

            AssertSql(
                """
@p='5'

SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM (
    SELECT DISTINCT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
) AS `c0`
ORDER BY `c0`.`CustomerID`
LIMIT 18446744073709551610 OFFSET @p
""");
        }

        public override async Task Skip_Take(bool async)
        {
            await base.Skip_Take(async);

            AssertSql(
                """
@p0='10'
@p='5'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
ORDER BY `c`.`ContactName`
LIMIT @p0 OFFSET @p
""");
        }

        public override async Task Join_Customers_Orders_Skip_Take(bool async)
        {
            await base.Join_Customers_Orders_Skip_Take(async);

            AssertSql(
                """
@p0='5'
@p='10'

SELECT `c`.`ContactName`, `o`.`OrderID`
FROM `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
ORDER BY `o`.`OrderID`
LIMIT @p0 OFFSET @p
""");
        }

        public override async Task Join_Customers_Orders_Skip_Take_followed_by_constant_projection(bool async)
        {
            await base.Join_Customers_Orders_Skip_Take_followed_by_constant_projection(async);

            AssertSql(
                """
@p0='5'
@p='10'

SELECT 'Foo'
FROM `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
ORDER BY `o`.`OrderID`
LIMIT @p0 OFFSET @p
""");
        }

        public override async Task Join_Customers_Orders_Projection_With_String_Concat_Skip_Take(bool async)
        {
            await base.Join_Customers_Orders_Projection_With_String_Concat_Skip_Take(async);

            AssertSql(
                """
@p0='5'
@p='10'

SELECT CONCAT(CONCAT(COALESCE(`c`.`ContactName`, ''), ' '), COALESCE(`c`.`ContactTitle`, '')) AS `Contact`, `o`.`OrderID`
FROM `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
ORDER BY `o`.`OrderID`
LIMIT @p0 OFFSET @p
""");
        }

        public override async Task Join_Customers_Orders_Orders_Skip_Take_Same_Properties(bool async)
        {
            await base.Join_Customers_Orders_Orders_Skip_Take_Same_Properties(async);

            AssertSql(
                """
@p0='5'
@p='10'

SELECT `o`.`OrderID`, `c`.`CustomerID` AS `CustomerIDA`, `c0`.`CustomerID` AS `CustomerIDB`, `c`.`ContactName` AS `ContactNameA`, `c0`.`ContactName` AS `ContactNameB`
FROM `Orders` AS `o`
INNER JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
INNER JOIN `Customers` AS `c0` ON `o`.`CustomerID` = `c0`.`CustomerID`
ORDER BY `o`.`OrderID`
LIMIT @p0 OFFSET @p
""");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        private class OrderCountDTO
        {
            public string Id { get; set; }
            public int Count { get; set; }

            public OrderCountDTO()
            {
            }

            public OrderCountDTO(string id)
            {
                Id = id;
                Count = 0;
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                {
                    return false;
                }

                return ReferenceEquals(this, obj) ? true : obj.GetType() == GetType() && Equals((OrderCountDTO)obj);
            }

            private bool Equals(OrderCountDTO other)
                => string.Equals(Id, other.Id) && Count == other.Count;

            public override int GetHashCode()
                => HashCode.Combine(Id, Count);
        }
    }
}
