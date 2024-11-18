// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public class SingleStoreRelationalConnection : RelationalConnection, ISingleStoreRelationalConnection
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly SingleStoreOptionsExtension _mySqlOptionsExtension;

        // ReSharper disable once VirtualMemberCallInConstructor
        public SingleStoreRelationalConnection(RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
            _mySqlOptionsExtension = Dependencies.ContextOptions.FindExtension<SingleStoreOptionsExtension>() ?? new SingleStoreOptionsExtension();
        }

        // TODO: Remove, because we don't use it anywhere.
        private bool IsMasterConnection { get; set; }

        protected override DbConnection CreateDbConnection()
            => new SingleStoreConnection(AddConnectionStringOptions(new SingleStoreConnectionStringBuilder(ConnectionString!)).ConnectionString);

        public virtual ISingleStoreRelationalConnection CreateMasterConnection()
        {
            // Add master connection specific options.
            var csb = new SingleStoreConnectionStringBuilder(ConnectionString!)
            {
                Database = string.Empty,
                Pooling = false,
            };

            csb = AddConnectionStringOptions(csb);

            var connectionString = csb.ConnectionString;
            var relationalOptions = RelationalOptionsExtension.Extract(Dependencies.ContextOptions);

            // Apply modified connection string.
            relationalOptions = relationalOptions.Connection is null
                ? relationalOptions.WithConnectionString(connectionString)
                : relationalOptions.WithConnection(DbConnection.CloneWith(connectionString));

            var optionsBuilder = new DbContextOptionsBuilder();
            var optionsBuilderInfrastructure = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;

            optionsBuilderInfrastructure.AddOrUpdateExtension(relationalOptions);

            return new SingleStoreRelationalConnection(Dependencies with { ContextOptions = optionsBuilder.Options })
            {
                IsMasterConnection = true
            };
        }

        [AllowNull]
        public new virtual SingleStoreConnection DbConnection
        {
            get => (SingleStoreConnection)base.DbConnection;
            set => base.DbConnection = value;
        }

        protected virtual SingleStoreConnectionStringBuilder AddConnectionStringOptions(SingleStoreConnectionStringBuilder builder)
        {
            var program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            builder.ConnectionAttributes = $"program_name:SingleStore Entity Framework Core provider, program_version:{program_version}";

            if (CommandTimeout != null)
            {
                builder.DefaultCommandTimeout = (uint)CommandTimeout.Value;
            }

            if (_mySqlOptionsExtension.NoBackslashEscapes)
            {
                builder.NoBackslashEscapes = true;
            }

            var boolHandling = _mySqlOptionsExtension.DefaultDataTypeMappings?.ClrBoolean;
            switch (boolHandling)
            {
                case null:
                    // Just keep using whatever is already defined in the connection string.
                    break;

                case SingleStoreBooleanType.Default:
                case SingleStoreBooleanType.TinyInt1:
                    builder.TreatTinyAsBoolean = true;
                    break;

                case SingleStoreBooleanType.None:
                case SingleStoreBooleanType.Bit1:
                    builder.TreatTinyAsBoolean = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return builder;
        }

        protected override bool SupportsAmbientTransactions => true;

        // CHECK: Is this obsolete or has it been moved somewhere else?
        // public override bool IsMultipleActiveResultSetsEnabled => false;

        public override void EnlistTransaction(Transaction transaction)
        {
            try
            {
                base.EnlistTransaction(transaction);
            }
            catch (SingleStoreException e)
            {
                if (e.Message == "Already enlisted in a Transaction.")
                {
                    // Return expected exception type.
                    throw new InvalidOperationException(e.Message, e);
                }

                throw;
            }
        }

        public override bool Open(bool errorsExpected = false)
        {
            var result = base.Open(errorsExpected);

            if (result)
            {
                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    AddSqlMode(NoBackslashEscapes);
                }
            }

            return result;
        }

        public override async Task<bool> OpenAsync(CancellationToken cancellationToken, bool errorsExpected = false)
        {
            var result = await base.OpenAsync(cancellationToken, errorsExpected)
                .ConfigureAwait(false);

            if (result)
            {
                if (_mySqlOptionsExtension.UpdateSqlModeOnOpen && _mySqlOptionsExtension.NoBackslashEscapes)
                {
                    await AddSqlModeAsync(NoBackslashEscapes)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        public virtual void AddSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolated($@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', {mode});");

        public virtual Task AddSqlModeAsync(string mode, CancellationToken cancellationToken = default)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolatedAsync($@"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', {mode});", cancellationToken);

        public virtual void RemoveSqlMode(string mode)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolated($@"SET SESSION sql_mode = REPLACE(@@sql_mode, {mode}, '');");

        public virtual void RemoveSqlModeAsync(string mode, CancellationToken cancellationToken = default)
            => Dependencies.CurrentContext.Context?.Database.ExecuteSqlInterpolatedAsync($@"SET SESSION sql_mode = REPLACE(@@sql_mode, {mode}, '');", cancellationToken);
    }
}
