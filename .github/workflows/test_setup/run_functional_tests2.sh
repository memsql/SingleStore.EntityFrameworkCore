cd test/EFCore.SingleStore.FunctionalTests/

dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FullInfrastructureMigrationsTest.'
((TOTAL_FAILURES = $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LoggingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LoadSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LazyLoadProxySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.KeysWithConvertersSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyLoadSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyTrackingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MaterializationInterceptionSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MigrationsSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MigrationsInfrastructureSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OptimisticConcurrencySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NotificationEntitiesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindQueryTaggingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MusicStoreSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.PropertyValuesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OverzealousInitializationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreDatabaseModelFactoryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreValueGenerationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreTypeMappingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoredProcedureUpdateSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonMicrosoftTypeMappingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonNewtonsoftTypeMappingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreUpdateSqlGeneratorTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~SaveChangesInterceptionSingleStoreTest'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SeedingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SerializationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreMigrationsSqlGeneratorTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreComplianceTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreApiConsistencyTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreGeneratedSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreGeneratedFixupSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SpatialSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreServiceCollectionExtensionsTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreNetTopologySuiteApiConsistencyTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TableSplittingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTTableSplittingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TransactionInterceptionSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TransactionSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TwoDatabasesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.UpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ValueConvertersEndToEndSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.WithConstructorsSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FiltersInheritanceBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NonSharedModelBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCFiltersInheritanceBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCInheritanceBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTFiltersInheritanceBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTInheritanceBulkUpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))

cd ../../

if [[ $TOTAL_FAILURES -ne 0 ]]; then
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
else
    exit 0
fi