// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using SingleStoreConnector;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides extension methods on <see cref="DbContextOptionsBuilder"/> and <see cref="DbContextOptionsBuilder{T}"/>
    /// to configure a <see cref="DbContext"/> to use with MySQL/MariaDB and EntityFrameworkCore.SingleStore.
    /// </summary>
    public static class SingleStoreDbContextOptionsBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a MySQL compatible database, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseSingleStore(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [CanBeNull] Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            optionsBuilder.AddInterceptors(new MatchInterceptor());
            ServerVersion serverVersion = SingleStoreServerVersion.LatestSupportedServerVersion;
            var extension = (SingleStoreOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithServerVersion(serverVersion);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            mySqlOptionsAction?.Invoke(new SingleStoreDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseSingleStore(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            optionsBuilder.AddInterceptors(new MatchInterceptor());
            var resolvedConnectionString = new NamedConnectionStringResolver(optionsBuilder.Options)
                .ResolveConnectionString(connectionString);

            var program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var csb = new SingleStoreConnectionStringBuilder(resolvedConnectionString)
            {
                AllowUserVariables = true,
                UseAffectedRows = false,
                ConnectionAttributes = $"program_name:SingleStore Entity Framework Core provider, program_version:{program_version}",
            };

            resolvedConnectionString = csb.ConnectionString;
            ServerVersion serverVersion;

            try
            {
                serverVersion = ServerVersion.AutoDetect(connectionString);
            }
            // There might occur different types of Exceptions while trying to AutoDetect() server version.
            // This includes: SocketException (when we're unable to connect to any of the specified hosts),
            // InvalidOperationException (unable to determine server version from version string), etc.
            // In this case the latest supported server version will be used, therefore we catch any Exception.
            catch (Exception)
            {
                serverVersion = SingleStoreServerVersion.LatestSupportedServerVersion;
            }

            var extension = (SingleStoreOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithServerVersion(serverVersion)
                .WithConnectionString(resolvedConnectionString);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            mySqlOptionsAction?.Invoke(new SingleStoreDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseSingleStore(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            optionsBuilder.AddInterceptors(new MatchInterceptor());
            var resolvedConnectionString = connection.ConnectionString is not null
                ? new NamedConnectionStringResolver(optionsBuilder.Options)
                    .ResolveConnectionString(connection.ConnectionString)
                : null;

            var csb = new SingleStoreConnectionStringBuilder(resolvedConnectionString);
            var program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            csb.ConnectionAttributes = $"program_name:SingleStore Entity Framework Core provider, program_version:{program_version}";

            if (!csb.AllowUserVariables ||
                csb.UseAffectedRows)
            {
                try
                {
                    csb.AllowUserVariables = true;
                    csb.UseAffectedRows = false;

                    connection.ConnectionString = csb.ConnectionString;
                }
                catch (SingleStoreException e)
                {
                    throw new InvalidOperationException(
                        @"The connection string used with EntityFrameworkCore.SingleStore must contain ""AllowUserVariables=true;UseAffectedRows=false"".",
                        e);
                }
            }

            ServerVersion serverVersion;

            try
            {
                serverVersion = ServerVersion.AutoDetect(connection.ConnectionString);
            }
            // There might occur different types of Exceptions while trying to AutoDetect() server version.
            // This includes: SocketException (when we're unable to connect to any of the specified hosts),
            // InvalidOperationException (unable to determine server version from version string), etc.
            // In this case the latest supported server version will be used, therefore we catch any Exception.
            catch (Exception)
            {
                serverVersion = SingleStoreServerVersion.LatestSupportedServerVersion;
            }

            var extension = (SingleStoreOptionsExtension)GetOrCreateExtension(optionsBuilder)
                .WithServerVersion(serverVersion)
                .WithConnection(connection);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            ConfigureWarnings(optionsBuilder);
            mySqlOptionsAction?.Invoke(new SingleStoreDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a MySQL compatible database, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseSingleStore<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [CanBeNull] Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseSingleStore(
                (DbContextOptionsBuilder)optionsBuilder, mySqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connectionString"> The connection string of the database to connect to. </param>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseSingleStore<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseSingleStore(
                (DbContextOptionsBuilder)optionsBuilder, connectionString, mySqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a MySQL compatible database.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <typeparam name="TContext"> The type of context to be configured. </typeparam>
        /// <param name="mySqlOptionsAction"> An optional action to allow additional MySQL specific configuration. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseSingleStore<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<SingleStoreDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseSingleStore(
                (DbContextOptionsBuilder)optionsBuilder, connection, mySqlOptionsAction);

        private static SingleStoreOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<SingleStoreOptionsExtension>()
               ?? new SingleStoreOptionsExtension();

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }

        private sealed class MatchInterceptor : DbCommandInterceptor
        {
            public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> resultDbCommand)
            {
                ProcessMatchQuery(command);
                return base.ReaderExecuting(command, eventData, resultDbCommand);
            }

            public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> resultDbCommand, CancellationToken cancellationToken = new CancellationToken())
            {
                ProcessMatchQuery(command);
                return base.ReaderExecutingAsync(command, eventData, resultDbCommand, cancellationToken);
            }

            private static void ProcessMatchQuery(DbCommand command)
            {
                var sql = command.CommandText;

                if (sql.Contains("MATCH"))
                {
                    var table = ExtractTableNameFromSqlCommandText(sql);

                    // Execute the OPTIMIZE TABLE statement separately
                    using (var optimizeTableCommand = command.Connection.CreateCommand())
                    {
                        optimizeTableCommand.Transaction = command.Transaction; // Make sure to use the same transaction if there's any
                        optimizeTableCommand.CommandText = $"OPTIMIZE TABLE {table} FLUSH";
                        optimizeTableCommand.ExecuteNonQuery();
                    }
                }
            }

            private static string ExtractTableNameFromSqlCommandText(string sqlCommandText)
            {
                if (Regex.IsMatch(sqlCommandText, @"MATCH", RegexOptions.IgnoreCase))
                {
                    // This regex pattern searches for the table name in a SQL query after either a FROM, JOIN, or UPDATE keyword,
                    // and matches table names enclosed in square brackets, backticks, or without any delimiters.
                    // The table name is captured using a named group called <table>.
                    var match = Regex.Match(sqlCommandText, @"(?:FROM|JOIN|UPDATE)\s+(?:\[(?<schema>.*?)\]\.\[(?<table>.*?)\]|`(?<table>[\w\.]+)`|(?<table>[\w\.]+))",
                        RegexOptions.IgnoreCase);

                    if (match.Success && match.Groups["table"].Success)
                    {
                        return match.Groups["table"].Value;
                    }
                }

                return string.Empty;
            }
        }
    }
}
