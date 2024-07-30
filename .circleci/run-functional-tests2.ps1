cd test\EFCore.SingleStore.FunctionalTests\

dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAsNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAggregateQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAggregateOperatorsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindStringIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindWhereQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FromSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FiltersInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeysSplitQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeysQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSplitSharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSplitQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FromSqlSprocQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FunkyDataQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceRelationshipsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryNoClientEvalSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTFiltersInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NullKeysSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.BoolIndexingOptimizationDisabledSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsSharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DateOnlyQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.EscapesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FieldsOnlyLoadSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GearsOfWarFromSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GearsOfWarQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.IncludeOneToOneSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftDomChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftDomQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftPocoChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftStringChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftPocoQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftStringQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftDomChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftDomQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftPocoChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftPocoQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftStringChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftStringQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyHeterogeneousQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MappingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NullSemanticsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedEntityQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryFilterFuncletizationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SimpleQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SqlExecutorSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTGearsOfWarQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTRelationshipsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.WarningsSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreDatabaseModelFactoryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonMicrosoftTypeMappingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonNewtonsoftTypeMappingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreTypeMappingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreUpdateSqlGeneratorTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)

cd ..\..\

if ( $TOTAL_FAILURES -ne 0 ) {
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
}
else {
    exit 0
}
