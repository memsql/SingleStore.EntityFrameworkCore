using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Configuration;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public class SingleStoreTestStore : RelationalTestStore
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        public const int DefaultCommandTimeout = 600;

        private readonly string _scriptPath;
        private readonly bool _useConnectionString;
        private readonly bool _noBackslashEscapes;

        protected override string OpenDelimiter => "`";
        protected override string CloseDelimiter => "`";

        public static SingleStoreTestStore GetOrCreate(string name, bool useConnectionString = false, bool noBackslashEscapes = false, string databaseCollation = null, SingleStoreGuidFormat guidFormat = SingleStoreGuidFormat.Default)
            => new SingleStoreTestStore(name, useConnectionString: useConnectionString, shared: true, noBackslashEscapes: noBackslashEscapes, databaseCollation: databaseCollation, guidFormat: guidFormat);

        public static SingleStoreTestStore GetOrCreate(string name, string scriptPath, bool noBackslashEscapes = false, string databaseCollation = null, SingleStoreGuidFormat guidFormat = SingleStoreGuidFormat.Default)
            => new SingleStoreTestStore(name, scriptPath: scriptPath, noBackslashEscapes: noBackslashEscapes, databaseCollation: databaseCollation, guidFormat: guidFormat);

        public static SingleStoreTestStore GetOrCreateInitialized(string name)
            => new SingleStoreTestStore(name, shared: true).InitializeSingleStore(null, (Func<DbContext>)null, null);

        public static SingleStoreTestStore Create(string name, bool useConnectionString = false, bool noBackslashEscapes = false, string databaseCollation = null, SingleStoreGuidFormat guidFormat = SingleStoreGuidFormat.Default)
            => new SingleStoreTestStore(name, useConnectionString: useConnectionString, shared: false, noBackslashEscapes: noBackslashEscapes, databaseCollation: databaseCollation, guidFormat: guidFormat);

        public static SingleStoreTestStore CreateInitialized(string name)
            => new SingleStoreTestStore(name, shared: false).InitializeSingleStore(null, null, null);

        public static SingleStoreTestStore RecreateInitialized(string name)
            => new SingleStoreTestStore(name, shared: false).InitializeSingleStore(null, null, null, c =>
            {
                c.Database.EnsureDeleted();
                c.Database.EnsureCreated();
            });

        public Lazy<ServerVersion> ServerVersion { get; }
        public string DatabaseCharSet { get; }
        public string DatabaseCollation { get; set; }

        // ReSharper disable VirtualMemberCallInConstructor
        private SingleStoreTestStore(
            string name,
            string databaseCharSet = null,
            string databaseCollation = null,
            bool useConnectionString = false,
            string scriptPath = null,
            bool shared = true,
            bool noBackslashEscapes = false,
            SingleStoreGuidFormat guidFormat = SingleStoreGuidFormat.Default)
            : base(name, shared)
        {
            _useConnectionString = useConnectionString;
            _noBackslashEscapes = noBackslashEscapes;
            ConnectionString = CreateConnectionString(name, _noBackslashEscapes, guidFormat);
            Connection = new SingleStoreConnection(ConnectionString);
            ServerVersion = new Lazy<ServerVersion>(() => Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect((SingleStoreConnection)Connection));
            DatabaseCharSet = databaseCharSet ?? ServerVersion.Value.DefaultCharSet;
            DatabaseCollation = databaseCollation ?? ServerVersion.Value.DefaultUtf8CiCollation;

            if (scriptPath != null)
            {
                _scriptPath = Path.Combine(
                    Path.GetDirectoryName(typeof(SingleStoreTestStore).GetTypeInfo().Assembly.Location) ?? string.Empty,
                    scriptPath);
            }
        }

        public static string CreateConnectionString(string name, bool noBackslashEscapes = false, SingleStoreGuidFormat guidFormat = SingleStoreGuidFormat.Default)
            => new SingleStoreConnectionStringBuilder(AppConfig.ConnectionString)
            {
                Database = name,
                DefaultCommandTimeout = (uint)GetCommandTimeout(),
                NoBackslashEscapes = noBackslashEscapes,
                PersistSecurityInfo = true, // needed by some tests to not leak a broken connection into the following tests
                GuidFormat = guidFormat,
                AllowUserVariables = true,
                UseAffectedRows = false,
            }.ConnectionString;

        private static int GetCommandTimeout() => AppConfig.Config.GetValue("Data:CommandTimeout", DefaultCommandTimeout);

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => _useConnectionString
                ? builder.UseSingleStore(ConnectionString, x => AddOptions(x, _noBackslashEscapes))
                : builder.UseSingleStore(Connection, x => AddOptions(x, _noBackslashEscapes));

        public static SingleStoreDbContextOptionsBuilder AddOptions(SingleStoreDbContextOptionsBuilder builder)
        {
            return builder
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                .CommandTimeout(GetCommandTimeout())
                .ExecutionStrategy(d => new TestSingleStoreRetryingExecutionStrategy(d));
            // .EnableIndexOptimizedBooleanColumns(); // TODO: Activate for all test for .NET 5. Tests should use
            //       `ONLY_FULL_GROUP_BY` to ensure correct working of the
            //       expression visitor in all cases, which is blocked by
            //       #1167 for MariaDB.
        }

        public static void AddOptions(SingleStoreDbContextOptionsBuilder builder, bool noBackslashEscapes)
        {
            AddOptions(builder);
            if (noBackslashEscapes)
            {
                builder.DisableBackslashEscaping();
            }
        }

        public SingleStoreTestStore InitializeSingleStore(IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean = null)
            => (SingleStoreTestStore)Initialize(serviceProvider, createContext, seed, clean);

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
        {
            if (CreateDatabase(clean))
            {
                if (_scriptPath != null)
                {
                    ExecuteScript(new FileInfo(_scriptPath));
                }
                else
                {
                    using (var context = createContext())
                    {
                        context.Database.EnsureCreatedResiliently();
                        seed?.Invoke(context);
                    }
                }
            }
        }

        private bool CreateDatabase(Action<DbContext> clean)
        {
            if (DatabaseExists(Name))
            {
                if (_scriptPath != null && !TestEnvironment.IsCI)
                {
                    return false;
                }

                using (var context = new DbContext(
                    AddProviderOptions(
                            new DbContextOptionsBuilder()
                                .EnableServiceProviderCaching(false))
                        .Options))
                {
                    clean?.Invoke(context);
                    Clean(context);
                    return true;
                }
            }

            using (var master = new SingleStoreConnection(CreateAdminConnectionString()))
            {
                master.Open();
                ExecuteNonQuery(master, GetCreateDatabaseStatement(Name, DatabaseCharSet, DatabaseCollation));
            }

            return true;
        }

        public override void Dispose()
        {
            DeleteDatabase();
            ChangeTableType();
        }

        private void DeleteDatabase()
        {
            // TODO: PLAT-6404 (Investigate how to drop the databases that aren't created by SingleStoreTestStore)
            string[] databases = {"TwoDatabasesOne", "TwoDatabasesIntercept", "TwoDatabasesOneB",
                "TwoDatabasesTwo", "TwoDatabasesTwoB", "ConnectionTest", "ConnectionSettings", "SeedsA", "SeedsS", $"{Name}"};

            using (var master = new SingleStoreConnection(CreateAdminConnectionString()))
            {
                foreach (var db in databases)
                {
                    ExecuteNonQuery(master, $@"DROP DATABASE IF EXISTS `{db}`;");
                }
            }
        }

        private void ChangeTableType()
        {
            using var master = new SingleStoreConnection(CreateAdminConnectionString());
            ExecuteNonQuery(master, "set global default_table_type = columnstore;");
        }

        private static string GetCreateDatabaseStatement(string name, string charset = null, string collation = null)
            => $@"CREATE DATABASE `{name}`{(string.IsNullOrEmpty(charset) ? null : $" CHARACTER SET {charset}")}{(string.IsNullOrEmpty(collation) ? null : $" COLLATE {collation}")};";

        private static bool DatabaseExists(string name)
        {
            using (var master = new SingleStoreConnection(CreateAdminConnectionString()))
                return ExecuteScalar<long>(master, $@"SELECT COUNT(*) FROM `INFORMATION_SCHEMA`.`SCHEMATA` WHERE `SCHEMA_NAME` = '{name}';") > 0;
        }

        private static string CreateAdminConnectionString()
            => CreateConnectionString(null);

        public void ExecuteScript(FileInfo scriptFile)
            => ExecuteScript(File.ReadAllText(scriptFile.FullName));

        public void ExecuteScript(string script)
            => Execute(
                Connection, command =>
                {
                    foreach (var batch in
                        new Regex(@"^/\*\s*GO\s*\*/", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0))
                            .Split(script)
                            .Where(b => !string.IsNullOrEmpty(b)))
                    {
                        command.CommandText = batch;
                        command.ExecuteNonQuery();
                    }

                    return 0;
                }, string.Empty);

        public override void Clean(DbContext context)
            => context.Database.EnsureClean();

        private static T ExecuteScalar<T>(DbConnection connection, string sql, params object[] parameters)
            => Execute(connection, command => (T)command.ExecuteScalar(), sql, false, parameters);

        private static T Execute<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql,
            bool useTransaction = false, object[] parameters = null)
            => ExecuteCommand(connection, execute, sql, useTransaction, parameters);

        private static T ExecuteCommand<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql, bool useTransaction, object[] parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            connection.Open();

            try
            {
                using (var transaction = useTransaction
                    ? connection.BeginTransaction()
                    : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        command.Transaction = transaction;
                        result = execute(command);
                    }

                    transaction?.Commit();

                    return result;
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        public int ExecuteNonQuery(string sql, params object[] parameters)
            => ExecuteNonQuery(Connection, sql, parameters);

        private static int ExecuteNonQuery(DbConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command => command.ExecuteNonQuery(), sql, false, parameters);

        public override void OpenConnection()
        {
            base.OpenConnection();

            if (_noBackslashEscapes)
            {
                AppendToSqlMode(NoBackslashEscapes, (SingleStoreConnection)Connection);
            }
        }

        public override async Task OpenConnectionAsync()
        {
            await base.OpenConnectionAsync();

            if (_noBackslashEscapes)
            {
                await AppendToSqlModeAsync(NoBackslashEscapes, (SingleStoreConnection)Connection);
            }
        }

        private static DbCommand CreateCommand(DbConnection connection, string commandText, object[] parameters)
        {
            var command = (SingleStoreCommand)connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = GetCommandTimeout();

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue("@p" + i, parameters[i]);
                }
            }

            return command;
        }

        public virtual void AppendToSqlMode(string mode, SingleStoreConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0);";
            command.Parameters.Add(new SingleStoreParameter("@p0", mode));

            command.ExecuteNonQuery();
        }

        public virtual Task AppendToSqlModeAsync(string mode, SingleStoreConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0);";
            command.Parameters.Add(new SingleStoreParameter("@p0", mode));

            return command.ExecuteNonQueryAsync();
        }
    }
}
