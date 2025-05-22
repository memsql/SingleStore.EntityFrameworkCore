cd test/EFCore.SingleStore.FunctionalTests/

dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FullInfrastructureMigrationsTest.'
((TOTAL_FAILURES = $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TwoDatabasesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.UpdatesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))

cd ../../

if [[ $TOTAL_FAILURES -ne 0 ]]; then
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
else
    exit 0
fi
