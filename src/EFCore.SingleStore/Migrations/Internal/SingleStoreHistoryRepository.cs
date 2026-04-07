// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Migrations.Internal
{
    public class SingleStoreHistoryRepository(
        [NotNull] HistoryRepositoryDependencies dependencies,
        [NotNull] ISingleStoreOptions singleStoreOptions)
        : HistoryRepository(dependencies)
    {
        private const string MigrationsScript = nameof(MigrationsScript);

        private readonly SingleStoreSqlGenerationHelper _sqlGenerationHelper =
            (SingleStoreSqlGenerationHelper)dependencies.SqlGenerationHelper;

        private readonly string _lockConnectionString =
            dependencies.Connection.ConnectionString
            ?? dependencies.Connection.DbConnection.ConnectionString;

        private readonly int _migrationLockCommandTimeoutSeconds =
            ToCommandTimeoutSeconds(singleStoreOptions.MigrationLockTimeout);

        // ... rest of the class stays the same

        public override LockReleaseBehavior LockReleaseBehavior
            => LockReleaseBehavior.Explicit;

        public override IMigrationsDatabaseLock AcquireDatabaseLock()
        {
            Dependencies.MigrationsLogger.AcquiringMigrationLock();

            var lockConnection = CreateLockConnection();
            try
            {
                EnsureLockTable(lockConnection);

                var transaction = lockConnection.BeginTransaction();

                AcquireRowLock(lockConnection, transaction);

                return new SingleStoreMigrationDatabaseLock(this, lockConnection, transaction);
            }
            catch
            {
                lockConnection.Dispose();
                throw;
            }
        }

        public override async Task<IMigrationsDatabaseLock> AcquireDatabaseLockAsync(CancellationToken cancellationToken = default)
        {
            Dependencies.MigrationsLogger.AcquiringMigrationLock();

            var lockConnection = await CreateLockConnectionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await EnsureLockTableAsync(lockConnection, cancellationToken).ConfigureAwait(false);

                var transaction = await lockConnection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                await AcquireRowLockAsync(lockConnection, transaction, cancellationToken).ConfigureAwait(false);

                return new SingleStoreMigrationDatabaseLock(this, lockConnection, transaction, cancellationToken);
            }
            catch
            {
                await lockConnection.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Returns the name of the database-wide lock for migrations.
        /// </summary>
        protected virtual string GetDatabaseLockName(string databaseName)
            => $"__{databaseName}_EFMigrationsLock";

        private string GetLockTableName()
            => GetDatabaseLockName(Dependencies.Connection.DbConnection.Database);

        private static int ToCommandTimeoutSeconds(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timeout),
                    timeout,
                    "The migration lock timeout must be greater than zero.");
            }

            if (timeout.TotalSeconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timeout),
                    timeout,
                    $"The migration lock timeout must be less than or equal to {TimeSpan.FromSeconds(int.MaxValue)}.");
            }

            return checked((int)Math.Ceiling(timeout.TotalSeconds));
        }

        private string CreateLockConnectionString()
        {
            var databaseName = Dependencies.Connection.DbConnection.Database;
            if (string.IsNullOrEmpty(databaseName))
            {
                return _lockConnectionString;
            }

            var builder = new SingleStoreConnectionStringBuilder(_lockConnectionString)
            {
                Database = databaseName,
            };

            return builder.ConnectionString;
        }

        private SingleStoreConnection CreateLockConnection()
        {
            var connection = new SingleStoreConnection(CreateLockConnectionString());
            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        private async Task<SingleStoreConnection> CreateLockConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SingleStoreConnection(CreateLockConnectionString());
            try
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                return connection;
            }
            catch
            {
                await connection.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }

        private void EnsureLockTable(SingleStoreConnection connection)
        {
            var table = SqlGenerationHelper.DelimitIdentifier(GetLockTableName());

            using var command = connection.CreateCommand();
            command.CommandTimeout = _migrationLockCommandTimeoutSeconds;

            command.CommandText = $"""
CREATE ROWSTORE TABLE IF NOT EXISTS {table} (
  `Id` INT NOT NULL PRIMARY KEY
);
""";
            command.ExecuteNonQuery();

            command.CommandText = $"""INSERT IGNORE INTO {table} (`Id`) VALUES (1);""";
            command.ExecuteNonQuery();
        }

        private async Task EnsureLockTableAsync(SingleStoreConnection connection, CancellationToken cancellationToken)
        {
            var table = SqlGenerationHelper.DelimitIdentifier(GetLockTableName());

            using var command = connection.CreateCommand();
            command.CommandTimeout = _migrationLockCommandTimeoutSeconds;

            command.CommandText = $"""
CREATE ROWSTORE TABLE IF NOT EXISTS {table} (
  `Id` INT NOT NULL PRIMARY KEY
);
""";
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            command.CommandText = $"""INSERT IGNORE INTO {table} (`Id`) VALUES (1);""";
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        private void AcquireRowLock(SingleStoreConnection connection, DbTransaction transaction)
        {
            var table = SqlGenerationHelper.DelimitIdentifier(GetLockTableName());

            using var command = connection.CreateCommand();
            command.Transaction = (SingleStoreTransaction)transaction;
            command.CommandTimeout = _migrationLockCommandTimeoutSeconds;

            // Touch the single sentinel row inside the dedicated lock transaction.
            // If another migrator already holds the row lock, this statement waits until the
            // configured command timeout expires. Once acquired, the lock remains held until the
            // transaction/connection is disposed by SingleStoreMigrationDatabaseLock.
            command.CommandText = $"""UPDATE {table} SET `Id` = `Id` WHERE `Id` = 1;""";
            command.ExecuteNonQuery();
        }

        private async Task AcquireRowLockAsync(
            SingleStoreConnection connection,
            DbTransaction transaction,
            CancellationToken cancellationToken)
        {
            var table = SqlGenerationHelper.DelimitIdentifier(GetLockTableName());

            using var command = connection.CreateCommand();
            command.Transaction = (SingleStoreTransaction)transaction;
            command.CommandTimeout = _migrationLockCommandTimeoutSeconds;

            command.CommandText = $"""UPDATE {table} SET `Id` = `Id` WHERE `Id` = 1;""";
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override void ConfigureTable([NotNull] EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.HasCharSet(CharSet.Utf8Mb4);
        }

        protected override string ExistsSql
        {
            get
            {
                var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

                var builder = new StringBuilder();

                builder.Append("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE ");

                builder
                    .Append("TABLE_SCHEMA=")
                    .Append(
                        stringTypeMapping.GenerateSqlLiteral(
                            _sqlGenerationHelper.GetSchemaName(TableName, TableSchema) ??
                            Dependencies.Connection.DbConnection.Database))
                    .Append(" AND TABLE_NAME=")
                    .Append(
                        stringTypeMapping.GenerateSqlLiteral(
                            _sqlGenerationHelper.GetObjectName(TableName, TableSchema)))
                    .Append(";");

                return builder.ToString();
            }
        }

        protected override bool InterpretExistsResult(object value) => value != null;

        public override string GetCreateIfNotExistsScript()
        {
            var script = GetCreateScript();
            return script.Insert(script.IndexOf("CREATE TABLE", StringComparison.Ordinal) + 12, " IF NOT EXISTS");
        }

        public override string GetBeginIfNotExistsScript(string migrationId) => GetBeginIfScript(migrationId, true);

        public override string GetBeginIfExistsScript(string migrationId) => GetBeginIfScript(migrationId, false);

        public virtual string GetBeginIfScript(string migrationId, bool notExists) => $@"DROP PROCEDURE IF EXISTS {MigrationsScript};
DELIMITER //
CREATE PROCEDURE {MigrationsScript}()
BEGIN
    IF{(notExists ? " NOT" : null)} EXISTS(SELECT 1 FROM {SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema)} WHERE {SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName)} = '{migrationId}') THEN
