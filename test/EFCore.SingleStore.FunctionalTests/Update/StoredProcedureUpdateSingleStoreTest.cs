using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Update;

public class StoredProcedureUpdateSingleStoreTest : StoredProcedureUpdateTestBase
{
    public override async Task Insert_with_output_parameter(bool async)
    {
        await base.Insert_with_output_parameter(
            async,
"""
CREATE PROCEDURE Entity_Insert(pName varchar(255), OUT pId int)
BEGIN
    INSERT INTO `Entity` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='New' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Insert`(@p1, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Insert_twice_with_output_parameter(bool async)
    {
        await base.Insert_twice_with_output_parameter(
            async,
"""
CREATE PROCEDURE Entity_Insert(pName varchar(255), OUT pId int)
BEGIN
    INSERT INTO `Entity` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='New1' (Size = 4000)
@p3='New2' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Insert`(@p1, @_out_p0);
SELECT @_out_p0;
SET @_out_p2 = NULL;
CALL `Entity_Insert`(@p3, @_out_p2);
SELECT @_out_p2;
""");
    }

    public override async Task Insert_with_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Insert_with_result_column(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Insert"), exception.Message);
    }

    public override async Task Insert_with_two_result_columns(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Insert_with_two_result_columns(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureResultColumnsNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Insert"), exception.Message);
    }

    public override async Task Insert_with_output_parameter_and_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Insert_with_output_parameter_and_result_column(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureResultColumnsNotSupported(nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Insert"), exception.Message);
    }

    public override async Task Update(bool async)
    {
        var createSprocSql = """
CREATE PROCEDURE Entity_Update(pId int, pName varchar(255)) AS
BEGIN
UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
END
""";
        DbContext context = null;
        try
        {
            context = (await InitializeAsync<DbContext>(
                    modelBuilder =>
                    {
                        modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                        modelBuilder.Entity<Entity>()
                            .UpdateUsingStoredProcedure<Entity>(
                                "Entity_Update",
                                spb => spb
                                    .HasOriginalValueParameter<long>(w => w.Id)
                                    .HasParameter<string>(w => w.Name));
                    },
                    seed: ctx => CreateStoredProcedures(ctx, createSprocSql))
                ).CreateContext();

            var entity = new Entity { Name = "Initial" };
            context.Set<Entity>().Add(entity);
            await SaveChanges(context, async);

            ClearLog();

            entity.Name = "Updated";
            await SaveChanges(context, async);

            using (TestSqlLoggerFactory.SuspendRecordingEvents())
            {
                var updatedEntity = await context.Set<Entity>()
                    .SingleAsync(w => w.Id == entity.Id);

                Assert.Equal("Updated", updatedEntity.Name);
            }
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (context != null)
            {
                await context.DisposeAsync();
            }
        }

        AssertSql(
"""
@p0='1'
@p1='Updated' (Size = 4000)

CALL `Entity_Update`(@p0, @p1);
""");
    }

    public override async Task Update_partial(bool async)
    {
        var createSprocSql = """
CREATE PROCEDURE EntityWithAdditionalProperty_Update(pId int, pName varchar(255), pAdditionalProperty int) AS
BEGIN
UPDATE `EntityWithAdditionalProperty` SET `Name` = pName, `AdditionalProperty` = pAdditionalProperty WHERE `Id` = pId;
END
""";

        DbContext context = null;
        try
        {
            context = (await InitializeAsync<DbContext>(
                    modelBuilder =>
                    {
                        modelBuilder.Entity<EntityWithAdditionalProperty>().Property(e => e.Id).HasColumnType("bigint");
                        modelBuilder.Entity<EntityWithAdditionalProperty>()
                            .UpdateUsingStoredProcedure<EntityWithAdditionalProperty>(
                                "EntityWithAdditionalProperty_Update",
                                spb => spb
                                    .HasOriginalValueParameter<int>(w => w.Id)
                                    .HasParameter<string>(w => w.Name)
                                    .HasParameter<int>(w => w.AdditionalProperty));
                    },
                    seed: ctx => CreateStoredProcedures(ctx, createSprocSql))
                ).CreateContext();

            var entity = new EntityWithAdditionalProperty
            {
                Name = "Foo",
                AdditionalProperty = 8
            };
            context.Set<EntityWithAdditionalProperty>().Add(entity);
            await SaveChanges(context, async);

            ClearLog();

            entity.Name = "Updated";
            await SaveChanges(context, async);

            using (TestSqlLoggerFactory.SuspendRecordingEvents())
            {
                var updatedEntity = await context.Set<EntityWithAdditionalProperty>()
                    .SingleAsync(w => w.Id == entity.Id);

                Assert.Equal("Updated", updatedEntity.Name);
                Assert.Equal(8, updatedEntity.AdditionalProperty);
            }
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (context != null)
            {
                await context.DisposeAsync();
            }
        }

        AssertSql(
"""
@p0='1'
@p1='Updated' (Size = 4000)
@p2='8'

CALL `EntityWithAdditionalProperty_Update`(@p0, @p1, @p2);
""");
    }

    public override async Task Update_with_output_parameter_and_rows_affected_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Update_with_output_parameter_and_rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureResultColumnsNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Update"), exception.Message);
    }

    public override async Task Update_with_output_parameter_and_rows_affected_result_column_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Update_with_output_parameter_and_rows_affected_result_column_concurrency_failure(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureResultColumnsNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Update"), exception.Message);
    }

    public override async Task Delete(bool async)
    {
        var createSprocSql = """
                             CREATE PROCEDURE Entity_Delete(pId int) AS
                             BEGIN
                             DELETE FROM `Entity` WHERE `Id` = pId;
                             END
                             """;
        DbContext context = null;
        try
        {
            var contextFactory = await InitializeAsync<DbContext>(
                modelBuilder =>
                {
                    modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                    modelBuilder.Entity<Entity>().DeleteUsingStoredProcedure<Entity>(
                        "Entity_Delete",
                        spb => spb.HasOriginalValueParameter<int>(w => w.Id)
                    );
                },
                seed: ctx => CreateStoredProcedures(ctx, createSprocSql)
            );

            context = contextFactory.CreateContext();

            // Create and add a new entity to the context
            var entity = new Entity
            {
                Name = "Initial"
            };
            context.Set<Entity>().Add(entity);
            await context.SaveChangesAsync();

            // Clear the log and remove the entity
            ClearLog();
            context.Set<Entity>().Remove(entity);
            await SaveChanges(context, async);

            // Verify the entity has been deleted
            using (TestSqlLoggerFactory.SuspendRecordingEvents())
            {
                var count = await context.Set<Entity>()
                    .CountAsync(b => b.Name == "Initial");
                Assert.Equal(0, count);
            }
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (context != null)
            {
                await context.DisposeAsync();
            }
        }

        AssertSql(
"""
@p0='1'

CALL `Entity_Delete`(@p0);
""");
    }

    public override async Task Delete_and_insert(bool async)
    {
        await base.Delete_and_insert(
            async,
"""
CREATE PROCEDURE Entity_Insert(pName varchar(255)) RETURNS INT AS
BEGIN
    INSERT INTO `Entity` (`Name`) VALUES (pName);
    RETURN LAST_INSERT_ID();
END;

GO;

CREATE PROCEDURE Entity_Delete(pId int) AS
BEGIN
DELETE FROM `Entity` WHERE `Id` = pId;
END;
""");

        AssertSql(
"""
@p0='1'
@p2='Entity2' (Size = 4000)

CALL `Entity_Delete`(@p0);
SET @_out_p1 = NULL;
CALL `Entity_Insert`(@p2, @_out_p1);
SELECT @_out_p1;
""");
    }

    public override async Task Rows_affected_parameter(bool async)
    {
        var createSprocSql = """
CREATE PROCEDURE Entity_Update(pId int, pName varchar(255)) RETURNS INT AS
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
    RETURN ROW_COUNT();
END
""";

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                modelBuilder.Entity<StoredProcedureUpdateTestBase.Entity>()
                    .UpdateUsingStoredProcedure(
                        "Entity_Update",
                        spb => spb
                            .HasOriginalValueParameter<long>(w => w.Id)
                            .HasParameter(w => w.Name));
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql));

        await using var context = contextFactory.CreateContext();

        var entity = new StoredProcedureUpdateTestBase.Entity
        {
            Name = "Initial"
        };

        context.Set<StoredProcedureUpdateTestBase.Entity>().Add(entity);
        await SaveChanges(context, async);

        entity.Name = "Updated";
        ClearLog(); // Assuming this method is accessible or replace with your logging logic
        await SaveChanges(context, async);

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var updatedEntity = await context.Set<StoredProcedureUpdateTestBase.Entity>()
                .SingleAsync(w => w.Id == entity.Id);

            Assert.Equal("Updated", updatedEntity.Name);
        }

        AssertSql(
            """
            @p1='Updated' (Size = 4000)
            @p2='1'

            SET @_out_p0 = NULL;
            CALL `Entity_Update`(@p2, @p1);
            SELECT @_out_p0;
            """);
    }

    public override async Task Rows_affected_parameter_and_concurrency_failure(bool async)
    {
        await base.Rows_affected_parameter_and_concurrency_failure(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pName varchar(255), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
    SET pRowsAffected = ROW_COUNT();
END
""");

        AssertSql(
"""
@p1='1'
@p2='Updated' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Update`(@p1, @p2, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Rows_affected_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_result_column_and_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column_and_concurrency_failure(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_return_value(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_return_value(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureReturnValueNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_return_value_and_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_return_value(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureReturnValueNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Store_generated_concurrency_token_as_in_out_parameter(bool async)
    {
        await base.Store_generated_concurrency_token_as_in_out_parameter(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, INOUT pConcurrencyToken timestamp(6), pName varchar(255), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId AND `ConcurrencyToken` = pConcurrencyToken;
    SET pRowsAffected = ROW_COUNT();
    SELECT `ConcurrencyToken` INTO pConcurrencyToken FROM `Entity` WHERE `Id` = pId;
END
""");

        Assert.StartsWith(
            """
@p2='1'
@p4=NULL (DbType = DateTime)
@p0='
""",
            TestSqlLoggerFactory.Sql);

        Assert.EndsWith(
"""
' (Nullable = true) (DbType = DateTime)
@p3='Updated' (Size = 4000)

SET @_out_p0 = @p0;
SET @_out_p1 = NULL;
CALL `Entity_Update`(@p2, @_out_p0, @p3, @_out_p1);
SELECT @_out_p0, @_out_p1;
""",
            TestSqlLoggerFactory.Sql);

//         AssertSql(
// """
// @p2='1'
// @p4=NULL (DbType = DateTime)
// @p0='2022-11-14T12:03:01.9884410' (Nullable = true) (DbType = DateTime)
// @p3='Updated' (Size = 4000)
//
// SET @_out_p0 = @p0;
// SET @_out_p1 = NULL;
// CALL `Entity_Update`(@p2, @_out_p0, @p3, @_out_p1);
// SELECT @_out_p0, @_out_p1;
// """);
    }

    public override async Task Store_generated_concurrency_token_as_two_parameters(bool async)
    {
        await base.Store_generated_concurrency_token_as_two_parameters(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pConcurrencyTokenIn timestamp(6), pName varchar(255), OUT pConcurrencyTokenOut timestamp(6), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId AND `ConcurrencyToken` = pConcurrencyTokenIn;
    SET pRowsAffected = ROW_COUNT();
    SELECT `ConcurrencyToken` INTO pConcurrencyTokenOut FROM `Entity` WHERE `Id` = pId;
END
""");

        Assert.StartsWith(
"""
@p2='1'
@p3='
""",
            TestSqlLoggerFactory.Sql);

        Assert.EndsWith(
            """
' (Nullable = true) (DbType = DateTime)
@p4='Updated' (Size = 4000)

SET @_out_p0 = NULL;
SET @_out_p1 = NULL;
CALL `Entity_Update`(@p2, @p3, @p4, @_out_p0, @_out_p1);
SELECT @_out_p0, @_out_p1;
""",
            TestSqlLoggerFactory.Sql);

//         AssertSql(
//             """
// @p2='1'
// @p3='2022-11-14T14:02:25.0912340' (Nullable = true) (DbType = DateTime)
// @p4='Updated' (Size = 4000)
//
// SET @_out_p0 = NULL;
// SET @_out_p1 = NULL;
// CALL `Entity_Update`(@p2, @p3, @p4, @_out_p0, @_out_p1);
// SELECT @_out_p0, @_out_p1;
// """);
    }

    public override async Task User_managed_concurrency_token(bool async)
    {
        var createSprocSql = """
CREATE PROCEDURE EntityWithAdditionalProperty_Update(pId int, pConcurrencyTokenOriginal int, pName varchar(255), pConcurrencyTokenCurrent int) RETURNS INT AS
BEGIN
 UPDATE `EntityWithAdditionalProperty` SET `Name` = pName, `AdditionalProperty` = pConcurrencyTokenCurrent WHERE `Id` = pId AND `AdditionalProperty` = pConcurrencyTokenOriginal;
 RETURN ROW_COUNT();
END
""";
        NonSharedModelTestBase.ContextFactory<DbContext> contextFactory = null;
        DbContext context1 = null;
        try
        {
            contextFactory = await InitializeAsync<DbContext>(
                modelBuilder =>
                {
                    modelBuilder.Entity<EntityWithAdditionalProperty>().Property(e => e.Id).HasColumnType("bigint");
                    modelBuilder.Entity<EntityWithAdditionalProperty>(b =>
                    {
                        b.Property<int>(e => e.AdditionalProperty)
                            .IsConcurrencyToken(true);

                        b.UpdateUsingStoredProcedure<EntityWithAdditionalProperty>(
                            "EntityWithAdditionalProperty_Update",
                            spb => spb
                                .HasOriginalValueParameter<long>(w => w.Id)
                                .HasOriginalValueParameter<int>(w => w.AdditionalProperty, pb => pb.HasName("ConcurrencyTokenOriginal"))
                                .HasParameter<string>(w => w.Name)
                                .HasParameter<int>(w => w.AdditionalProperty, pb => pb.HasName("ConcurrencyTokenCurrent")));
                    });
                },
                seed: ctx => CreateStoredProcedures(ctx, createSprocSql)
            );

            context1 = contextFactory.CreateContext();

            var entity1 = new EntityWithAdditionalProperty
            {
                Name = "Initial",
                AdditionalProperty = 8
            };
            context1.Set<EntityWithAdditionalProperty>().Add(entity1);
            await SaveChanges(context1, async);

            entity1.Name = "Updated";
            entity1.AdditionalProperty = 9;

            await using (var context2 = contextFactory.CreateContext())
            {
                var entity2 = await context2.Set<EntityWithAdditionalProperty>()
                    .SingleAsync(w => w.Name == "Initial");

                entity2.Name = "Preempted";
                entity2.AdditionalProperty = 999;
                await SaveChanges(context2, async);
            }

            ClearLog();

            var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await SaveChanges(context1, async);
            });

            Assert.Same(entity1, Assert.Single(ex.Entries).Entity);
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (context1 != null)
            {
                await context1.DisposeAsync();
            }
        }


        AssertSql(
"""
@p1='1'
@p2='8'
@p3='Updated' (Size = 4000)
@p4='9'

SET @_out_p0 = NULL;
CALL `EntityWithAdditionalProperty_Update`(@p1, @p2, @p3, @p4, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Original_and_current_value_on_non_concurrency_token(bool async)
    {
        await base.Original_and_current_value_on_non_concurrency_token(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pNameCurrent varchar(255), pNameOriginal varchar(255))
BEGIN
    IF pNameCurrent <> pNameOriginal THEN
        UPDATE `Entity` SET `Name` = pNameCurrent WHERE `Id` = pId;
    END IF;
END
""");

        AssertSql(
"""
@p0='1'
@p1='Updated' (Size = 4000)
@p2='Initial' (Size = 4000)

CALL `Entity_Update`(@p0, @p1, @p2);
""");
    }

    public override async Task Input_or_output_parameter_with_input(bool async)
    {
        await base.Input_or_output_parameter_with_input(
            async,
"""
CREATE PROCEDURE Entity_Insert(OUT pId int, INOUT pName varchar(255))
BEGIN
    IF pName IS NULL THEN
        INSERT INTO `Entity` (`Name`) VALUES ('Some default value');
        SET pName = 'Some default value';
    ELSE
        INSERT INTO `Entity` (`Name`) VALUES (pName);
        SET pName = NULL;
    END IF;

    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='Initial' (Nullable = false) (Size = 4000)

SET @_out_p0 = NULL;
SET @_out_p1 = @p1;
CALL `Entity_Insert`(@_out_p0, @_out_p1);
SELECT @_out_p0, @_out_p1;
""");
    }

    public override async Task Input_or_output_parameter_with_output(bool async)
    {
        await base.Input_or_output_parameter_with_output(
            async,
"""
CREATE PROCEDURE Entity_Insert(OUT pId int, INOUT pName varchar(255))
BEGIN
    IF pName IS NULL THEN
        INSERT INTO `Entity` (`Name`) VALUES ('Some default value');
        SET pName = 'Some default value';
    ELSE
        INSERT INTO `Entity` (`Name`) VALUES (pName);
        SET pName = NULL;
    END IF;

    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
SET @_out_p0 = NULL;
SET @_out_p1 = @p1;
CALL `Entity_Insert`(@_out_p0, @_out_p1);
SELECT @_out_p0, @_out_p1;
""");
    }

    public override async Task Tph(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Tpt(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Tpt_mixed_sproc_and_non_sproc(bool async)
    {
        await base.Tpt_mixed_sproc_and_non_sproc(
            async,
"""
CREATE PROCEDURE Parent_Insert(OUT pId int, pName varchar(255))
BEGIN
    INSERT INTO `Parent` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='Child' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Parent_Insert`(@_out_p0, @p1);
SELECT @_out_p0;
""",
                //
                """
@p2='1'
@p3='8'

SET AUTOCOMMIT = 1;
INSERT INTO `Child1` (`Id`, `Child1Property`)
VALUES (@p2, @p3);
""");
    }

    public override async Task Tpc(bool async)
    {
        var createSprocSql =
"""
ALTER TABLE `Child1` MODIFY COLUMN `Id` INT AUTO_INCREMENT;
ALTER TABLE `Child1` AUTO_INCREMENT = 100000;

GO;

CREATE PROCEDURE Child1_Insert(OUT pId int, pName varchar(255), pChild1Property int)
BEGIN
    INSERT INTO `Child1` (`Name`, `Child1Property`) VALUES (pName, pChild1Property);
    SET pId = LAST_INSERT_ID();
END
""";

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Parent>().UseTpcMappingStrategy();

                modelBuilder.Entity<Child1>()
                    .UseTpcMappingStrategy()
                    .InsertUsingStoredProcedure(
                        nameof(Child1) + "_Insert",
                        spb => spb
                            .HasParameter(w => w.Id, pb => pb.IsOutput())
                            .HasParameter(w => w.Name)
                            .HasParameter(w => w.Child1Property))
                    .Property(e => e.Id).UseSingleStoreIdentityColumn(); // <--
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // <-- added
            });

        await using var context = contextFactory.CreateContext();

        var entity1 = new Child1 { Name = "Child", Child1Property = 8 };
        context.Set<Child1>().Add(entity1);
        await SaveChanges(context, async);

        context.ChangeTracker.Clear();

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var entity2 = context.Set<Child1>().Single(b => b.Id == entity1.Id);

            Assert.Equal("Child", entity2.Name);
            Assert.Equal(8, entity2.Child1Property);
        }

        AssertSql(
"""
@p1='Child' (Size = 4000)
@p2='8'

SET @_out_p0 = NULL;
CALL `Child1_Insert`(@_out_p0, @p1, @p2);
SELECT @_out_p0;
""");
    }

    public override async Task Non_sproc_followed_by_sproc_commands_in_the_same_batch(bool async)
    {
        await base.Non_sproc_followed_by_sproc_commands_in_the_same_batch(
            async,
            """
            CREATE PROCEDURE EntityWithAdditionalProperty_Insert(pName text, OUT pId int, pAdditional_property int)
            BEGIN
                INSERT INTO EntityWithAdditionalProperty (`Name`, `AdditionalProperty`) VALUES (pName, pAdditional_property);
                SET pId = LAST_INSERT_ID();
            END
            """);

        AssertSql(
            """
            @p2='1'
            @p0='2'
            @p3='1'
            @p1='Entity1_Modified' (Size = 4000)
            @p5='Entity2' (Size = 4000)
            @p6='0'

            UPDATE `EntityWithAdditionalProperty` SET `AdditionalProperty` = @p0, `Name` = @p1
            WHERE `Id` = @p2 AND `AdditionalProperty` = @p3;
            SELECT ROW_COUNT();

            SET @_out_p4 = NULL;
            CALL `EntityWithAdditionalProperty_Insert`(@p5, @_out_p4, @p6);
            SELECT @_out_p4;
            """);
    }

    private async Task SaveChanges(DbContext context, bool async)
    {
        if (async)
        {
            await context.SaveChangesAsync();
        }
        else
        {
            // ReSharper disable once MethodHasAsyncOverload
            context.SaveChanges();
        }
    }

    protected override void CreateStoredProcedures(DbContext context, string createSprocSql)
    {
        foreach (var batch in
                 new Regex(@"[\r\n\s]*(?:\r|\n)GO;?[\r\n\s]*", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000.0))
                     .Split(createSprocSql).Where(b => !string.IsNullOrEmpty(b)))
        {
            context.Database.ExecuteSqlRaw(batch);
        }
    }

    protected override void ConfigureStoreGeneratedConcurrencyToken(EntityTypeBuilder entityTypeBuilder, string propertyName)
        => entityTypeBuilder.Property<byte[]>(propertyName).IsRowVersion();

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
