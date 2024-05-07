using System;
using Microsoft.Extensions.Configuration;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public static class TestEnvironment
    {
        public static IConfiguration Config => AppConfig.Config;
        public static string DefaultConnection => AppConfig.ConnectionString;

        public static bool IsCI { get; } = Environment.GetEnvironmentVariable("PIPELINE_WORKSPACE") != null
                                           || Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;

        static TestEnvironment()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }

        public static string GetDefaultConnectionString(string databaseName)
            => new SingleStoreConnectionStringBuilder(DefaultConnection)
            {
                Database = databaseName
            }.ConnectionString;

        public static int? GetInt(string key)
            => int.TryParse(Config[key], out var value) ? value : (int?)null;
    }
}
