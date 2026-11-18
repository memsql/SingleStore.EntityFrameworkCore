using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindWhereQuerySingleStoreTest : NorthwindWhereQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindWhereQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Where_contains_on_navigation(bool async)
        {
            return base.Where_contains_on_navigation(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task FirstOrDefault_over_scalar_projection_compared_to_not_null(bool async)
        {
            return base.FirstOrDefault_over_scalar_projection_compared_to_not_null(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task FirstOrDefault_over_scalar_projection_compared_to_null(bool async)
        {
            return base.FirstOrDefault_over_scalar_projection_compared_to_null(async);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Where_as_queryable_expression(bool async)
        {
            return base.Where_as_queryable_expression(async);
        }

        [ConditionalTheory(Skip = "issue #552")]
        public override Task Where_multiple_contains_in_subquery_with_and(bool async)
        {
            return base.Where_multiple_contains_in_subquery_with_and(async);
        }

        [ConditionalTheory(Skip = "issue #552")]
        public override Task Where_multiple_contains_in_subquery_with_or(bool async)
        {
            return base.Where_multiple_contains_in_subquery_with_or(async);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_remove(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.City.Remove(3) == "Sea"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 1, 3) = 'Sea'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_remove_count(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.City.Remove(3, 1) == "Seatle"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CONCAT(SUBSTRING(`c`.`City`, 1, 3), SUBSTRING(`c`.`City`, (3 + 1) + 1, CHAR_LENGTH(`c`.`City`) - (3 + 1))) = 'Seatle'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_guid(bool async)
        {
            var guidParameter = new Guid("4D68FE70-DDB0-47D7-B6DB-437684FA3E1F");

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => guidParameter == Guid.NewGuid()),
                assertEmpty: true);

            AssertSql(
"""
@guidParameter='4d68fe70-ddb0-47d7-b6db-437684fa3e1f'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE @guidParameter = UUID()
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_single_object(bool async)
        {
            object i = 1;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object(bool async)
        {
            object i = 1;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i, c.CustomerID) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@i='1' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object_2(bool async)
        {
            object i = 1;
            object j = 2;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i, j, c.CustomerID) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
                @"@i='1' (Size = 4000)
@j='2' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, `c`.`CustomerID`) = `c`.`CompanyName`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object_3(bool async)
        {
            object i = 1;
            object j = 2;
            object k = 3;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] {i, j, k, c.CustomerID}) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@i='1' (Size = 4000)
@j='2' (Size = 4000)
@k='3' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, @k, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_params_string_array(bool async)
        {
            var i = "A";
            var j = "B";
            var k = "C";
            var m = "D";

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] {i, j, k, m, c.CustomerID}) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@i='A' (Size = 4000)
@j='B' (Size = 4000)
@k='C' (Size = 4000)
@m='D' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, @k, @m, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_string_array(bool async)
        {
            var array = new[] {"A", "B", "C", "D"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='ABCD' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_string_array_single_element(bool async)
        {
            var array = new[] {"A"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='A' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_params_object_array(bool async)
        {
            object i = 1;
            object j = 2;
            object k = 3;
            object m = 4;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[]{i, j, k, m, c.CustomerID}) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@i='1' (Size = 4000)
@j='2' (Size = 4000)
@k='3' (Size = 4000)
@m='4' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, @k, @m, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_object_array(bool async)
        {
            var array = new object[] {1, 2, 3, 4};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1234' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_object_array_single_element(bool async)
        {
            var array = new object[] {1};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_string_enumerable(bool async)
        {
            IEnumerable<string> enumerable = new[] {"A", "B", "C", "D"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='ABCD' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_string_enumerable_single_element(bool async)
        {
            IEnumerable<string> enumerable = new[] {"A"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='A' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_generic_enumerable(bool async)
        {
            IEnumerable<int> enumerable = new[] {1, 2, 3, 4};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1234' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_generic_enumerable_single_element(bool async)
        {
            IEnumerable<int> enumerable = new[] {1};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        public override Task Where_subquery_FirstOrDefault_compared_to_entity(bool async)
        {
            return base.Where_subquery_FirstOrDefault_compared_to_entity(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        public override Task ElementAt_over_custom_projection_compared_to_not_null(bool async)
        {
            return base.ElementAt_over_custom_projection_compared_to_not_null(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Where_Queryable_AsEnumerable_Contains_negated(bool async)
        {
            return base.Where_Queryable_AsEnumerable_Contains_negated(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task ElementAtOrDefault_over_custom_projection_compared_to_null(bool async)
        {
            return base.ElementAtOrDefault_over_custom_projection_compared_to_null(async);
        }

        public override async Task Where_compare_constructed_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_constructed_multi_value_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_constructed_multi_value_not_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_multi_value_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_multi_value_not_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_multi_value_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_multi_value_not_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => SingleStoreTestHelpers.AssertAllMethodsOverridden(GetType());

        public override async Task Where_bool_client_side_negated(bool async)
        {
            await base.Where_bool_client_side_negated(async);

            AssertSql();
        }

        public override async Task EF_MultipleParameters_with_non_evaluatable_argument_throws(bool async)
        {
            await base.EF_MultipleParameters_with_non_evaluatable_argument_throws(async);

            AssertSql();
        }

        public override async Task Where_simple(bool async)
        {
            await base.Where_simple(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
""");
        }

        public override async Task<string> Where_simple_closure(bool async)
        {
            var queryString = await base.Where_simple_closure(async);

            AssertSql(
                """
@city='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city
""");

            return queryString;
        }

        public override async Task Where_indexer_closure(bool async)
        {
            await base.Where_indexer_closure(async);

            AssertSql(
                """
@p='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @p
""");
        }

        public override async Task Where_dictionary_key_access_closure(bool async)
        {
            await base.Where_dictionary_key_access_closure(async);

            AssertSql(
                """
@get_Item='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @get_Item
""");
        }

        public override async Task Where_tuple_item_closure(bool async)
        {
            await base.Where_tuple_item_closure(async);

            AssertSql(
                """
@predicateTuple_Item2='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @predicateTuple_Item2
""");
        }

        public override async Task Where_named_tuple_item_closure(bool async)
        {
            await base.Where_named_tuple_item_closure(async);

            AssertSql(
                """
@predicateTuple_Item2='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @predicateTuple_Item2
""");
        }

        public override async Task Where_simple_closure_constant(bool async)
        {
            await base.Where_simple_closure_constant(async);

            AssertSql(
                """
@predicate='True'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE @predicate
""");
        }

        public override async Task Where_simple_closure_via_query_cache(bool async)
        {
            await base.Where_simple_closure_via_query_cache(async);

            AssertSql(
                """
@city='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city
""",
                //
                """
@city='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city
""");
        }

        public override async Task Where_method_call_nullable_type_closure_via_query_cache(bool async)
        {
            await base.Where_method_call_nullable_type_closure_via_query_cache(async);

            AssertSql(
                """
@p='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
                //
                """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""");
        }

        public override async Task Where_method_call_nullable_type_reverse_closure_via_query_cache(bool async)
        {
            await base.Where_method_call_nullable_type_reverse_closure_via_query_cache(async);

            AssertSql(
                """
@p='1' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`EmployeeID` AS signed) > @p
""",
                //
                """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`EmployeeID` AS signed) > @p
""");
        }

        public override async Task Where_method_call_closure_via_query_cache(bool async)
        {
            await base.Where_method_call_closure_via_query_cache(async);

            AssertSql(
                """
@GetCity='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @GetCity
""",
                //
                """
@GetCity='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @GetCity
""");
        }

        public override async Task Where_field_access_closure_via_query_cache(bool async)
        {
            await base.Where_field_access_closure_via_query_cache(async);

            AssertSql(
                """
@city_InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstanceFieldValue
""",
                //
                """
@city_InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstanceFieldValue
""");
        }

        public override async Task Where_property_access_closure_via_query_cache(bool async)
        {
            await base.Where_property_access_closure_via_query_cache(async);

            AssertSql(
                """
@city_InstancePropertyValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstancePropertyValue
""",
                //
                """
@city_InstancePropertyValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstancePropertyValue
""");
        }

        public override async Task Where_static_field_access_closure_via_query_cache(bool async)
        {
            await base.Where_static_field_access_closure_via_query_cache(async);

            AssertSql(
                """
@StaticFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticFieldValue
""",
                //
                """
@StaticFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticFieldValue
""");
        }

        public override async Task Where_static_property_access_closure_via_query_cache(bool async)
        {
            await base.Where_static_property_access_closure_via_query_cache(async);

            AssertSql(
                """
@StaticPropertyValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticPropertyValue
""",
                //
                """
@StaticPropertyValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticPropertyValue
""");
        }

        public override async Task Where_nested_field_access_closure_via_query_cache(bool async)
        {
            await base.Where_nested_field_access_closure_via_query_cache(async);

            AssertSql(
                """
@city_Nested_InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstanceFieldValue
""",
                //
                """
@city_Nested_InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstanceFieldValue
""");
        }

        public override async Task Where_nested_property_access_closure_via_query_cache(bool async)
        {
            await base.Where_nested_property_access_closure_via_query_cache(async);

            AssertSql(
                """
@city_Nested_InstancePropertyValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstancePropertyValue
""",
                //
                """
@city_Nested_InstancePropertyValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstancePropertyValue
""");
        }

        public override async Task Where_nested_field_access_closure_via_query_cache_error_null(bool async)
        {
            await base.Where_nested_field_access_closure_via_query_cache_error_null(async);

            AssertSql();
        }

        public override async Task Where_nested_field_access_closure_via_query_cache_error_method_null(bool async)
        {
            await base.Where_nested_field_access_closure_via_query_cache_error_method_null(async);

            AssertSql();
        }

        public override async Task Where_new_instance_field_access_query_cache(bool async)
        {
            await base.Where_new_instance_field_access_query_cache(async);

            AssertSql(
                """
@InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""",
                //
                """
@InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""");
        }

        public override async Task Where_new_instance_field_access_closure_via_query_cache(bool async)
        {
            await base.Where_new_instance_field_access_closure_via_query_cache(async);

            AssertSql(
                """
@InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""",
                //
                """
@InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""");
        }

        public override async Task Where_simple_closure_via_query_cache_nullable_type(bool async)
        {
            await base.Where_simple_closure_via_query_cache_nullable_type(async);

            AssertSql(
                """
@p='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
                //
                """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
                //
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""");
        }

        public override async Task Where_simple_closure_via_query_cache_nullable_type_reverse(bool async)
        {
            await base.Where_simple_closure_via_query_cache_nullable_type_reverse(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""",
                //
                """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
                //
                """
@p='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""");
        }

        public override async Task Where_subquery_closure_via_query_cache(bool async)
        {
            await base.Where_subquery_closure_via_query_cache(async);

            AssertSql(
                """
@customerID='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`CustomerID` = @customerID) AND (`o`.`CustomerID` = `c`.`CustomerID`))
""",
                //
                """
@customerID='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`CustomerID` = @customerID) AND (`o`.`CustomerID` = `c`.`CustomerID`))
""");
        }

        public override async Task Where_simple_shadow(bool async)
        {
            await base.Where_simple_shadow(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`Title` = 'Sales Representative'
""");
        }

        public override async Task Where_simple_shadow_projection(bool async)
        {
            await base.Where_simple_shadow_projection(async);

            AssertSql(
                """
SELECT `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`Title` = 'Sales Representative'
""");
        }

        public override async Task Where_simple_shadow_projection_mixed(bool async)
        {
            await base.Where_simple_shadow_projection_mixed(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`Title` = 'Sales Representative'
""");
        }

        public override async Task Where_simple_shadow_subquery(bool async)
        {
            await base.Where_simple_shadow_subquery(async);

            AssertSql(
                """
@p='5'

SELECT `e0`.`EmployeeID`, `e0`.`City`, `e0`.`Country`, `e0`.`FirstName`, `e0`.`ReportsTo`, `e0`.`Title`
FROM (
    SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
    FROM `Employees` AS `e`
    ORDER BY `e`.`EmployeeID`
    LIMIT @p
) AS `e0`
WHERE `e0`.`Title` = 'Sales Representative'
ORDER BY `e0`.`EmployeeID`
""");
        }

        public override async Task Where_shadow_subquery_FirstOrDefault(bool async)
        {
            await base.Where_shadow_subquery_FirstOrDefault(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE (`e`.`Title` = (
    SELECT `e0`.`Title`
    FROM `Employees` AS `e0`
    ORDER BY `e0`.`Title`
    LIMIT 1)) OR (`e`.`Title` IS NULL AND ((
    SELECT `e0`.`Title`
    FROM `Employees` AS `e0`
    ORDER BY `e0`.`Title`
    LIMIT 1) IS NULL))
""");
        }

        public override async Task Where_client(bool async)
        {
            await base.Where_client(async);

            AssertSql();
        }

        public override async Task Where_subquery_correlated(bool async)
        {
            await base.Where_subquery_correlated(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Customers` AS `c0`
    WHERE `c`.`CustomerID` = `c0`.`CustomerID`)
""");
        }

        public override async Task Where_subquery_correlated_client_eval(bool async)
        {
            await base.Where_subquery_correlated_client_eval(async);

            AssertSql();
        }

        public override async Task Where_client_and_server_top_level(bool async)
        {
            await base.Where_client_and_server_top_level(async);

            AssertSql();
        }

        public override async Task Where_client_or_server_top_level(bool async)
        {
            await base.Where_client_or_server_top_level(async);

            AssertSql();
        }

        public override async Task Where_client_and_server_non_top_level(bool async)
        {
            await base.Where_client_and_server_non_top_level(async);

            AssertSql();
        }

        public override async Task Where_client_deep_inside_predicate_and_server_top_level(bool async)
        {
            await base.Where_client_deep_inside_predicate_and_server_top_level(async);

            AssertSql();
        }

        public override async Task Where_equals_method_int(bool async)
        {
            await base.Where_equals_method_int(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`EmployeeID` = 1
""");
        }

        public override async Task Where_equals_using_object_overload_on_mismatched_types(bool async)
        {
            await base.Where_equals_using_object_overload_on_mismatched_types(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""");
        }

        public override async Task Where_equals_using_int_overload_on_mismatched_types(bool async)
        {
            await base.Where_equals_using_int_overload_on_mismatched_types(async);

            AssertSql(
                """
@p='1'

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`EmployeeID` = @p
""");
        }

        public override async Task Where_equals_on_mismatched_types_nullable_int_long(bool async)
        {
            await base.Where_equals_on_mismatched_types_nullable_int_long(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""",
                //
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""");
        }

        public override async Task Where_equals_on_mismatched_types_int_nullable_int(bool async)
        {
            await base.Where_equals_on_mismatched_types_int_nullable_int(async);

            AssertSql(
                """
@intPrm='2'

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` = @intPrm
""",
                //
                """
@intPrm='2'

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE @intPrm = `e`.`ReportsTo`
""");
        }

        public override async Task Where_equals_on_mismatched_types_nullable_long_nullable_int(bool async)
        {
            await base.Where_equals_on_mismatched_types_nullable_long_nullable_int(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""",
                //
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""");
        }

        public override async Task Where_equals_on_matched_nullable_int_types(bool async)
        {
            await base.Where_equals_on_matched_nullable_int_types(async);

            AssertSql(
                """
@nullableIntPrm='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE @nullableIntPrm = `e`.`ReportsTo`
""",
                //
                """
@nullableIntPrm='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` = @nullableIntPrm
""");
        }

        public override async Task Where_equals_on_null_nullable_int_types(bool async)
        {
            await base.Where_equals_on_null_nullable_int_types(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""",
                //
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""");
        }

        public override async Task Where_comparison_nullable_type_not_null(bool async)
        {
            await base.Where_comparison_nullable_type_not_null(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` = 2
""");
        }

        public override async Task Where_comparison_nullable_type_null(bool async)
        {
            await base.Where_comparison_nullable_type_null(async);

            AssertSql(
                """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""");
        }

        public override async Task Where_simple_reversed(bool async)
        {
            await base.Where_simple_reversed(async);

            AssertSql(
                """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE 'London' = `c`.`City`
""");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
