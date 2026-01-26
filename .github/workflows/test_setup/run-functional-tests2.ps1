cd test\EFCore.SingleStore.FunctionalTests\

dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSplitSharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsSharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftPocoQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftStringChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftStringQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftDomChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftDomQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftPocoChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftPocoQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftStringChangeTrackingTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftStringQueryTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyHeterogeneousQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.MappingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.MatchQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAsTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAsNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAggregateQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAggregateOperatorsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindDbFunctionsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindCompiledQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindChangeTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindGroupByQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindFunctionsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindEFPropertyIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindKeylessEntitiesQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindJoinQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindIncludeNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindQueryFiltersQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindMiscellaneousQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSelectQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSetOperationsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindStringIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindWhereQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NullKeysSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NullSemanticsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedEntityQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryFilterFuncletizationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindNavigationsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.SharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryNoClientEvalSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.SimpleQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.SqlExecutorSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ToSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCGearsOfWarQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCFiltersInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCManyToManyNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCManyToManyQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCRelationshipsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCFiltersInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTFiltersInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTInheritanceBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexTypeBulkUpdatesSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexTypesTrackingSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.BadDataJsonDeserializationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.AdHocComplexTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.AdHocAdvancedMappingsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.OperatorsProceduralSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.OperatorsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.OptionalDependentQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.SqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.TPHInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net8.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreConnectionStringOptionsValidatorTests.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)

cd ..\..\

if ( $TOTAL_FAILURES -ne 0 ) {
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
}
else {
    exit 0
}

