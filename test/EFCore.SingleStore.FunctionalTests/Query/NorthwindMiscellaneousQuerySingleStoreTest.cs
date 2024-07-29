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
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

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

        protected override bool CanExecuteQueryString
            => true;

        public override async Task Select_bitwise_or(bool async)
        {
            await base.Select_bitwise_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, (`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Select_bitwise_or_multiple(bool async)
        {
            await base.Select_bitwise_or_multiple(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) | (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Select_bitwise_and(bool async)
        {
            await base.Select_bitwise_and(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, (`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Select_bitwise_and_or(bool async)
        {
            await base.Select_bitwise_and_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) | (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Where_bitwise_or_with_logical_or(bool async)
        {
            await base.Where_bitwise_or_with_logical_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) OR (`c`.`CustomerID` = 'ANTON')");
        }

        public override async Task Where_bitwise_and_with_logical_and(bool async)
        {
            await base.Where_bitwise_and_with_logical_and(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) AND (`c`.`CustomerID` = 'ANTON')");
        }

        public override async Task Where_bitwise_or_with_logical_and(bool async)
        {
            await base.Where_bitwise_or_with_logical_and(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) AND (`c`.`Country` = 'Germany')");
        }

        public override async Task Where_bitwise_and_with_logical_or(bool async)
        {
            await base.Where_bitwise_and_with_logical_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) OR (`c`.`CustomerID` = 'ANTON')");
        }

        public override async Task Where_bitwise_binary_not(bool async)
        {
            await base.Where_bitwise_binary_not(async);

            AssertSql(
                @"@__negatedId_0='-10249'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE CAST(~`o`.`OrderID` AS signed) = @__negatedId_0");
        }

        public override async Task Where_bitwise_binary_and(bool async)
        {
            await base.Where_bitwise_binary_and(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (`o`.`OrderID` & 10248) = 10248");
        }

        public override async Task Where_bitwise_binary_or(bool async)
        {
            await base.Where_bitwise_binary_or(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (`o`.`OrderID` | 10248) = 10248");
        }

        public override async Task Select_bitwise_or_with_logical_or(bool async)
        {
            await base.Select_bitwise_or_with_logical_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) OR (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Select_bitwise_and_with_logical_and(bool async)
        {
            await base.Select_bitwise_and_with_logical_and(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) AND (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        [ConditionalTheory]
        public override async Task Take_Skip(bool async)
        {
            await base.Take_Skip(async);

            AssertSql(
                @"@__p_0='10'
@__p_1='5'

SELECT `t`.`CustomerID`, `t`.`Address`, `t`.`City`, `t`.`CompanyName`, `t`.`ContactName`, `t`.`ContactTitle`, `t`.`Country`, `t`.`Fax`, `t`.`Phone`, `t`.`PostalCode`, `t`.`Region`
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactName`
    LIMIT @__p_0
) AS `t`
ORDER BY `t`.`ContactName`
LIMIT 18446744073709551610 OFFSET @__p_1");
        }

        [ConditionalTheory]
        public override async Task Select_expression_references_are_updated_correctly_with_subquery(bool async)
        {
            await base.Select_expression_references_are_updated_correctly_with_subquery(async);

            AssertSql(
                @"@__nextYear_0='2017'

SELECT DISTINCT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` IS NOT NULL AND (EXTRACT(year FROM `o`.`OrderDate`) < @__nextYear_0)");
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Entity_equality_orderby_subquery(bool async)
        {
            // Ordering in the base test is arbitrary.
            return AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.OrderBy(o => o.OrderID).FirstOrDefault()).ThenBy(c => c.CustomerID),
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.FirstOrDefault() == null ? (int?)null : c.Orders.OrderBy(o => o.OrderID).FirstOrDefault().OrderID).ThenBy(c => c.CustomerID),
                entryCount: 91,
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
                    entryCount: 55,
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
        public override void OrderBy_any()
        {
            base.OrderBy_any();
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

        [ConditionalFact(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
        public override void Select_Where_Subquery_Deep_First()
        {
            base.Select_Where_Subquery_Deep_First();
        }

        [ConditionalFact(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
        public override void Select_Where_Subquery_Deep_Single()
        {
            base.Select_Where_Subquery_Deep_Single();
        }

        [ConditionalFact(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override void Select_Where_Subquery_Equality()
        {
            base.Select_Where_Subquery_Equality();
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

        public override Task Where_query_composition2(bool async)
        {
            return AssertQuery(
                async,
                ss => from e1 in ss.Set<Employee>().OrderBy(e => e.EmployeeID).Take(3)
                      where e1.FirstName
                          == (from e2 in ss.Set<Employee>().OrderBy(e => e.EmployeeID)
                              select new { Foo = e2 }).First().Foo.FirstName
                      select e1,
                entryCount: 1);
        }

        public override Task Where_query_composition2_FirstOrDefault(bool async)
        {
            return AssertQuery(
                async,
                ss => from e1 in ss.Set<Employee>().OrderBy(e => e.EmployeeID).Take(3)
                    where e1.FirstName
                          == (from e2 in ss.Set<Employee>().OrderBy(e => e.EmployeeID)
                              select e2).FirstOrDefault().FirstName
                    select e1,
                entryCount: 1);
        }

        public override Task Where_query_composition2_FirstOrDefault_with_anonymous(bool async)
        {
            return AssertQuery(
                async,
                ss => from e1 in ss.Set<Employee>().OrderBy(e => e.EmployeeID).Take(3)
                    where e1.FirstName
                          == (from e2 in ss.Set<Employee>().OrderBy(e => e.EmployeeID)
                              select new { Foo = e2 }).FirstOrDefault().Foo.FirstName
                    select e1,
                entryCount: 1);
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
                @"@__p_0='5'

SELECT `t`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
FROM (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
    ORDER BY `o`.`OrderID`
    LIMIT 18446744073709551610 OFFSET @__p_0
) AS `t`
LEFT JOIN `Order Details` AS `o0` ON `t`.`OrderID` = `o0`.`OrderID`
ORDER BY `t`.`OrderID`, `o0`.`ProductID`");
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
                @"@__p_1='10'
@__p_0='5'

SELECT `t`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
FROM (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
    ORDER BY `o`.`OrderID`
    LIMIT @__p_1 OFFSET @__p_0
) AS `t`
LEFT JOIN `Order Details` AS `o0` ON `t`.`OrderID` = `o0`.`OrderID`
ORDER BY `t`.`OrderID`, `o0`.`ProductID`");
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
                @"@__p_0='10'

SELECT `t`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
FROM (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
    ORDER BY `o`.`OrderID`
    LIMIT @__p_0
) AS `t`
LEFT JOIN `Order Details` AS `o0` ON `t`.`OrderID` = `o0`.`OrderID`
ORDER BY `t`.`OrderID`, `o0`.`ProductID`");
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
                    "Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindMiscellaneousQueryMySqlTest",
                    "InstanceMethod"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_method_throws(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_using_instance_in_static_method(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInMethodArgument(
                    "Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindMiscellaneousQueryMySqlTest",
                    "StaticMethod"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_in_static_method(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_using_instance_in_anonymous_type(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInTree(
                    "Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindMiscellaneousQueryMySqlTest"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_in_anonymous_type(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_unknown_method(bool async)
        {
            await AssertTranslationFailedWithDetails(
                () => base.Client_code_unknown_method(async),
                CoreStrings.QueryUnableToTranslateMethod(
                    "Microsoft.EntityFrameworkCore.Query.NorthwindMiscellaneousQueryTestBase<Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindQueryMySqlFixture<Microsoft.EntityFrameworkCore.TestUtilities.NoopModelCustomizer>>",
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
SELECT `t`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM (
    SELECT DISTINCT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `t`
LEFT JOIN `Orders` AS `o0` ON `t`.`CustomerID` = `o0`.`CustomerID`
ORDER BY `t`.`CustomerID`, `o0`.`OrderID`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task DefaultIfEmpty_Sum_over_collection_navigation(bool async)
        {
            return base.DefaultIfEmpty_Sum_over_collection_navigation(async);
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
