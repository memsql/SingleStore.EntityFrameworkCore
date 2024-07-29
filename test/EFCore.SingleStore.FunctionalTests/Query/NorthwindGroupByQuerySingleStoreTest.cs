﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindGroupByQuerySingleStoreTest : NorthwindGroupByQueryRelationalTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindGroupByQuerySingleStoreTest(
            NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task GroupBy_aggregate_Contains(bool async)
        {
            return base.GroupBy_aggregate_Contains(async);
        }

        [ConditionalTheory(Skip = "Feature 'Dependent aggregate inside subselect' is not supported by SingleStore.")]
        public override Task GroupBy_aggregate_from_multiple_query_in_same_projection_2(bool async)
        {
            return base.GroupBy_aggregate_from_multiple_query_in_same_projection_2(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task GroupBy_aggregate_from_multiple_query_in_same_projection_3(bool async)
        {
            return base.GroupBy_aggregate_from_multiple_query_in_same_projection_3(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task GroupBy_scalar_subquery(bool async)
        {
            return base.GroupBy_scalar_subquery(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task GroupBy_Shadow(bool async)
        {
            return base.GroupBy_Shadow(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task GroupBy_with_aggregate_containing_complex_where(bool async)
        {
            return base.GroupBy_with_aggregate_containing_complex_where(async);
        }
        public override async Task AsEnumerable_in_subquery_for_GroupBy(bool async)
        {
            await base.AsEnumerable_in_subquery_for_GroupBy(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `t2`.`OrderID`, `t2`.`CustomerID`, `t2`.`EmployeeID`, `t2`.`OrderDate`, `t2`.`CustomerID0`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `t0`.`OrderID`, `t0`.`CustomerID`, `t0`.`EmployeeID`, `t0`.`OrderDate`, `t`.`CustomerID` AS `CustomerID0`
    FROM (
        SELECT `o`.`CustomerID`
        FROM `Orders` AS `o`
        WHERE `o`.`CustomerID` = `c`.`CustomerID`
        GROUP BY `o`.`CustomerID`
    ) AS `t`
    LEFT JOIN (
        SELECT `t1`.`OrderID`, `t1`.`CustomerID`, `t1`.`EmployeeID`, `t1`.`OrderDate`
        FROM (
            SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`, ROW_NUMBER() OVER(PARTITION BY `o0`.`CustomerID` ORDER BY `o0`.`OrderDate` DESC) AS `row`
            FROM `Orders` AS `o0`
            WHERE `o0`.`CustomerID` = `c`.`CustomerID`
        ) AS `t1`
        WHERE `t1`.`row` <= 1
    ) AS `t0` ON `t`.`CustomerID` = `t0`.`CustomerID`
) AS `t2` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`, `t2`.`CustomerID0`");
        }

        public override async Task Complex_query_with_groupBy_in_subquery1(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery1(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `t`.`Sum`, `t`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `t` ON TRUE
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Complex_query_with_groupBy_in_subquery2(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery2(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `t`.`Max`, `t`.`Sum`, `t`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    GROUP BY `o`.`CustomerID`
) AS `t` ON TRUE
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Complex_query_with_groupBy_in_subquery3(bool async)
        {
            await base.Complex_query_with_groupBy_in_subquery3(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `t`.`Max`, `t`.`Sum`, `t`.`CustomerID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT MAX(CHAR_LENGTH(`o`.`CustomerID`)) AS `Max`, COALESCE(SUM(`o`.`OrderID`), 0) AS `Sum`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
) AS `t` ON TRUE
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Select_nested_collection_with_groupby(bool async)
        {
            await base.Select_nested_collection_with_groupby(async);

            AssertSql(
                @"SELECT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`), `c`.`CustomerID`, `t`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o0`.`OrderID`
    FROM `Orders` AS `o0`
    WHERE `c`.`CustomerID` = `o0`.`CustomerID`
    GROUP BY `o0`.`OrderID`
) AS `t` ON TRUE
WHERE `c`.`CustomerID` LIKE 'F%'
ORDER BY `c`.`CustomerID`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override async Task GroupBy_group_Distinct_Select_Distinct_aggregate(bool async)
        {
            await base.GroupBy_group_Distinct_Select_Distinct_aggregate(async);

            AssertSql(
                @"SELECT `o`.`CustomerID` AS `Key`, MAX(DISTINCT (`o`.`OrderDate`)) AS `Max`
FROM `Orders` AS `o`
GROUP BY `o`.`CustomerID`");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task GroupBy_Count_in_projection(bool async)
        {
            return base.GroupBy_Count_in_projection(async);
        }

        [SupportedServerVersionCondition("8.0.22-mysql", "0.0.0-mariadb")]
        public override Task GroupBy_group_Where_Select_Distinct_aggregate(bool async)
        {
            // See https://github.com/mysql-net/SingleStoreConnector/issues/898.
            return base.GroupBy_group_Where_Select_Distinct_aggregate(async);
        }

        [SupportedServerVersionCondition("8.0.0-mysql", "0.0.0-mariadb")] // Is an issue issue in MySQL 5.7.34, but not in 8.0.25.
        public override Task GroupBy_constant_with_where_on_grouping_with_aggregate_operators(bool async)
        {
            // See https://github.com/mysql-net/SingleStoreConnector/issues/980.
            return base.GroupBy_constant_with_where_on_grouping_with_aggregate_operators(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
