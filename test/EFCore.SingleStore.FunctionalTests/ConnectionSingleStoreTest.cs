using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class ConnectionSingleStoreTest
    {
        [ConditionalFact]
        public virtual void SetConnectionString()
        {
            var correctConnectionString = SingleStoreTestStore.CreateConnectionString("ConnectionTest");

            var csb = new SingleStoreConnectionStringBuilder(correctConnectionString);
            var correctPort = csb.Port;

            // Set an incorrect port, where no database server is listening.
            csb.Port = 65123;

            var incorrectConnectionString = csb.ConnectionString;
            using var context = CreateContext(incorrectConnectionString);

            context.Database.SetConnectionString(correctConnectionString);

            var connection = (SingleStoreConnection)context.Database.GetDbConnection();
            csb = new SingleStoreConnectionStringBuilder(connection.ConnectionString);

            Assert.Equal(csb.Port, correctPort);
        }

        [ConditionalFact]
        public virtual void SetConnectionString_affects_master_connection()
        {
            var correctConnectionString = SingleStoreTestStore.CreateConnectionString("ConnectionTest");

            // Set an incorrect port, where no database server is listening.
            var csb = new SingleStoreConnectionStringBuilder(correctConnectionString) { Port = 65123 };

            var incorrectConnectionString = csb.ConnectionString;
            using var context = CreateContext(incorrectConnectionString);

            context.Database.SetConnectionString(correctConnectionString);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [ConditionalFact]
        public virtual void ConnectionAttributes()
        {
            var attrs = "ConnectionAttributes=my_key_1:my_val_1,my_key_2:my_val_2";
            using var connection = new SingleStoreConnection(AppConfig.ConnectionString + ";" + attrs);
            connection.Open();

            if (AppConfig.ServerVersion.Supports.ConnectionAttributes)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT ATTRIBUTE_NAME, ATTRIBUTE_VALUE FROM information_schema.mv_connection_attributes;";

                using var reader = cmd.ExecuteReader();
                HashSet<Tuple<string, string>> dbConnAttrs = new HashSet<Tuple<string, string>>();
                while (reader.Read())
                {
                    dbConnAttrs.Add(new Tuple<string, string>(reader.GetString(0), reader.GetString(1)));
                }

                Tuple<string, string> expectedPair = new Tuple<string, string>("my_key_1", "my_val_1");
                Assert.Contains(expectedPair, dbConnAttrs);
                expectedPair = new Tuple<string, string>("my_key_2", "my_val_2");
                Assert.Contains(expectedPair, dbConnAttrs);
            }
        }

        [Fact]
        public async Task UseSingleStore_IncludesConnectorAttributes_InConnectionString()
        {
            await using var _ = await ((SingleStoreTestStore)SingleStoreNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionAttributesTest"))
                .InitializeAsync(null, (Func<DbContext>)null);

            var cs = SingleStoreTestStore.CreateConnectionString("ConnectionAttributesTest");
            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseSingleStore(cs, b => b.ApplyConfiguration());

            using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var conn = (SingleStoreConnection)context.Database.GetDbConnection();
            var csb  = new SingleStoreConnectionStringBuilder(conn.ConnectionString);

            var parts = csb.ConnectionAttributes
                .TrimEnd(',')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            var programVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Assert.Contains(parts, p => p.StartsWith("_connector_name:SingleStore Entity Framework Core provider"));
            Assert.Contains(parts, p => p.StartsWith($"_connector_version:{programVersion}"));
        }

        [Fact]
        public async Task Can_create_admin_connection_with_data_source()
        {
            await using var _ = await ((SingleStoreTestStore)SingleStoreNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionTest"))
                .InitializeAsync(null, (Func<DbContext>)null);

            await using var dataSource = new SingleStoreDataSourceBuilder(SingleStoreTestStore.CreateConnectionString("ConnectionTest")).Build();

            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseSingleStore(dataSource, b => b.ApplyConfiguration());
            await using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalConnection = context.GetService<ISingleStoreRelationalConnection>();
            await using var masterConnection = relationalConnection.CreateMasterConnection();

            Assert.Equal(string.Empty, new SingleStoreConnectionStringBuilder(masterConnection.ConnectionString).Database);

            await masterConnection.OpenAsync(default);
        }

        [Fact]
        public async Task Can_create_admin_connection_with_connection_string()
        {
            await using var _ = await ((SingleStoreTestStore)SingleStoreNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionTest"))
                .InitializeAsync(null, (Func<DbContext>)null);

            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseSingleStore(SingleStoreTestStore.CreateConnectionString("ConnectionTest"),
                b => b.ApplyConfiguration());
            await using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalConnection = context.GetService<ISingleStoreRelationalConnection>();
            await using var masterConnection = relationalConnection.CreateMasterConnection();

            Assert.Equal(string.Empty, new SingleStoreConnectionStringBuilder(masterConnection.ConnectionString).Database);

            await masterConnection.OpenAsync(default);
        }

        [Fact]
        public async Task Can_create_admin_connection_with_connection()
        {
            await using var _ = await ((SingleStoreTestStore)SingleStoreNorthwindTestStoreFactory.Instance
                    .GetOrCreate("ConnectionTestWithConnection"))
                .InitializeAsync(null, (Func<DbContext>)null);

            await using var connection = new SingleStoreConnection(SingleStoreTestStore.CreateConnectionString("ConnectionTestWithConnection"));

            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseSingleStore(connection, b => b.ApplyConfiguration());
            await using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalConnection = context.GetService<ISingleStoreRelationalConnection>();
            await using var masterConnection = relationalConnection.CreateMasterConnection();

            Assert.Equal(string.Empty, new SingleStoreConnectionStringBuilder(masterConnection.ConnectionString).Database);

            await masterConnection.OpenAsync(default);
        }


        [ConditionalFact(Skip = "Feature 'Non-underscore or alphanumeric characters in database name, or name starts with digit' is not supported by SingleStore")]
        public void Can_create_database_with_disablebackslashescaping()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GeneralOptionsContext>();
            optionsBuilder.UseSingleStore(SingleStoreTestStore.CreateConnectionString("ConnectionTest_" + Guid.NewGuid()), b => b.ApplyConfiguration().DisableBackslashEscaping());
            using var context = new GeneralOptionsContext(optionsBuilder.Options);

            var relationalDatabaseCreator = context.GetService<IRelationalDatabaseCreator>();

            try
            {
                relationalDatabaseCreator.EnsureCreated();
            }
            finally
            {
                try
                {
                    relationalDatabaseCreator.EnsureDeleted();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkSingleStore()
            .BuildServiceProvider();

        protected ConnectionSingleStoreContext CreateContext(string connectionString)
            => new ConnectionSingleStoreContext(_serviceProvider, connectionString);


        public class ConnectionSingleStoreContext : DbContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly string _connectionString;

            public ConnectionSingleStoreContext(IServiceProvider serviceProvider, string connectionString)
            {
                _serviceProvider = serviceProvider;
                _connectionString = connectionString;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseSingleStore(_connectionString)
                    .UseInternalServiceProvider(_serviceProvider);
        }

        public class GeneralOptionsContext : DbContext
        {
            public GeneralOptionsContext(DbContextOptions<GeneralOptionsContext> options)
                : base(options)
            {
            }
        }
    }
}
