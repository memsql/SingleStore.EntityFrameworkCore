using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class EscapesSingleStoreTest : EscapesSingleStoreTestBase<EscapesSingleStoreTest.EscapesSingleStoreFixture>
    {
        public EscapesSingleStoreTest(EscapesSingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact]
        public override void Input_query_escapes_parameter()
        {
            base.Input_query_escapes_parameter();

            AssertSql(
                @"@p0='Back\slash's Garden Party' (Nullable = false) (Size = 4000)

INSERT INTO `Artists` (`Name`)
VALUES (@p0);
SELECT `ArtistId`
FROM `Artists`
WHERE ROW_COUNT() = 1 AND `ArtistId` = LAST_INSERT_ID();",
                //
                @"SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` LIKE '% Garden Party'");
        }

        [ConditionalTheory]
        public override async Task Where_query_escapes_literal(bool async)
        {
            await base.Where_query_escapes_literal(async);

            AssertSql(
                @"SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` = 'Back\\slasher''s'");
        }

        [ConditionalTheory]
        public override async Task Where_query_escapes_parameter(bool async)
        {
            await base.Where_query_escapes_parameter(async);

            AssertSql(
                @"@__artistName_0='Back\slasher's' (Size = 4000)

SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` = @__artistName_0");
        }

        [ConditionalTheory]
        public override async Task Where_contains_query_escapes(bool async)
        {
            await base.Where_contains_query_escapes(async);

            AssertSql(
                @"SELECT `a`.`ArtistId`, `a`.`Name`
FROM `Artists` AS `a`
WHERE `a`.`Name` IN ('Back\\slasher''s', 'John''s Chill Box')");
        }

        public class EscapesSingleStoreFixture : EscapesSingleStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
        }
    }
}
