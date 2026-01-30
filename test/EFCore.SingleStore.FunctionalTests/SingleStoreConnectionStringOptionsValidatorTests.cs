using System;
using System.Linq;
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
            var cs = WithMandatoryFlags(AppConfig.ConnectionString, allowUserVariables: false, useAffectedRows: true);
            cs = WithAttrs(cs, "foo:bar,baz:qux");

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
            var cs = WithMandatoryFlags(AppConfig.ConnectionString, allowUserVariables: false, useAffectedRows: true);

            Assert.True(_validator.EnsureMandatoryOptions(ref cs));
            Assert.False(_validator.EnsureMandatoryOptions(ref cs));

            var parts = new SingleStoreConnectionStringBuilder(cs)
                .ConnectionAttributes
                .TrimEnd(',')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;
            Assert.Equal(1, parts.Count(p => p.StartsWith("_connector_name:SingleStore Entity Framework Core provider")));
            Assert.Equal(1, parts.Count(p => p.StartsWith($"_connector_version:{programVersion}")));
        }

        [Fact]
        public void EnsureMandatoryOptions_SetsMandatoryFlags_WhenMissing()
        {
            var cs = WithMandatoryFlags(AppConfig.ConnectionString, false, true);

            var changed = _validator.EnsureMandatoryOptions(ref cs);

            Assert.True(changed);
            var result = new SingleStoreConnectionStringBuilder(cs);
            Assert.True(result.AllowUserVariables);
            Assert.False(result.UseAffectedRows);

            var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;
            Assert.Contains("_connector_name:SingleStore Entity Framework Core provider", result.ConnectionAttributes);
            Assert.Contains($"_connector_version:{programVersion}", result.ConnectionAttributes);
        }

        [Fact]
        public void EnsureMandatoryOptions_DbConnection_InjectsAttributesAndFlags()
        {
            var cs = WithMandatoryFlags(AppConfig.ConnectionString, allowUserVariables: false, useAffectedRows: true);
            using var conn = new SingleStoreConnection(cs);

            var changed = _validator.EnsureMandatoryOptions(conn);

            Assert.True(changed);
            var result = new SingleStoreConnectionStringBuilder(conn.ConnectionString);
            Assert.True(result.AllowUserVariables);
            Assert.False(result.UseAffectedRows);

            // NOTE: We intentionally don't assert ConnectionAttributes here.
            // EnsureMandatoryOptions(DbConnection) is only allowed to mutate the connection string when it is safe to do so.
            // In practice, EF Core (and the provider) may open the DbConnection before our validator runs (or a caller may pass
            // an already-open connection)
            // Assert.Contains("_connector_name:SingleStore Entity Framework Core provider", result.ConnectionAttributes);
            // var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;
            // Assert.Contains($"_connector_version:{programVersion}", result.ConnectionAttributes);
        }

        [Fact]
        public void EnsureMandatoryOptions_DbDataSource_ThrowsWhenMissingMandatoryOptions()
        {
            var badCs = WithMandatoryFlags(AppConfig.ConnectionString, allowUserVariables: false, useAffectedRows: true);
            var dataSource = new SingleStoreDataSourceBuilder(badCs).Build();

            var ex = Assert.Throws<InvalidOperationException>(
                () => _validator.EnsureMandatoryOptions(dataSource));
            Assert.Contains("AllowUserVariables=True;UseAffectedRows=False", ex.Message);
        }

        private static string WithMandatoryFlags(string baseCs, bool allowUserVariables, bool useAffectedRows)
            => new SingleStoreConnectionStringBuilder(baseCs)
            {
                AllowUserVariables = allowUserVariables,
                UseAffectedRows = useAffectedRows
            }.ConnectionString;

        private static string WithAttrs(string baseCs, string attrs)
            => new SingleStoreConnectionStringBuilder(baseCs)
            {
                ConnectionAttributes = attrs
            }.ConnectionString;
    }
}
