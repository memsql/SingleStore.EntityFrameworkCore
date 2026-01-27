using System;
using System.Linq;
using System.Reflection;
using Xunit;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreConnectionStringOptionsValidatorTests
    {
        private readonly SingleStoreConnectionStringOptionsValidator _validator
            = new();

        [Fact]
        public void EnsureMandatoryOptions_AppendsConnectionAttributesWithoutRemovingExisting()
        {
            var attrs = "foo:bar,baz:qux";
            var cs = AppConfig.ConnectionString + ";" + $"ConnectionAttributes={attrs}";

            var changed = _validator.EnsureMandatoryOptions(ref cs);

            Assert.True(changed);
            var newCsb = new SingleStoreConnectionStringBuilder(cs);
            var parts = newCsb.ConnectionAttributes
                .TrimEnd(',')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            Assert.Contains("foo:bar", parts);
            Assert.Contains("baz:qux", parts);
            var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;
            Assert.Contains(parts, p => p.StartsWith("_connector_name:SingleStore Entity Framework Core provider"));
            Assert.Contains(parts, p => p.StartsWith($"_connector_version:{programVersion}"));
        }

        [Fact]
        public void EnsureMandatoryOptions_DoesNotDuplicateConnectionAttributes_OnRepeatedCalls()
        {
            var cs = AppConfig.ConnectionString;

            Assert.True(_validator.EnsureMandatoryOptions(ref cs));
            Assert.False(_validator.EnsureMandatoryOptions(ref cs));

            var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;

            var parts = new SingleStoreConnectionStringBuilder(cs)
                .ConnectionAttributes
                .TrimEnd(',')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            Assert.Equal(1, parts.Count(p => p.StartsWith("_connector_name:SingleStore Entity Framework Core provider")));
            Assert.Equal(1, parts.Count(p => p.StartsWith($"_connector_version:{programVersion}")));
        }

        [Fact]
        public void EnsureMandatoryOptions_SetsMandatoryFlags_WhenMissing()
        {
            var builder = new SingleStoreConnectionStringBuilder(AppConfig.ConnectionString)
            {
                AllowUserVariables = false,
                UseAffectedRows  = true
            };
            var cs = builder.ConnectionString;

            var changed = _validator.EnsureMandatoryOptions(ref cs);

            Assert.True(changed);
            var result = new SingleStoreConnectionStringBuilder(cs);
            Assert.True(result.AllowUserVariables);
            Assert.False(result.UseAffectedRows);
        }

        [Fact]
        public void EnsureMandatoryOptions_DbConnection_InjectsAttributesAndFlags()
        {
            using var conn = new SingleStoreConnection(AppConfig.ConnectionString);

            var changed = _validator.EnsureMandatoryOptions(conn);

            Assert.True(changed);
            var result = new SingleStoreConnectionStringBuilder(conn.ConnectionString);
            Assert.True(result.AllowUserVariables);
            Assert.False(result.UseAffectedRows);
            Assert.Contains("_connector_name:SingleStore Entity Framework Core provider", result.ConnectionAttributes);
            var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;
            Assert.Contains($"_connector_version:{programVersion}", result.ConnectionAttributes);
        }

        [Fact]
        public void EnsureMandatoryOptions_DbDataSource_ThrowsWhenMissingMandatoryOptions()
        {
            var dataSource = new SingleStoreDataSourceBuilder(AppConfig.ConnectionString).Build();

            var ex = Assert.Throws<InvalidOperationException>(
                () => _validator.EnsureMandatoryOptions(dataSource));
            Assert.Contains("AllowUserVariables=True;UseAffectedRows=False", ex.Message);
        }
    }
}
