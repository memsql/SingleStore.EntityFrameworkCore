using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
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

        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkSingleStore()
            .BuildServiceProvider();

        protected ConnectionMysqlContext CreateContext(string connectionString)
            => new ConnectionMysqlContext(_serviceProvider, connectionString);
    }

    public class ConnectionMysqlContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _connectionString;

        public ConnectionMysqlContext(IServiceProvider serviceProvider, string connectionString)
        {
            _serviceProvider = serviceProvider;
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSingleStore(_connectionString)
                .UseInternalServiceProvider(_serviceProvider);
    }
}
