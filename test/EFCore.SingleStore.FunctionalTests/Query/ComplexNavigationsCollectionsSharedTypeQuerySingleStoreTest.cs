using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class ComplexNavigationsCollectionsSharedTypeQuerySingleStoreTest : ComplexNavigationsCollectionsSharedTypeQueryRelationalTestBase<
        ComplexNavigationsSharedTypeQuerySingleStoreTest.ComplexNavigationsSharedTypeQuerySingleStoreFixture>
    {
        public ComplexNavigationsCollectionsSharedTypeQuerySingleStoreTest(
            ComplexNavigationsSharedTypeQuerySingleStoreTest.ComplexNavigationsSharedTypeQuerySingleStoreFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task SelectMany_with_Include1(bool async)
        {
            await base.SelectMany_with_Include1(async);

            AssertSql(
                @"SELECT `t`.`Id`, `t`.`OneToOne_Required_PK_Date`, `t`.`Level1_Optional_Id`, `t`.`Level1_Required_Id`, `t`.`Level2_Name`, `t`.`OneToMany_Optional_Inverse2Id`, `t`.`OneToMany_Required_Inverse2Id`, `t`.`OneToOne_Optional_PK_Inverse2Id`, `l`.`Id`, `t0`.`Id`, `t0`.`Level2_Optional_Id`, `t0`.`Level2_Required_Id`, `t0`.`Level3_Name`, `t0`.`OneToMany_Optional_Inverse3Id`, `t0`.`OneToMany_Required_Inverse3Id`, `t0`.`OneToOne_Optional_PK_Inverse3Id`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Optional_Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Optional_Inverse2Id`, `l0`.`OneToMany_Required_Inverse2Id`, `l0`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`OneToMany_Optional_Inverse2Id`
LEFT JOIN (
    SELECT `l1`.`Id`, `l1`.`Level2_Optional_Id`, `l1`.`Level2_Required_Id`, `l1`.`Level3_Name`, `l1`.`OneToMany_Optional_Inverse3Id`, `l1`.`OneToMany_Required_Inverse3Id`, `l1`.`OneToOne_Optional_PK_Inverse3Id`
    FROM `Level1` AS `l1`
    WHERE `l1`.`Level2_Required_Id` IS NOT NULL AND (`l1`.`OneToMany_Required_Inverse3Id` IS NOT NULL)
) AS `t0` ON CASE
    WHEN (`t`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`t`.`Level1_Required_Id` IS NOT NULL)) AND `t`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `t`.`Id`
END = `t0`.`OneToMany_Optional_Inverse3Id`
ORDER BY `l`.`Id`, `t`.`Id`");
        }

        public override async Task SelectMany_with_navigation_and_Distinct(bool async)
        {
            await base.SelectMany_with_navigation_and_Distinct(async);

            AssertSql(
                @"SELECT `l`.`Id`, `l`.`Date`, `l`.`Name`, `t`.`Id`, `t0`.`Id`, `t0`.`OneToOne_Required_PK_Date`, `t0`.`Level1_Optional_Id`, `t0`.`Level1_Required_Id`, `t0`.`Level2_Name`, `t0`.`OneToMany_Optional_Inverse2Id`, `t0`.`OneToMany_Required_Inverse2Id`, `t0`.`OneToOne_Optional_PK_Inverse2Id`
FROM `Level1` AS `l`
INNER JOIN (
    SELECT DISTINCT `l0`.`Id`, `l0`.`OneToOne_Required_PK_Date`, `l0`.`Level1_Optional_Id`, `l0`.`Level1_Required_Id`, `l0`.`Level2_Name`, `l0`.`OneToMany_Optional_Inverse2Id`, `l0`.`OneToMany_Required_Inverse2Id`, `l0`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l0`
    WHERE (`l0`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l0`.`Level1_Required_Id` IS NOT NULL)) AND `l0`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t` ON `l`.`Id` = `t`.`OneToMany_Optional_Inverse2Id`
LEFT JOIN (
    SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Optional_Id`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Optional_Inverse2Id`, `l1`.`OneToMany_Required_Inverse2Id`, `l1`.`OneToOne_Optional_PK_Inverse2Id`
    FROM `Level1` AS `l1`
    WHERE (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL
) AS `t0` ON `l`.`Id` = `t0`.`OneToMany_Optional_Inverse2Id`
WHERE (`t`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`t`.`Level1_Required_Id` IS NOT NULL)) AND `t`.`OneToMany_Required_Inverse2Id` IS NOT NULL
ORDER BY `l`.`Id`, `t`.`Id`");
        }

        public override async Task Take_Select_collection_Take(bool async)
        {
            await base.Take_Select_collection_Take(async);

            AssertSql(
                @"@__p_0='1'

SELECT `t`.`Id`, `t`.`Name`, `t0`.`Id`, `t0`.`Name`, `t0`.`Level1Id`, `t0`.`Level2Id`, `t0`.`Id0`, `t0`.`Date`, `t0`.`Name0`, `t0`.`Id1`
FROM (
    SELECT `l`.`Id`, `l`.`Name`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0
) AS `t`
LEFT JOIN LATERAL (
    SELECT CASE
        WHEN (`t1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`t1`.`Level1_Required_Id` IS NOT NULL)) AND `t1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `t1`.`Id`
    END AS `Id`, `t1`.`Level2_Name` AS `Name`, `t1`.`OneToMany_Required_Inverse2Id` AS `Level1Id`, `t1`.`Level1_Required_Id` AS `Level2Id`, `l0`.`Id` AS `Id0`, `l0`.`Date`, `l0`.`Name` AS `Name0`, `t1`.`Id` AS `Id1`, `t1`.`c`
    FROM (
        SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Required_Inverse2Id`, CASE
            WHEN (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l1`.`Id`
        END AS `c`
        FROM `Level1` AS `l1`
        WHERE ((`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL) AND (`t`.`Id` = `l1`.`OneToMany_Required_Inverse2Id`)
        ORDER BY CASE
            WHEN (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l1`.`Id`
        END
        LIMIT 3
    ) AS `t1`
    INNER JOIN `Level1` AS `l0` ON `t1`.`Level1_Required_Id` = `l0`.`Id`
) AS `t0` ON TRUE
ORDER BY `t`.`Id`, `t0`.`c`, `t0`.`Id1`");
        }

        public override async Task Skip_Take_Select_collection_Skip_Take(bool async)
        {
            await base.Skip_Take_Select_collection_Skip_Take(async);

            AssertSql(
                @"@__p_0='1'

SELECT `t`.`Id`, `t`.`Name`, `t0`.`Id`, `t0`.`Name`, `t0`.`Level1Id`, `t0`.`Level2Id`, `t0`.`Id0`, `t0`.`Date`, `t0`.`Name0`, `t0`.`Id1`
FROM (
    SELECT `l`.`Id`, `l`.`Name`
    FROM `Level1` AS `l`
    ORDER BY `l`.`Id`
    LIMIT @__p_0 OFFSET @__p_0
) AS `t`
LEFT JOIN LATERAL (
    SELECT CASE
        WHEN (`t1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`t1`.`Level1_Required_Id` IS NOT NULL)) AND `t1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `t1`.`Id`
    END AS `Id`, `t1`.`Level2_Name` AS `Name`, `t1`.`OneToMany_Required_Inverse2Id` AS `Level1Id`, `t1`.`Level1_Required_Id` AS `Level2Id`, `l0`.`Id` AS `Id0`, `l0`.`Date`, `l0`.`Name` AS `Name0`, `t1`.`Id` AS `Id1`, `t1`.`c`
    FROM (
        SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Required_Inverse2Id`, CASE
            WHEN (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l1`.`Id`
        END AS `c`
        FROM `Level1` AS `l1`
        WHERE ((`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL) AND (`t`.`Id` = `l1`.`OneToMany_Required_Inverse2Id`)
        ORDER BY CASE
            WHEN (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL THEN `l1`.`Id`
        END
        LIMIT 3 OFFSET 1
    ) AS `t1`
    INNER JOIN `Level1` AS `l0` ON `t1`.`Level1_Required_Id` = `l0`.`Id`
) AS `t0` ON TRUE
ORDER BY `t`.`Id`, `t0`.`c`, `t0`.`Id1`");
        }

        public override async Task Skip_Take_on_grouping_element_inside_collection_projection(bool async)
        {
            await base.Skip_Take_on_grouping_element_inside_collection_projection(async);

            AssertSql(
                @"SELECT `l`.`Id`, `t2`.`Date`, `t2`.`Id`, `t2`.`Date0`, `t2`.`Name`
FROM `Level1` AS `l`
LEFT JOIN LATERAL (
    SELECT `t`.`Date`, `t0`.`Id`, `t0`.`Date` AS `Date0`, `t0`.`Name`
    FROM (
        SELECT `l0`.`Date`
        FROM `Level1` AS `l0`
        WHERE (`l0`.`Name` = `l`.`Name`) OR (`l0`.`Name` IS NULL AND (`l`.`Name` IS NULL))
        GROUP BY `l0`.`Date`
    ) AS `t`
    LEFT JOIN (
        SELECT `t1`.`Id`, `t1`.`Date`, `t1`.`Name`
        FROM (
            SELECT `l1`.`Id`, `l1`.`Date`, `l1`.`Name`, ROW_NUMBER() OVER(PARTITION BY `l1`.`Date` ORDER BY `l1`.`Name`) AS `row`
            FROM `Level1` AS `l1`
            WHERE (`l1`.`Name` = `l`.`Name`) OR (`l1`.`Name` IS NULL AND (`l`.`Name` IS NULL))
        ) AS `t1`
        WHERE (1 < `t1`.`row`) AND (`t1`.`row` <= 6)
    ) AS `t0` ON `t`.`Date` = `t0`.`Date`
) AS `t2` ON TRUE
ORDER BY `l`.`Id`, `t2`.`Date`, `t2`.`Date0`, `t2`.`Name`");
        }

        public override async Task Skip_Take_Distinct_on_grouping_element(bool async)
        {
            await base.Skip_Take_Distinct_on_grouping_element(async);

            AssertSql(
                @"SELECT `t`.`Date`, `t0`.`Id`, `t0`.`Date`, `t0`.`Name`
FROM (
    SELECT `l`.`Date`
    FROM `Level1` AS `l`
    GROUP BY `l`.`Date`
) AS `t`
LEFT JOIN LATERAL (
    SELECT DISTINCT `t1`.`Id`, `t1`.`Date`, `t1`.`Name`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Date`, `l0`.`Name`
        FROM `Level1` AS `l0`
        WHERE `t`.`Date` = `l0`.`Date`
        ORDER BY `l0`.`Name`
        LIMIT 5 OFFSET 1
    ) AS `t1`
) AS `t0` ON TRUE
ORDER BY `t`.`Date`");
        }

        public override async Task Skip_Take_on_grouping_element_with_collection_include(bool async)
        {
            await base.Skip_Take_on_grouping_element_with_collection_include(async);

            AssertSql(
                @"SELECT `t`.`Date`, `t1`.`Id`, `t1`.`Date`, `t1`.`Name`, `t1`.`Id0`, `t1`.`OneToOne_Required_PK_Date`, `t1`.`Level1_Optional_Id`, `t1`.`Level1_Required_Id`, `t1`.`Level2_Name`, `t1`.`OneToMany_Optional_Inverse2Id`, `t1`.`OneToMany_Required_Inverse2Id`, `t1`.`OneToOne_Optional_PK_Inverse2Id`
FROM (
    SELECT `l`.`Date`
    FROM `Level1` AS `l`
    GROUP BY `l`.`Date`
) AS `t`
LEFT JOIN LATERAL (
    SELECT `t0`.`Id`, `t0`.`Date`, `t0`.`Name`, `t2`.`Id` AS `Id0`, `t2`.`OneToOne_Required_PK_Date`, `t2`.`Level1_Optional_Id`, `t2`.`Level1_Required_Id`, `t2`.`Level2_Name`, `t2`.`OneToMany_Optional_Inverse2Id`, `t2`.`OneToMany_Required_Inverse2Id`, `t2`.`OneToOne_Optional_PK_Inverse2Id`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Date`, `l0`.`Name`
        FROM `Level1` AS `l0`
        WHERE `t`.`Date` = `l0`.`Date`
        ORDER BY `l0`.`Name`
        LIMIT 5 OFFSET 1
    ) AS `t0`
    LEFT JOIN (
        SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Optional_Id`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Optional_Inverse2Id`, `l1`.`OneToMany_Required_Inverse2Id`, `l1`.`OneToOne_Optional_PK_Inverse2Id`
        FROM `Level1` AS `l1`
        WHERE (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL
    ) AS `t2` ON `t0`.`Id` = `t2`.`OneToMany_Optional_Inverse2Id`
) AS `t1` ON TRUE
ORDER BY `t`.`Date`, `t1`.`Name`, `t1`.`Id`");
        }

        public override async Task Skip_Take_on_grouping_element_with_reference_include(bool async)
        {
            await base.Skip_Take_on_grouping_element_with_reference_include(async);

            AssertSql(
                @"SELECT `t`.`Date`, `t1`.`Id`, `t1`.`Date`, `t1`.`Name`, `t1`.`Id0`, `t1`.`OneToOne_Required_PK_Date`, `t1`.`Level1_Optional_Id`, `t1`.`Level1_Required_Id`, `t1`.`Level2_Name`, `t1`.`OneToMany_Optional_Inverse2Id`, `t1`.`OneToMany_Required_Inverse2Id`, `t1`.`OneToOne_Optional_PK_Inverse2Id`
FROM (
    SELECT `l`.`Date`
    FROM `Level1` AS `l`
    GROUP BY `l`.`Date`
) AS `t`
LEFT JOIN LATERAL (
    SELECT `t0`.`Id`, `t0`.`Date`, `t0`.`Name`, `t2`.`Id` AS `Id0`, `t2`.`OneToOne_Required_PK_Date`, `t2`.`Level1_Optional_Id`, `t2`.`Level1_Required_Id`, `t2`.`Level2_Name`, `t2`.`OneToMany_Optional_Inverse2Id`, `t2`.`OneToMany_Required_Inverse2Id`, `t2`.`OneToOne_Optional_PK_Inverse2Id`
    FROM (
        SELECT `l0`.`Id`, `l0`.`Date`, `l0`.`Name`
        FROM `Level1` AS `l0`
        WHERE `t`.`Date` = `l0`.`Date`
        ORDER BY `l0`.`Name`
        LIMIT 5 OFFSET 1
    ) AS `t0`
    LEFT JOIN (
        SELECT `l1`.`Id`, `l1`.`OneToOne_Required_PK_Date`, `l1`.`Level1_Optional_Id`, `l1`.`Level1_Required_Id`, `l1`.`Level2_Name`, `l1`.`OneToMany_Optional_Inverse2Id`, `l1`.`OneToMany_Required_Inverse2Id`, `l1`.`OneToOne_Optional_PK_Inverse2Id`
        FROM `Level1` AS `l1`
        WHERE (`l1`.`OneToOne_Required_PK_Date` IS NOT NULL AND (`l1`.`Level1_Required_Id` IS NOT NULL)) AND `l1`.`OneToMany_Required_Inverse2Id` IS NOT NULL
    ) AS `t2` ON `t0`.`Id` = `t2`.`Level1_Optional_Id`
) AS `t1` ON TRUE
ORDER BY `t`.`Date`, `t1`.`Name`, `t1`.`Id`");
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        public override Task Filtered_include_context_accessed_inside_filter_correlated(bool async)
        {
            return base.Filtered_include_context_accessed_inside_filter_correlated(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
