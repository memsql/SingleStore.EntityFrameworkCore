using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreFixture : ServiceProviderFixtureBase, IDisposable
    {
        internal List<string> CreatedDatabases { get; } = new();

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        public void Dispose()
        {
            using (var master = new SingleStoreConnection(AppConfig.ConnectionString))
            {
                master.Open();

                foreach (var db in CreatedDatabases)
                {
                    var command = (SingleStoreCommand)master.CreateCommand();
                    command.CommandText = $@"DROP DATABASE IF EXISTS `{db}`;";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
