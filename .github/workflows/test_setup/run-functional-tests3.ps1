cd test\EFCore.SingleStore.FunctionalTests\

dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MigrationsSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MigrationsInfrastructureSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OptimisticConcurrencySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NotificationEntitiesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindQueryTaggingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MusicStoreSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.PropertyValuesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OverzealousInitializationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreDatabaseModelFactoryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreValueGenerationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreTypeMappingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoredProcedureUpdateSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonMicrosoftTypeMappingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonNewtonsoftTypeMappingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreUpdateSqlGeneratorTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~SaveChangesInterceptionSingleStoreTest'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SeedingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SerializationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreMigrationsSqlGeneratorTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreComplianceTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreApiConsistencyTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreGeneratedSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreGeneratedFixupSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SpatialSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreServiceCollectionExtensionsTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreNetTopologySuiteApiConsistencyTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TableSplittingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTTableSplittingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TransactionInterceptionSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TransactionSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TwoDatabasesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.UpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ValueConvertersEndToEndSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.WithConstructorsSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FiltersInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NonSharedModelBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCFiltersInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTFiltersInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)

cd ..\..\

if ( $TOTAL_FAILURES -ne 0 ) {
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
}
else {
    exit 0
}
