using EntityFrameworkCore.SingleStore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.SingleStore.Diagnostics.Internal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public class SingleStoreDatabaseCleaner : RelationalDatabaseCleaner
    {
        private readonly ISingleStoreOptions _options;
        private readonly IRelationalTypeMappingSource _relationalTypeMappingSource;

        public SingleStoreDatabaseCleaner(ISingleStoreOptions options, IRelationalTypeMappingSource relationalTypeMappingSource)
        {
            _options = options;
            _relationalTypeMappingSource = relationalTypeMappingSource;
        }

        protected override IDatabaseModelFactory CreateDatabaseModelFactory(ILoggerFactory loggerFactory)
            => new SingleStoreDatabaseModelFactory(
                new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                    loggerFactory,
                    new LoggingOptions(),
                    new DiagnosticListener("Fake"),
                    new SingleStoreLoggingDefinitions(),
                    new NullDbContextLogger()),
                _relationalTypeMappingSource,
                _options);

        protected override bool AcceptIndex(DatabaseIndex index) => false;
        protected override bool AcceptTable(DatabaseTable table) => !(table is DatabaseView);

        protected override string BuildCustomSql(DatabaseModel databaseModel)
            => @"SELECT 0";
    }
}
