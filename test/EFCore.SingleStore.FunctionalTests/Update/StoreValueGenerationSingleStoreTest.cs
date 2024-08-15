using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.StoreValueGenerationModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Update;

public class StoreValueGenerationSingleStoreTest : StoreValueGenerationTestBase<
    StoreValueGenerationSingleStoreTest.StoreValueGenerationSingleStoreFixture>
{
    public StoreValueGenerationSingleStoreTest(StoreValueGenerationSingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        // fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override bool ShouldCreateImplicitTransaction(
        EntityState firstOperationType,
        EntityState? secondOperationType,
        GeneratedValues generatedValues,
        bool withSameEntityType)
    {
        if (AppConfig.ServerVersion.Supports.Returning)
        {
            return generatedValues != GeneratedValues.None && firstOperationType == EntityState.Modified ||
                   secondOperationType is not null && !(firstOperationType == secondOperationType &&
                                                        (firstOperationType == EntityState.Added && withSameEntityType && generatedValues == GeneratedValues.None));
        }
        else
        {
            return generatedValues != GeneratedValues.None && firstOperationType != EntityState.Deleted ||
                   secondOperationType is not null && !(firstOperationType == secondOperationType &&
                                                        (firstOperationType == EntityState.Added && withSameEntityType));
        }
    }

    protected override int ShouldExecuteInNumberOfCommands(
        EntityState firstOperationType,
        EntityState? secondOperationType,
        GeneratedValues generatedValues,
        bool withDatabaseGenerated)
        => 1;

    #region Single operation

    public override async Task Add_with_generated_values(bool async)
    {
        await base.Add_with_generated_values(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
@p0='1000'

SET AUTOCOMMIT = 1;
INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p0)
RETURNING `Id`, `Data1`;
""");
        }
        else
        {
            AssertSql(
                """
@p0='1000'

INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p0);
SELECT `Id`, `Data1`
FROM `WithSomeDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""");
        }
    }

    public override async Task Add_with_no_generated_values(bool async)
    {
        await base.Add_with_no_generated_values(async);

        AssertSql(
"""
@p0='100'
@p1='1000'
@p2='1000'

SET AUTOCOMMIT = 1;
INSERT INTO `WithNoDatabaseGenerated` (`Id`, `Data1`, `Data2`)
VALUES (@p0, @p1, @p2);
""");
    }

    public override async Task Add_with_all_generated_values(bool async)
    {
        await base.Add_with_all_generated_values(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
SET AUTOCOMMIT = 1;
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ()
RETURNING `Id`, `Data1`, `Data2`;
""");
        }
        else
        {
            AssertSql(
                """
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ();
SELECT `Id`, `Data1`, `Data2`
FROM `WithAllDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""");
        }
    }

    public override async Task Modify_with_no_generated_values(bool async)
    {
        await base.Modify_with_no_generated_values(async);

        AssertSql(
"""
@p2='1'
@p0='1000'
@p1='1000'

SET AUTOCOMMIT = 1;
UPDATE `WithNoDatabaseGenerated` SET `Data1` = @p0, `Data2` = @p1
WHERE `Id` = @p2;
SELECT ROW_COUNT();
""");
    }

    #endregion Single operation

    #region Two operations with same entity type

    public override async Task Add_Add_with_same_entity_type_and_generated_values(bool async)
    {
        await base.Add_Add_with_same_entity_type_and_generated_values(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
@p0='1000'
@p1='1001'

INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p0)
RETURNING `Id`, `Data1`;
INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p1)
RETURNING `Id`, `Data1`;
""");
        }
        else
        {
            AssertSql(
                """
@p0='1000'
@p1='1001'

INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p0);
SELECT `Id`, `Data1`
FROM `WithSomeDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p1);
SELECT `Id`, `Data1`
FROM `WithSomeDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""");
        }
    }

    public override async Task Add_Add_with_same_entity_type_and_no_generated_values(bool async)
    {
        await base.Add_Add_with_same_entity_type_and_no_generated_values(async);

        AssertSql(
"""
@p0='100'
@p1='1000'
@p2='1000'
@p3='101'
@p4='1001'
@p5='1001'

SET AUTOCOMMIT = 1;
INSERT INTO `WithNoDatabaseGenerated` (`Id`, `Data1`, `Data2`)
VALUES (@p0, @p1, @p2),
(@p3, @p4, @p5);
""");
    }

    public override async Task Add_Add_with_same_entity_type_and_all_generated_values(bool async)
    {
        await base.Add_Add_with_same_entity_type_and_all_generated_values(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ()
RETURNING `Id`, `Data1`, `Data2`;
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ()
RETURNING `Id`, `Data1`, `Data2`;
""");
        }
        else
        {
            AssertSql(
                """
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ();
SELECT `Id`, `Data1`, `Data2`
FROM `WithAllDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ();
SELECT `Id`, `Data1`, `Data2`
FROM `WithAllDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""");
        }
    }

    public override async Task Modify_Modify_with_same_entity_type_and_no_generated_values(bool async)
    {
        await base.Modify_Modify_with_same_entity_type_and_no_generated_values(async);

        AssertSql(
"""
@p2='1'
@p0='1000'
@p1='1000'
@p5='2'
@p3='1001'
@p4='1001'

UPDATE `WithNoDatabaseGenerated` SET `Data1` = @p0, `Data2` = @p1
WHERE `Id` = @p2;
SELECT ROW_COUNT();

UPDATE `WithNoDatabaseGenerated` SET `Data1` = @p3, `Data2` = @p4
WHERE `Id` = @p5;
SELECT ROW_COUNT();
""");
    }

    #endregion Two operations with same entity type

    #region Two operations with different entity types

    public override async Task Add_Add_with_different_entity_types_and_generated_values(bool async)
    {
        await base.Add_Add_with_different_entity_types_and_generated_values(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
@p0='1000'
@p1='1001'

INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p0)
RETURNING `Id`, `Data1`;
INSERT INTO `WithSomeDatabaseGenerated2` (`Data2`)
VALUES (@p1)
RETURNING `Id`, `Data1`;
""");
        }
        else
        {
            AssertSql(
                """
@p0='1000'
@p1='1001'

INSERT INTO `WithSomeDatabaseGenerated` (`Data2`)
VALUES (@p0);
SELECT `Id`, `Data1`
FROM `WithSomeDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

INSERT INTO `WithSomeDatabaseGenerated2` (`Data2`)
VALUES (@p1);
SELECT `Id`, `Data1`
FROM `WithSomeDatabaseGenerated2`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""");
        }
    }

    public override async Task Add_Add_with_different_entity_types_and_no_generated_values(bool async)
    {
        await base.Add_Add_with_different_entity_types_and_no_generated_values(async);

        AssertSql(
"""
@p0='100'
@p1='1000'
@p2='1000'
@p3='101'
@p4='1001'
@p5='1001'

INSERT INTO `WithNoDatabaseGenerated` (`Id`, `Data1`, `Data2`)
VALUES (@p0, @p1, @p2);
INSERT INTO `WithNoDatabaseGenerated2` (`Id`, `Data1`, `Data2`)
VALUES (@p3, @p4, @p5);
""");
    }

    public override async Task Add_Add_with_different_entity_types_and_all_generated_values(bool async)
    {
        await base.Add_Add_with_different_entity_types_and_all_generated_values(async);

        if (AppConfig.ServerVersion.Supports.Returning)
        {
            AssertSql(
                """
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ()
RETURNING `Id`, `Data1`, `Data2`;
INSERT INTO `WithAllDatabaseGenerated2` ()
VALUES ()
RETURNING `Id`, `Data1`, `Data2`;
""");
        }
        else
        {
            AssertSql(
                """
INSERT INTO `WithAllDatabaseGenerated` ()
VALUES ();
SELECT `Id`, `Data1`, `Data2`
FROM `WithAllDatabaseGenerated`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

INSERT INTO `WithAllDatabaseGenerated2` ()
VALUES ();
SELECT `Id`, `Data1`, `Data2`
FROM `WithAllDatabaseGenerated2`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();
""");
        }
    }

    public override async Task Modify_Modify_with_different_entity_types_and_no_generated_values(bool async)
    {
        await base.Modify_Modify_with_different_entity_types_and_no_generated_values(async);

        AssertSql(
"""
@p2='1'
@p0='1000'
@p1='1000'
@p5='2'
@p3='1001'
@p4='1001'

UPDATE `WithNoDatabaseGenerated` SET `Data1` = @p0, `Data2` = @p1
WHERE `Id` = @p2;
SELECT ROW_COUNT();

UPDATE `WithNoDatabaseGenerated2` SET `Data1` = @p3, `Data2` = @p4
WHERE `Id` = @p5;
SELECT ROW_COUNT();
""");
    }

    #endregion Two operations with different entity types

    public class StoreValueGenerationSingleStoreFixture : StoreValueGenerationFixtureBase
    {
        private string _cleanDataSql;

        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType == typeof(StoreValueGenerationData))
                {
                    modelBuilder.Entity(entityType.Name, b =>
                    {
                        b.Property("Id").HasColumnType("bigint");
                    });
                }
            }
        }

        public override void CleanData()
        {
            using var context = CreateContext();
            context.Database.ExecuteSqlRaw(_cleanDataSql ??= GetCleanDataSql());
        }

        private string GetCleanDataSql()
        {
            var context = CreateContext();
            var builder = new StringBuilder();

            var helper = context.GetService<ISqlGenerationHelper>();
            var tables = context.Model.GetEntityTypes()
                .SelectMany(e => e.GetTableMappings().Select(m => helper.DelimitIdentifier(m.Table.Name, m.Table.Schema)));

            foreach (var table in tables)
            {
                builder.AppendLine($"TRUNCATE TABLE {table};");
            }

            return builder.ToString();
        }
    }
}