";

        public override string GetEndIfScript() => $@"
    END IF;
END //
DELIMITER ;
CALL {MigrationsScript}();
DROP PROCEDURE {MigrationsScript};
";

        public virtual void ConfigureModel(ModelBuilder modelBuilder)
            => modelBuilder.HasCharSet(null, DelegationModes.ApplyToDatabases);

        #region Necessary implementation because we cannot directly override EnsureModel

        private IModel _model;
        private string _migrationIdColumnName;
        private string _productVersionColumnName;

        protected virtual IModel EnsureModel()
        {
            if (_model == null)
            {
                var conventionSet = Dependencies.ConventionSetBuilder.CreateConventionSet();

                ConventionSet.Remove(conventionSet.ModelInitializedConventions, typeof(DbSetFindingConvention));
                ConventionSet.Remove(conventionSet.ModelInitializedConventions, typeof(RelationalDbFunctionAttributeConvention));

                var modelBuilder = new ModelBuilder(conventionSet);

                ConfigureModel(modelBuilder);

                modelBuilder.Entity<HistoryRow>(
                    x =>
                    {
                        ConfigureTable(x);
                        x.ToTable(TableName, TableSchema);
                    });

                _model = Dependencies.ModelRuntimeInitializer.Initialize(modelBuilder.FinalizeModel(), designTime: true, validationLogger: null);
            }

            return _model;
        }

        public override string GetCreateScript()
        {
            var model = EnsureModel();

            var operations = Dependencies.ModelDiffer.GetDifferences(null, model.GetRelationalModel());
            var commandList = Dependencies.MigrationsSqlGenerator.Generate(operations, model);

            return string.Concat(commandList.Select(c => c.CommandText));
        }

        protected override string MigrationIdColumnName
            => _migrationIdColumnName ??= EnsureModel()
                .FindEntityType(typeof(HistoryRow))!
                .FindProperty(nameof(HistoryRow.MigrationId))!
                .GetColumnName();

        protected override string ProductVersionColumnName
            => _productVersionColumnName ??= EnsureModel()
                .FindEntityType(typeof(HistoryRow))!
                .FindProperty(nameof(HistoryRow.ProductVersion))!
                .GetColumnName();

        #endregion Necessary implementation because we cannot directly override EnsureModel

        private sealed class SingleStoreMigrationDatabaseLock(
            SingleStoreHistoryRepository historyRepository,
            SingleStoreConnection lockConnection,
            DbTransaction lockTransaction,
            CancellationToken cancellationToken = default)
            : IMigrationsDatabaseLock
        {
            public IHistoryRepository HistoryRepository => historyRepository;

            public void Dispose()
            {
                try
                {
                    lockTransaction.Rollback();
                }
                catch
                {
                    // Ignore rollback failures while releasing the migrations lock.
                }

                lockTransaction.Dispose();
                lockConnection.Dispose();
            }

            public async ValueTask DisposeAsync()
            {
                try
                {
                    await lockTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // Ignore rollback failures while releasing the migrations lock.
                }

                await lockTransaction.DisposeAsync().ConfigureAwait(false);
                await lockConnection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
