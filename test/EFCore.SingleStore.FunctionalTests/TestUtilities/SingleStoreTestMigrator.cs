﻿//ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Migrations.Internal;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public class SingleStoreTestMigrator : SingleStoreMigrator
    {
        public Func<MigrationsSqlGenerationOptions, MigrationsSqlGenerationOptions> MigrationsSqlGenerationOptionsOverrider { get; set; }

        public SingleStoreTestMigrator(
            [NotNull] IMigrationsAssembly migrationsAssembly,
            [NotNull] IHistoryRepository historyRepository,
            [NotNull] IDatabaseCreator databaseCreator,
            [NotNull] IMigrationsSqlGenerator migrationsSqlGenerator,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder,
            [NotNull] IMigrationCommandExecutor migrationCommandExecutor,
            [NotNull] IRelationalConnection connection,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper,
            [NotNull] ICurrentDbContext currentContext,
            [NotNull] IModelRuntimeInitializer modelRuntimeInitializer,
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            [NotNull] IRelationalCommandDiagnosticsLogger commandLogger,
            [NotNull] IDatabaseProvider databaseProvider)
            : base(
                migrationsAssembly,
                historyRepository,
                databaseCreator,
                migrationsSqlGenerator,
                rawSqlCommandBuilder,
                migrationCommandExecutor,
                connection,
                sqlGenerationHelper,
                currentContext,
                modelRuntimeInitializer,
                logger,
                commandLogger,
                databaseProvider)
        {
        }

        protected override IReadOnlyList<MigrationCommand> GenerateUpSql(
            Migration migration,
            MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
            => base.GenerateUpSql(migration, MigrationsSqlGenerationOptionsOverrider?.Invoke(options) ?? options);

        protected override IReadOnlyList<MigrationCommand> GenerateDownSql(
            Migration migration,
            Migration previousMigration,
            MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
            => base.GenerateDownSql(migration, previousMigration, MigrationsSqlGenerationOptionsOverrider?.Invoke(options) ?? options);
    }
}
