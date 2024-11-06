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
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Update;

public class StoredProcedureUpdateSingleStoreTest : StoredProcedureUpdateTestBase
{
    public override async Task Insert_with_output_parameter(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Insert_with_output_parameter(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Insert"), exception.Message);
    }

    public override async Task Insert_twice_with_output_parameter(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Insert_twice_with_output_parameter(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Insert"), exception.Message);
    }

    public override async Task Insert_with_result_column(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE Entity_Insert(pName VARCHAR(255))
                             AS
                             BEGIN
                                 INSERT INTO Entity (Name) VALUES (pName);
                                 ECHO SELECT LAST_INSERT_ID() AS Id;
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint")
                    .UseSingleStoreIdentityColumn();

                modelBuilder.Entity<Entity>()
                    .InsertUsingStoredProcedure(
                        nameof(Entity) + "_Insert",
                        spb => spb
                            .HasParameter(e => e.Name)
                            .HasResultColumn(e => e.Id));
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // Ignore specific EF Core warnings
            });

        await using var context = contextFactory.CreateContext();

        var entity = new Entity
        {
            Name = "Foo"
        };

        context.Set<Entity>().Add(entity);
        await SaveChanges(context, async);

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var fetchedEntity = await context.Set<Entity>()
                .SingleAsync(e => e.Id == entity.Id);

            Assert.Equal("Foo", fetchedEntity.Name);
        }
    }

    public override async Task Insert_with_two_result_columns(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE EntityWithAdditionalProperty_Insert(pName VARCHAR(255))
                             AS
                             BEGIN
                                 INSERT INTO EntityWithAdditionalProperty (Name, AdditionalProperty) VALUES (pName, 8);
                                 ECHO SELECT LAST_INSERT_ID() AS Id, 8 AS AdditionalProperty;
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint")
                    .UseSingleStoreIdentityColumn(); // Ensure proper handling of the identity column

                modelBuilder.Entity<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .Property(e => e.AdditionalProperty)
                    .ValueGeneratedOnAddOrUpdate();

                modelBuilder.Entity<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .InsertUsingStoredProcedure(
                        nameof(EntityWithAdditionalProperty) + "_Insert",
                        spb => spb
                            .HasParameter(e => e.Name)
                            .HasResultColumn(e => e.Id)
                            .HasResultColumn(e => e.AdditionalProperty)); // Map the Id and AdditionalProperty as result columns
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // Ignore specific EF Core warnings
            });

        await using var context = contextFactory.CreateContext();

        var entity = new StoredProcedureUpdateTestBase.EntityWithAdditionalProperty { Name = "Foo" };

        context.Set<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>().Add(entity);
        await SaveChanges(context, async);

        Assert.Equal<int>(8, entity.AdditionalProperty);

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var fetchedEntity = await context.Set<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                .SingleAsync(e => e.Id == entity.Id);

            Assert.Equal("Foo", fetchedEntity.Name);
        }
    }

    public override async Task Insert_with_output_parameter_and_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Insert_with_output_parameter_and_result_column(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Insert"), exception.Message);
    }

    public override async Task Update(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

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
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

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
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Update"), exception.Message);
    }

    public override async Task Update_with_output_parameter_and_rows_affected_result_column_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Update_with_output_parameter_and_rows_affected_result_column_concurrency_failure(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Update"), exception.Message);
    }

    public override async Task Delete(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

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
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE Entity_Insert(pName varchar(255)) AS
                             BEGIN
                                INSERT INTO `Entity` (`Name`) VALUES (pName);
                                ECHO SELECT LAST_INSERT_ID() AS pId;
                             END;

                             GO;

                             CREATE PROCEDURE Entity_Delete(pId int) AS
                             BEGIN
                                DELETE FROM `Entity` WHERE `Id` = pId;
                             END;
                             """;
        DbContext context = null;
        try
        {
            // Initialize the context and configure the entity for insert and delete using stored procedures
            var contextFactory = await InitializeAsync<DbContext>(
                modelBuilder =>
                {
                    modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                    modelBuilder.Entity<Entity>()
                        .InsertUsingStoredProcedure<Entity>(
                            "Entity_Insert",
                            spb => spb
                                .HasParameter<string>(w => w.Name)
                                .HasResultColumn(w => w.Id)
                        )
                        .DeleteUsingStoredProcedure<Entity>(
                            "Entity_Delete",
                            spb => spb.HasOriginalValueParameter<int>(w => w.Id)
                        );
                },
                seed: ctx => CreateStoredProcedures(ctx, createSprocSql)
            );

            context = contextFactory.CreateContext();

            // Create and add a new entity to the context
            var entity1 = new Entity
            {
                Name = "Entity1"
            };
            context.Set<Entity>().Add(entity1);
            await context.SaveChangesAsync();

            // Clear the log and perform the delete and insert operations
            ClearLog();
            context.Set<Entity>().Remove(entity1);
            context.Set<Entity>().Add(new Entity
            {
                Name = "Entity2"
            });
            await SaveChanges(context, async);

            // Verify that Entity1 was deleted and Entity2 was inserted
            using (TestSqlLoggerFactory.SuspendRecordingEvents())
            {
                var entity1Count = await context.Set<Entity>()
                    .CountAsync(b => b.Name == "Entity1");
                var entity2Count = await context.Set<Entity>()
                    .CountAsync(b => b.Name == "Entity2");

                Assert.Equal(0, entity1Count);
                Assert.Equal(1, entity2Count);
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
@p1='Entity2' (Size = 4000)

CALL `Entity_Delete`(@p0);
CALL `Entity_Insert`(@p1);
""");
    }

    public override async Task Rows_affected_parameter(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_parameter(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_parameter_and_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_parameter_and_concurrency_failure(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_result_column(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE Entity_Update(pId int, pName varchar(255)) AS
                             BEGIN
                                 UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
                                 ECHO SELECT ROW_COUNT();
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                modelBuilder.Entity<Entity>()
                    .UpdateUsingStoredProcedure(
                        nameof(Entity) + "_Update",
                        spb => spb
                            .HasOriginalValueParameter(e => e.Id)
                            .HasParameter(e => e.Name)
                            .HasRowsAffectedResultColumn());

                modelBuilder.Entity<Entity>()
                    .Property(e => e.Id)
                    .UseSingleStoreIdentityColumn(); // Ensure proper handling of the identity column if needed
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // Ignore specific EF Core warnings
            });

        await using var context = contextFactory.CreateContext();

        var entity = new Entity
        {
            Name = "Initial"
        };

        context.Set<Entity>().Add(entity);
        await SaveChanges(context, async);

        entity.Name = "Updated";
        ClearLog(); // Assuming this method is accessible or replace with your logging logic
        await SaveChanges(context, async);

        context.ChangeTracker.Clear();

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var updatedEntity = await context.Set<Entity>()
                .SingleAsync(e => e.Id == entity.Id);

            Assert.Equal("Updated", updatedEntity.Name);
        }

        AssertSql(
            """
            @p0='1'
            @p1='Updated' (Size = 4000)

            CALL `Entity_Update`(@p0, @p1);
            """);
    }

    public override async Task Rows_affected_result_column_and_concurrency_failure(bool async)
    {
        var createSprocSql = """
                             CREATE PROCEDURE Entity_Update(pId int, pName varchar(255)) AS
                             BEGIN
                                 UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
                                 ECHO SELECT ROW_COUNT();
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                modelBuilder.Entity<Entity>()
                    .UpdateUsingStoredProcedure(
                        nameof(Entity) + "_Update",
                        spb => spb
                            .HasOriginalValueParameter(e => e.Id)
                            .HasParameter(e => e.Name)
                            .HasRowsAffectedResultColumn());
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql));

        await using var context1 = contextFactory.CreateContext();

        var entity = new Entity
        {
            Name = "Initial"
        };

        context1.Set<Entity>().Add(entity);
        await SaveChanges(context1, async);

        await using (var context2 = contextFactory.CreateContext())
        {
            var entityToDelete = await context2.Set<Entity>().SingleAsync(e => e.Name == "Initial");
            context2.Set<Entity>().Remove(entityToDelete);
            await SaveChanges(context2, async);
        }

        ClearLog(); // Assuming this method is accessible or replace with your logging logic

        entity.Name = "Updated";

        var concurrencyException = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            async () => await SaveChanges(context1, async));

        Assert.Same(entity, concurrencyException.Entries.Single().Entity);

        context1.ChangeTracker.Clear();

        AssertSql(
            """
            @p0='1'
            @p1='Updated' (Size = 4000)

            CALL `Entity_Update`(@p0, @p1);
            """);
    }

    public override async Task Rows_affected_return_value(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE Entity_Update(pId INT, pName VARCHAR(255))
                             RETURNS INT AS
                             BEGIN
                                 UPDATE Entity SET Name = pName WHERE Id = pId;
                                 RETURN ROW_COUNT();
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                modelBuilder.Entity<Entity>()
                    .UpdateUsingStoredProcedure(
                        nameof(Entity) + "_Update",
                        spb => spb
                            .HasOriginalValueParameter(e => e.Id)
                            .HasParameter(e => e.Name)
                            .HasRowsAffectedReturnValue(true));

                modelBuilder.Entity<Entity>()
                    .Property(e => e.Id)
                    .UseSingleStoreIdentityColumn(); // Ensure proper handling of the identity column if needed
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // Ignore specific EF Core warnings
            });

        await using var context = contextFactory.CreateContext();

        var entity = new Entity
        {
            Name = "Initial"
        };

        context.Set<Entity>().Add(entity);
        await SaveChanges(context, async);

        entity.Name = "Updated";
        ClearLog(); // Assuming this method is accessible or replace with your logging logic
        await SaveChanges(context, async);

        context.ChangeTracker.Clear();

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var updatedEntity = await context.Set<Entity>()
                .SingleAsync(e => e.Id == entity.Id);

            Assert.Equal("Updated", updatedEntity.Name);
        }

        AssertSql(
            """
            @p2=NULL (Nullable = false) (Direction = Output) (DbType = Int32)
            @p0='1'
            @p1='Updated' (Size = 4000)

            CALL `Entity_Update`(@p0, @p1);
            """);
    }

    //no exception
    public override async Task Rows_affected_return_value_and_concurrency_failure(bool async)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE Entity_Update(pId INT, pName VARCHAR(255))
                             RETURNS INT AS
                             BEGIN
                                 UPDATE Entity SET Name = pName WHERE Id = pId;
                                 RETURN ROW_COUNT();
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                modelBuilder.Entity<Entity>()
                    .UpdateUsingStoredProcedure(
                        nameof(Entity) + "_Update",
                        spb => spb
                            .HasOriginalValueParameter(e => e.Id)
                            .HasParameter(e => e.Name)
                            .HasRowsAffectedReturnValue(true));
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql));

        await using var context1 = contextFactory.CreateContext();

        var entity = new Entity
        {
            Name = "Initial"
        };

        context1.Set<Entity>().Add(entity);
        await SaveChanges(context1, async);

        await using (var context2 = contextFactory.CreateContext())
        {
            var entityToDelete = await context2.Set<Entity>().SingleAsync(e => e.Name == "Initial");
            context2.Set<Entity>().Remove(entityToDelete);
            await SaveChanges(context2, async);
        }

        ClearLog(); // Assuming this method is accessible or replace with your logging logic

        entity.Name = "Updated";

        var concurrencyException = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            async () => await SaveChanges(context1, async));

        Assert.Same(entity, concurrencyException.Entries.Single().Entity);

        context1.ChangeTracker.Clear();

        AssertSql(
            """
            @p2=NULL (Nullable = false) (Direction = Output) (DbType = Int32)
            @p0='1'
            @p1='Updated' (Size = 4000)

            CALL `Entity_Update`(@p0, @p1);
            """);
    }

    public override async Task Store_generated_concurrency_token_as_in_out_parameter(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Store_generated_concurrency_token_as_in_out_parameter(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Store_generated_concurrency_token_as_two_parameters(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Store_generated_concurrency_token_as_in_out_parameter(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    //no exception
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
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var createSprocSql = """
                             CREATE PROCEDURE Entity_Update(pId int, pNameCurrent varchar(255), pNameOriginal varchar(255)) AS
                             BEGIN
                                 IF pNameCurrent != pNameOriginal THEN
                                     UPDATE `Entity` SET `Name` = pNameCurrent WHERE `Id` = pId;
                                 END IF;
                             END
                             """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Entity>().Property(e => e.Id).HasColumnType("bigint");
                modelBuilder.Entity<StoredProcedureUpdateTestBase.Entity>()
                    .UpdateUsingStoredProcedure(
                        nameof(Entity) + "_Update",
                        spb => spb
                            .HasOriginalValueParameter(e => e.Id)
                            .HasParameter(e => e.Name, pb => pb.HasName("NameCurrent"))
                            .HasOriginalValueParameter(e => e.Name, pb => pb.HasName("NameOriginal")));

                modelBuilder.Entity<StoredProcedureUpdateTestBase.Entity>()
                    .Property(e => e.Id)
                    .UseSingleStoreIdentityColumn(); // Ensure proper handling of the identity column
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // Ignore specific EF Core warnings
            });

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
                .SingleAsync(e => e.Id == entity.Id);

            Assert.Equal("Updated", updatedEntity.Name);
        }

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
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Input_or_output_parameter_with_input(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Insert"), exception.Message);
    }

    public override async Task Input_or_output_parameter_with_output(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Input_or_output_parameter_with_output(async, createSprocSql: ""));

        Assert.Equal(
            SingleStoreStrings.StoredProcedureOutputParametersNotSupported(
                nameof(Entity), nameof(Entity) + "_Insert"), exception.Message);
    }

    public override async Task Tph(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Tph(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureOutputParametersNotSupported(nameof(Parent), nameof(Tph) + "_Insert"), exception.Message);
    }

    public override async Task Tpt(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Tpt(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureOutputParametersNotSupported(nameof(Parent), nameof(Parent) + "_Insert"), exception.Message);
    }

    public override async Task Tpt_mixed_sproc_and_non_sproc(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Tpt_mixed_sproc_and_non_sproc(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureOutputParametersNotSupported(nameof(Parent), nameof(Parent) + "_Insert"), exception.Message);
    }

    public override async Task Tpc(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Tpc(async, createSprocSql: ""));

        Assert.Equal(SingleStoreStrings.StoredProcedureOutputParametersNotSupported(nameof(Parent), nameof(Parent) + "_Insert"), exception.Message);
    }

    public override async Task Non_sproc_followed_by_sproc_commands_in_the_same_batch(bool async)
    {
        var createSprocSql = """
    CREATE PROCEDURE EntityWithAdditionalProperty_Insert(pName TEXT, pAdditional_property INT) AS
    BEGIN
        INSERT INTO EntityWithAdditionalProperty (`Name`, `AdditionalProperty`) VALUES (pName, pAdditional_property);
        ECHO SELECT LAST_INSERT_ID() AS Id;
    END
    """;

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint")
                    .UseSingleStoreIdentityColumn(); // Ensure proper handling of the identity column

                modelBuilder.Entity<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .InsertUsingStoredProcedure(
                        nameof(EntityWithAdditionalProperty) + "_Insert",
                        spb => spb
                            .HasParameter(e => e.Name)
                            .HasParameter(e => e.AdditionalProperty)
                            .HasResultColumn(e => e.Id));

                modelBuilder.Entity<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .Property(e => e.AdditionalProperty)
                    .IsConcurrencyToken(true); // Set AdditionalProperty as a concurrency token
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // Ignore specific EF Core warnings
            });

        await using var context = contextFactory.CreateContext();

        StoredProcedureUpdateTestBase.EntityWithAdditionalProperty entity1 = null;
        try
        {
            entity1 = new StoredProcedureUpdateTestBase.EntityWithAdditionalProperty
            {
                Name = "Entity1",
                AdditionalProperty = 1
            };

            context.Set<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>().Add(entity1);
            using (TestSqlLoggerFactory.SuspendRecordingEvents())
            {
                await SaveChanges(context, async);
            }

            var entity2 = new StoredProcedureUpdateTestBase.EntityWithAdditionalProperty
            {
                Name = "Entity2",
                AdditionalProperty = 2
            };
            context.Set<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>().Add(entity2);

            // Modify entity1
            entity1.Name = "Entity1_Modified";
            entity1.AdditionalProperty = 2;

            await SaveChanges(context, async);

            using (TestSqlLoggerFactory.SuspendRecordingEvents())
            {
                Assert.Equal("Entity2", context.Set<StoredProcedureUpdateTestBase.EntityWithAdditionalProperty>()
                    .Single(e => e.Id == entity2.Id).Name);
            }
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (context != null)
                await context.DisposeAsync();

            entity1 = null;
        }

        AssertSql(
            """
            @p2='1'
            @p0='2'
            @p3='1'
            @p1='Entity1_Modified' (Size = 4000)
            @p4='Entity2' (Size = 4000)
            @p5='2'

            UPDATE `EntityWithAdditionalProperty` SET `AdditionalProperty` = @p0, `Name` = @p1
            WHERE `Id` = @p2 AND `AdditionalProperty` = @p3;
            SELECT ROW_COUNT();

            CALL `EntityWithAdditionalProperty_Insert`(@p4, @p5);
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
