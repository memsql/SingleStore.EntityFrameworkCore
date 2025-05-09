cd test\EFCore.SingleStore.FunctionalTests\

dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsQuerySingleStoreTest.'
$TOTAL_FAILURES = ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.Ef6GroupBySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeysQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeysSplitQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DateOnlyQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.EscapesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.EntitySplittingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FromSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FieldsOnlyLoadSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FiltersInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FromSqlSprocQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FunkyDataQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GearsOfWarFromSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GearsOfWarQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.IncludeOneToOneSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftPocoChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.BoolIndexingOptimizationDisabledSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftDomChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftDomQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceRelationshipsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSplitQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTFiltersInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES = ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTGearsOfWarQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTRelationshipsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.WarningsSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConcurrencyDetectorDisabledSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeyEndToEndSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CommandInterceptionSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.BuiltInDataTypesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConnectionSettingsSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~ConnectionInterceptionSingleStoreTest'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConferencePlannerSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConcurrencyDetectorEnabledSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DataAnnotationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CustomConvertersSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConvertToProviderTypesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConnectionSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.EntitySplittingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NonSharedModelUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DesignTimeSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DefaultValuesTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DatabindingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~FindSingleStoreTest'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FieldMappingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ExistingConnectionSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GraphUpdatesSingleStoreTestBase.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GraphUpdatesSingleStoreClientNoActionTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GraphUpdatesSingleStoreClientCascadeTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FullInfrastructureMigrationsTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LoggingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LoadSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LazyLoadProxySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.KeysWithConvertersSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyLoadSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyTrackingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MaterializationInterceptionSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)


cd ..\..\

if ( $TOTAL_FAILURES -ne 0 ) {
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
}
else {
    exit 0
}
