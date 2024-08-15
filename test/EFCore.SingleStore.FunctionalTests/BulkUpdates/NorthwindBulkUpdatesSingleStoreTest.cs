using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.BulkUpdates;

public class NorthwindBulkUpdatesSingleStoreTest : NorthwindBulkUpdatesTestBase<NorthwindBulkUpdatesSingleStoreFixture<NoopModelCustomizer>>
{
    public NorthwindBulkUpdatesSingleStoreTest(
        NorthwindBulkUpdatesSingleStoreFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        ClearLog();
        // Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Delete_Where_TagWith(bool async)
    {
        await base.Delete_Where_TagWith(async);

        AssertSql(
"""
-- MyDelete

DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < 10300
""");
    }

    public override async Task Delete_Where(bool async)
    {
        await base.Delete_Where(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < 10300
""");
    }

    public override async Task Delete_Where_parameter(bool async)
    {
        await base.Delete_Where_parameter(async);

        AssertSql(
"""
@__quantity_0='1' (Nullable = true) (DbType = Int16)

DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`Quantity` = @__quantity_0
""",
                //
                """
DELETE `o`
FROM `Order Details` AS `o`
WHERE FALSE
""");
    }

    [ConditionalTheory(Skip = "SingleStore doesn't support DELETE statement with ORDER BY in it")]
    public override async Task Delete_Where_OrderBy(bool async)
    {
        await base.Delete_Where_OrderBy(async);

        AssertSql(
"""
DELETE
FROM `Order Details`
WHERE `OrderID` < 10300
ORDER BY `OrderID`
""");
    }

    public override async Task Delete_Where_OrderBy_Skip(bool async)
    {
        await base.Delete_Where_OrderBy_Skip(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        ORDER BY `o0`.`OrderID`
        LIMIT 18446744073709551610 OFFSET @__p_0
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    [ConditionalTheory(Skip = "SingleStore doesn't support DELETE statement with ORDER BY in it")]
    public override async Task Delete_Where_OrderBy_Take(bool async)
    {
        await base.Delete_Where_OrderBy_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE
FROM `Order Details`
WHERE `OrderID` < 10300
ORDER BY `OrderID`
LIMIT @__p_0
""");
    }

    public override async Task Delete_Where_OrderBy_Skip_Take(bool async)
    {
        await base.Delete_Where_OrderBy_Skip_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        ORDER BY `o0`.`OrderID`
        LIMIT @__p_0 OFFSET @__p_0
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_Skip(bool async)
    {
        await base.Delete_Where_Skip(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        LIMIT 18446744073709551610 OFFSET @__p_0
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_Take(bool async)
    {
        await base.Delete_Where_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE
FROM `Order Details`
WHERE `OrderID` < 10300
LIMIT @__p_0
""");
    }

    public override async Task Delete_Where_Skip_Take(bool async)
    {
        await base.Delete_Where_Skip_Take(async);

        AssertSql(
"""
@__p_0='100'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10300
        LIMIT @__p_0 OFFSET @__p_0
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    [ConditionalTheory(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
    public override async Task Delete_Where_predicate_with_GroupBy_aggregate(bool async)
    {
        await base.Delete_Where_predicate_with_GroupBy_aggregate(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < (
    SELECT (
        SELECT `o1`.`OrderID`
        FROM `Orders` AS `o1`
        WHERE (`o0`.`CustomerID` = `o1`.`CustomerID`) OR (`o0`.`CustomerID` IS NULL AND (`o1`.`CustomerID` IS NULL))
        LIMIT 1)
    FROM `Orders` AS `o0`
    GROUP BY `o0`.`CustomerID`
    HAVING COUNT(*) > 11
    LIMIT 1)
""");
    }

    [ConditionalTheory(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
    public override async Task Delete_Where_predicate_with_GroupBy_aggregate_2(bool async)
    {
        await base.Delete_Where_predicate_with_GroupBy_aggregate_2(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o1`
    GROUP BY `o1`.`CustomerID`
    HAVING (COUNT(*) > 9) AND ((
        SELECT `o2`.`OrderID`
        FROM `Orders` AS `o2`
        WHERE (`o1`.`CustomerID` = `o2`.`CustomerID`) OR (`o1`.`CustomerID` IS NULL AND (`o2`.`CustomerID` IS NULL))
        LIMIT 1) = `o0`.`OrderID`))
""");
    }

    public override async Task Delete_GroupBy_Where_Select(bool async)
    {
        await base.Delete_GroupBy_Where_Select(async);

        AssertSql();
    }

    public override async Task Delete_GroupBy_Where_Select_2(bool async)
    {
        await base.Delete_GroupBy_Where_Select_2(async);

        AssertSql();
    }

    public override async Task Delete_Where_Skip_Take_Skip_Take_causing_subquery(bool async)
    {
        await base.Delete_Where_Skip_Take_Skip_Take_causing_subquery(async);

        AssertSql(
"""
@__p_0='100'
@__p_2='5'
@__p_1='20'

DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `t`.`OrderID`, `t`.`ProductID`, `t`.`Discount`, `t`.`Quantity`, `t`.`UnitPrice`
        FROM (
            SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
            FROM `Order Details` AS `o0`
            WHERE `o0`.`OrderID` < 10300
            LIMIT @__p_0 OFFSET @__p_0
        ) AS `t`
        LIMIT @__p_2 OFFSET @__p_1
    ) AS `t0`
    WHERE (`t0`.`OrderID` = `o`.`OrderID`) AND (`t0`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_Distinct(bool async)
    {
        await base.Delete_Where_Distinct(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE `o`.`OrderID` < 10300
""");
    }

    [ConditionalTheory(Skip = "Operation 'Update/Delete right table of a join' is not allowed.")]
    public override async Task Delete_SelectMany(bool async)
    {
        await base.Delete_SelectMany(async);

        AssertSql(
"""
DELETE `o0`
FROM `Orders` AS `o`
INNER JOIN `Order Details` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `o`.`OrderID` < 10250
""");
    }

    public override async Task Delete_SelectMany_subquery(bool async)
    {
        await base.Delete_SelectMany_subquery(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o0`
    INNER JOIN (
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`ProductID` > 0
    ) AS `t` ON `o0`.`OrderID` = `t`.`OrderID`
    WHERE (`o0`.`OrderID` < 10250) AND ((`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`)))
""");
    }

    public override async Task Delete_Where_using_navigation(bool async)
    {
        await base.Delete_Where_using_navigation(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE EXTRACT(year FROM `o0`.`OrderDate`) = 2000
""");
    }

    public override async Task Delete_Where_using_navigation_2(bool async)
    {
        await base.Delete_Where_using_navigation_2(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
LEFT JOIN `Customers` AS `c` ON `o0`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`CustomerID` IS NOT NULL AND (`c`.`CustomerID` LIKE 'F%')
""");
    }

    public override async Task Delete_Union(bool async)
    {
        await base.Delete_Union(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        UNION
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Concat(bool async)
    {
        await base.Delete_Concat(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        UNION ALL
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Intersect(bool async)
    {
        await base.Delete_Intersect(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        INTERSECT
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Except(bool async)
    {
        await base.Delete_Except(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`, `o0`.`Discount`, `o0`.`Quantity`, `o0`.`UnitPrice`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 10250
        EXCEPT
        SELECT `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
        FROM `Order Details` AS `o1`
        WHERE `o1`.`OrderID` > 11250
    ) AS `t`
    WHERE (`t`.`OrderID` = `o`.`OrderID`) AND (`t`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_non_entity_projection(bool async)
    {
        await base.Delete_non_entity_projection(async);

        AssertSql();
    }

    public override async Task Delete_non_entity_projection_2(bool async)
    {
        await base.Delete_non_entity_projection_2(async);

        AssertSql();
    }

    public override async Task Delete_non_entity_projection_3(bool async)
    {
        await base.Delete_non_entity_projection_3(async);

        AssertSql();
    }

    public override async Task Delete_FromSql_converted_to_subquery(bool async)
    {
        await base.Delete_FromSql_converted_to_subquery(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `OrderID`, `ProductID`, `UnitPrice`, `Quantity`, `Discount`
        FROM `Order Details`
        WHERE `OrderID` < 10300
    ) AS `m`
    WHERE (`m`.`OrderID` = `o`.`OrderID`) AND (`m`.`ProductID` = `o`.`ProductID`))
""");
    }

    public override async Task Delete_Where_optional_navigation_predicate(bool async)
    {
        await base.Delete_Where_optional_navigation_predicate(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
LEFT JOIN `Customers` AS `c` ON `o0`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`City` IS NOT NULL AND (`c`.`City` LIKE 'Se%')
""");
    }

    public override async Task Delete_with_join(bool async)
    {
        await base.Delete_with_join(async);

        AssertSql(
"""
@__p_1='100'
@__p_0='0'

DELETE `o`
FROM `Order Details` AS `o`
INNER JOIN (
    SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < 10300
    ORDER BY `o0`.`OrderID`
    LIMIT @__p_1 OFFSET @__p_0
) AS `t` ON `o`.`OrderID` = `t`.`OrderID`
""");
    }

    public override async Task Delete_with_left_join(bool async)
    {
        await base.Delete_with_left_join(async);

        AssertSql(
"""
@__p_1='100'
@__p_0='0'

DELETE `o`
FROM `Order Details` AS `o`
LEFT JOIN (
    SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < 10300
    ORDER BY `o0`.`OrderID`
    LIMIT @__p_1 OFFSET @__p_0
) AS `t` ON `o`.`OrderID` = `t`.`OrderID`
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Delete_with_cross_join(bool async)
    {
        await base.Delete_with_cross_join(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
CROSS JOIN (
    SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < 10300
    ORDER BY `o0`.`OrderID`
    LIMIT 100 OFFSET 0
) AS `t`
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Delete_with_cross_apply(bool async)
    {
        await base.Delete_with_cross_apply(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
JOIN LATERAL (
    SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < `o`.`OrderID`
    ORDER BY `o0`.`OrderID`
    LIMIT 100 OFFSET 0
) AS `t` ON TRUE
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Delete_with_outer_apply(bool async)
    {
        await base.Delete_with_outer_apply(async);

        AssertSql(
"""
DELETE `o`
FROM `Order Details` AS `o`
LEFT JOIN LATERAL (
    SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
    FROM `Orders` AS `o0`
    WHERE `o0`.`OrderID` < `o`.`OrderID`
    ORDER BY `o0`.`OrderID`
    LIMIT 100 OFFSET 0
) AS `t` ON TRUE
WHERE `o`.`OrderID` < 10276
""");
    }

    public override async Task Update_Where_set_constant_TagWith(bool async)
    {
        await base.Update_Where_set_constant_TagWith(async);

        AssertExecuteUpdateSql(
"""
-- MyUpdate

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_constant(bool async)
    {
        await base.Update_Where_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_parameter_set_constant(bool async)
    {
        await base.Update_Where_parameter_set_constant(async);

        AssertExecuteUpdateSql(
            """
@__customer_0='ALFKI' (Size = 255)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` = @__customer_0
""",
            //
            """
@__customer_0='ALFKI' (Size = 255)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @__customer_0
""",
            //
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""",
            //
            """
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE FALSE
""");
    }

    public override async Task Update_Where_set_parameter(bool async)
    {
        await base.Update_Where_set_parameter(async);

        AssertExecuteUpdateSql(
"""
@__value_0='Abc' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__value_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_parameter_from_closure_array(bool async)
    {
        await base.Update_Where_set_parameter_from_closure_array(async);

        AssertExecuteUpdateSql(
"""
@__p_0='Abc' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__p_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_parameter_from_inline_list(bool async)
    {
        await base.Update_Where_set_parameter_from_inline_list(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Abc'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_parameter_from_multilevel_property_access(bool async)
    {
        await base.Update_Where_set_parameter_from_multilevel_property_access(async);

        AssertExecuteUpdateSql(
"""
@__container_Containee_Property_0='Abc' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__container_Containee_Property_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    [ConditionalTheory(Skip = "Feature 'UPDATE...LIMIT must be constrained to a single partition' is not supported by SingleStore.")]
    public override async Task Update_Where_Skip_set_constant(bool async)
    {
        await base.Update_Where_Skip_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    LIMIT 18446744073709551610 OFFSET @__p_0
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    [ConditionalTheory(Skip = "Feature 'UPDATE...LIMIT must be constrained to a single partition' is not supported by SingleStore")]
    public override async Task Update_Where_Take_set_constant(bool async)
    {
        await AssertUpdate(
            async,
            ss => ss.Set<Customer>().Where(c => c.CustomerID.StartsWith("F")).Take(4),
            e => e,
            s => s.SetProperty(c => c.ContactName, "Updated"),
            rowsAffectedCount: 4,
            (b, a) => Assert.All(a, c => Assert.Equal("Updated", c.ContactName)));

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
LIMIT @__p_0
""");
    }

    public override async Task Update_Where_Skip_Take_set_constant(bool async)
    {
        await base.Update_Where_Skip_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_1='4'
@__p_0='2'

UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    LIMIT @__p_1 OFFSET @__p_0
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Skip_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Skip_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    ORDER BY `c0`.`City`
    LIMIT 18446744073709551610 OFFSET @__p_0
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Take_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_0='4'

UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    ORDER BY `c0`.`City`
    LIMIT @__p_0
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Skip_Take_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Skip_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_1='4'
@__p_0='2'

UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    ORDER BY `c0`.`City`
    LIMIT @__p_1 OFFSET @__p_0
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_OrderBy_Skip_Take_Skip_Take_set_constant(bool async)
    {
        await base.Update_Where_OrderBy_Skip_Take_Skip_Take_set_constant(async);

        AssertExecuteUpdateSql(
"""
@__p_1='6'
@__p_0='2'

UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `t`.`CustomerID`, `t`.`Address`, `t`.`City`, `t`.`CompanyName`, `t`.`ContactName`, `t`.`ContactTitle`, `t`.`Country`, `t`.`Fax`, `t`.`Phone`, `t`.`PostalCode`, `t`.`Region`
    FROM (
        SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
        FROM `Customers` AS `c0`
        WHERE `c0`.`CustomerID` LIKE 'F%'
        ORDER BY `c0`.`City`
        LIMIT @__p_1 OFFSET @__p_0
    ) AS `t`
    ORDER BY `t`.`City`
    LIMIT @__p_0 OFFSET @__p_0
) AS `t0` ON `c`.`CustomerID` = `t0`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Where_GroupBy_aggregate_set_constant(bool async)
    {
        await base.Update_Where_GroupBy_aggregate_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` = (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
    HAVING COUNT(*) > 11
    LIMIT 1)
""");
    }

    [ConditionalTheory(Skip = "SingleStore does not support this type of query: nested scalar subselects.")]
    public override async Task Update_Where_GroupBy_First_set_constant(bool async)
    {
        await base.Update_Where_GroupBy_First_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` = (
    SELECT (
        SELECT `o0`.`CustomerID`
        FROM `Orders` AS `o0`
        WHERE (`o`.`CustomerID` = `o0`.`CustomerID`) OR (`o`.`CustomerID` IS NULL AND (`o0`.`CustomerID` IS NULL))
        LIMIT 1)
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
    HAVING COUNT(*) > 11
    LIMIT 1)
""");
    }

    public override async Task Update_Where_GroupBy_First_set_constant_2(bool async)
    {
        await base.Update_Where_GroupBy_First_set_constant_2(async);

        AssertExecuteUpdateSql();
    }

    [ConditionalTheory(Skip = "SingleStore does not support this type of query: unsupported nested scalar subselects")]
    public override async Task Update_Where_GroupBy_First_set_constant_3(bool async)
    {
        await base.Update_Where_GroupBy_First_set_constant_3(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    GROUP BY `o`.`CustomerID`
    HAVING (COUNT(*) > 11) AND ((
        SELECT `c0`.`CustomerID`
        FROM `Orders` AS `o0`
        LEFT JOIN `Customers` AS `c0` ON `o0`.`CustomerID` = `c0`.`CustomerID`
        WHERE (`o`.`CustomerID` = `o0`.`CustomerID`) OR (`o`.`CustomerID` IS NULL AND (`o0`.`CustomerID` IS NULL))
        LIMIT 1) = `c`.`CustomerID`))
""");
    }

    public override async Task Update_Where_Distinct_set_constant(bool async)
    {
        await base.Update_Where_Distinct_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_using_navigation_set_null(bool async)
    {
        await base.Update_Where_using_navigation_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
SET `o`.`OrderDate` = NULL
WHERE `c`.`City` = 'Seattle'
""");
    }

    public override async Task Update_Where_using_navigation_2_set_constant(bool async)
    {
        await base.Update_Where_using_navigation_2_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
LEFT JOIN `Customers` AS `c` ON `o0`.`CustomerID` = `c`.`CustomerID`
SET `o`.`Quantity` = CAST(1 AS signed)
WHERE `c`.`City` = 'Seattle'
""");
    }

    [ConditionalTheory(Skip = "Operation 'Update/Delete right table of a join' is not allowed")]
    public override async Task Update_Where_SelectMany_set_null(bool async)
    {
        await base.Update_Where_SelectMany_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN `Orders` AS `o` ON `c`.`CustomerID` = `o`.`CustomerID`
SET `o`.`OrderDate` = NULL
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_property_plus_constant(bool async)
    {
        await base.Update_Where_set_property_plus_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = CONCAT(COALESCE(`c`.`ContactName`, ''), 'Abc')
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_property_plus_parameter(bool async)
    {
        await base.Update_Where_set_property_plus_parameter(async);

        AssertExecuteUpdateSql(
"""
@__value_0='Abc' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = CONCAT(COALESCE(`c`.`ContactName`, ''), @__value_0)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_property_plus_property(bool async)
    {
        await base.Update_Where_set_property_plus_property(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = CONCAT(COALESCE(`c`.`ContactName`, ''), `c`.`CustomerID`)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_constant_using_ef_property(bool async)
    {
        await base.Update_Where_set_constant_using_ef_property(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_set_null(bool async)
    {
        await base.Update_Where_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`ContactName` = NULL
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_without_property_to_set_throws(bool async)
    {
        await base.Update_without_property_to_set_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_with_invalid_lambda_throws(bool async)
    {
        await base.Update_with_invalid_lambda_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Where_multiple_set(bool async)
    {
        await base.Update_Where_multiple_set(async);

        AssertExecuteUpdateSql(
"""
@__value_0='Abc' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`City` = 'Seattle',
    `c`.`ContactName` = @__value_0
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_invalid_lambda_in_set_property_throws(bool async)
    {
        await base.Update_with_invalid_lambda_in_set_property_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_multiple_entity_throws(bool async)
    {
        await base.Update_multiple_entity_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_unmapped_property_throws(bool async)
    {
        await base.Update_unmapped_property_throws(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Union_set_constant(bool async)
    {
        await base.Update_Union_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    UNION
    SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
    FROM `Customers` AS `c1`
    WHERE `c1`.`CustomerID` LIKE 'A%'
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Concat_set_constant(bool async)
    {
        await base.Update_Concat_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    UNION ALL
    SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
    FROM `Customers` AS `c1`
    WHERE `c1`.`CustomerID` LIKE 'A%'
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Except_set_constant(bool async)
    {
        await base.Update_Except_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    EXCEPT
    SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
    FROM `Customers` AS `c1`
    WHERE `c1`.`CustomerID` LIKE 'A%'
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_Intersect_set_constant(bool async)
    {
        await base.Update_Intersect_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` LIKE 'F%'
    INTERSECT
    SELECT `c1`.`CustomerID`, `c1`.`Address`, `c1`.`City`, `c1`.`CompanyName`, `c1`.`ContactName`, `c1`.`ContactTitle`, `c1`.`Country`, `c1`.`Fax`, `c1`.`Phone`, `c1`.`PostalCode`, `c1`.`Region`
    FROM `Customers` AS `c1`
    WHERE `c1`.`CustomerID` LIKE 'A%'
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
""");
    }

    public override async Task Update_with_join_set_constant(bool async)
    {
        await base.Update_with_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
INNER JOIN (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_left_join_set_constant(bool async)
    {
        await base.Update_with_left_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
LEFT JOIN (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_set_constant(bool async)
    {
        await base.Update_with_cross_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `t`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_apply_set_constant(bool async)
    {
        await base.Update_with_cross_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
JOIN LATERAL (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `t` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_outer_apply_set_constant(bool async)
    {
        await base.Update_with_outer_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
LEFT JOIN LATERAL (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `t` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_left_join_set_constant(bool async)
    {
        await base.Update_with_cross_join_left_join_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` IS NOT NULL AND (`c0`.`City` LIKE 'S%')
) AS `t`
LEFT JOIN (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `t0` ON `c`.`CustomerID` = `t0`.`CustomerID`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_cross_apply_set_constant(bool async)
    {
        await base.Update_with_cross_join_cross_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` IS NOT NULL AND (`c0`.`City` LIKE 'S%')
) AS `t`
JOIN LATERAL (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `t0` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_with_cross_join_outer_apply_set_constant(bool async)
    {
        await base.Update_with_cross_join_outer_apply_set_constant(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`City` IS NOT NULL AND (`c0`.`City` LIKE 'S%')
) AS `t`
LEFT JOIN LATERAL (
    SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE (`o`.`OrderID` < 10300) AND (EXTRACT(year FROM `o`.`OrderDate`) < CHAR_LENGTH(`c`.`ContactName`))
) AS `t0` ON TRUE
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_FromSql_set_constant(bool async)
    {
        await base.Update_FromSql_set_constant(async);

        AssertExecuteUpdateSql();
    }

    public override async Task Update_Where_SelectMany_subquery_set_null(bool async)
    {
        await base.Update_Where_SelectMany_subquery_set_null(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Orders` AS `o`
INNER JOIN (
    SELECT `t`.`OrderID`, `t`.`CustomerID`, `t`.`EmployeeID`, `t`.`OrderDate`, `c`.`CustomerID` AS `CustomerID0`
    FROM `Customers` AS `c`
    INNER JOIN (
        SELECT `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
        FROM `Orders` AS `o0`
        WHERE EXTRACT(year FROM `o0`.`OrderDate`) = 1997
    ) AS `t` ON `c`.`CustomerID` = `t`.`CustomerID`
    WHERE `c`.`CustomerID` LIKE 'F%'
) AS `t0` ON `o`.`OrderID` = `t0`.`OrderID`
SET `o`.`OrderDate` = NULL
""");
    }

    [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
    public override async Task Update_Where_Join_set_property_from_joined_single_result_table(bool async)
    {
        await base.Update_Where_Join_set_property_from_joined_single_result_table(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`City` = CAST(EXTRACT(year FROM (
    SELECT `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    ORDER BY `o`.`OrderDate` DESC
    LIMIT 1)) AS char)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    public override async Task Update_Where_Join_set_property_from_joined_table(bool async)
    {
        await base.Update_Where_Join_set_property_from_joined_table(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
CROSS JOIN (
    SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
    FROM `Customers` AS `c0`
    WHERE `c0`.`CustomerID` = 'ALFKI'
) AS `t`
SET `c`.`City` = `t`.`City`
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
    public override async Task Update_Where_Join_set_property_from_joined_single_result_scalar(bool async)
    {
        await base.Update_Where_Join_set_property_from_joined_single_result_scalar(async);

        AssertExecuteUpdateSql(
"""
UPDATE `Customers` AS `c`
SET `c`.`City` = CAST(EXTRACT(year FROM (
    SELECT `o`.`OrderDate`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    ORDER BY `o`.`OrderDate` DESC
    LIMIT 1)) AS char)
WHERE `c`.`CustomerID` LIKE 'F%'
""");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}