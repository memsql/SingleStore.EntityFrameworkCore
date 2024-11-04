cd test\EFCore.SingleStore.FunctionalTests\

dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindJoinQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindIncludeNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindQueryFiltersQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindMiscellaneousQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSelectQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSetOperationsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindStringIncludeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindWhereQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NullKeysSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NullSemanticsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedEntityQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryFilterFuncletizationSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindNavigationsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SharedTypeQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryNoClientEvalSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SimpleQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SqlExecutorSingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ToSqlQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCGearsOfWarQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCFiltersInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCInheritanceQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCManyToManyNoTrackingQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCManyToManyQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)
dotnet.exe test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPCRelationshipsQuerySingleStoreTest.'
$TOTAL_FAILURES += ($LASTEXITCODE -ne 0)

cd ..\..\

if ( $TOTAL_FAILURES -ne 0 ) {
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
}
else {
    exit 0
}
