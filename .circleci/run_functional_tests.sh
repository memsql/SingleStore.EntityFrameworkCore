cd test/EFCore.SingleStore.FunctionalTests/

dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.BuiltInDataTypesSingleStoreTest.'
((TOTAL_FAILURES = $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeyEndToEndSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConcurrencyDetectorDisabledSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConcurrencyDetectorEnabledSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConferencePlannerSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConnectionInterceptionSingleStoreTestBase.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConnectionSettingsSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConnectionSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ConvertToProviderTypesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CustomConvertersSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DataAnnotationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DatabindingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DesignTimeSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FieldMappingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FindSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FullInfrastructureMigrationsTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.KeysWithConvertersSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LazyLoadProxySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.LoadSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MigrationsInfrastructureSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MigrationsSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MusicStoreSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindQueryTaggingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NotificationEntitiesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OptimisticConcurrencySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OverzealousInitializationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.PropertyValuesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SaveChangesInterceptionSingleStoreTestBase.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SeedingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SerializationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreApiConsistencyTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreComplianceTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreMigrationsSqlGeneratorTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreNetTopologySuiteApiConsistencyTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreServiceCollectionExtensionsTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.StoreGeneratedSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TableSplittingSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTTableSplittingSingleStoreTest.'
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
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindNavigationsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeNoTrackingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSetOperationsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSelectQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindQueryFiltersQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindMiscellaneousQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindKeylessEntitiesQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindJoinQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindIncludeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindIncludeNoTrackingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindGroupByQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindFunctionsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindDbFunctionsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindCompiledQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindChangeTrackingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAsTrackingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAsNoTrackingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAggregateQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindAggregateOperatorsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindSplitIncludeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindStringIncludeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NorthwindWhereQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FromSqlQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FiltersInheritanceQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeysSplitQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.CompositeKeysQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSplitSharedTypeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSplitQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsSharedTypeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsCollectionsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FromSqlSprocQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FunkyDataQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceRelationshipsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryNoClientEvalSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTFiltersInheritanceQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NullKeysSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.BoolIndexingOptimizationDisabledSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ComplexNavigationsSharedTypeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.DateOnlyQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.EscapesSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.FieldsOnlyLoadSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GearsOfWarFromSqlQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.GearsOfWarQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.IncludeOneToOneSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.InheritanceQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftDomChangeTrackingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftDomQueryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftPocoChangeTrackingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftStringChangeTrackingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftPocoQueryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonMicrosoftStringQueryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftDomChangeTrackingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftDomQueryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftPocoChangeTrackingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftPocoQueryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftStringChangeTrackingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.JsonNewtonsoftStringQueryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.ManyToManyHeterogeneousQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.MappingQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.NullSemanticsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedEntityQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.OwnedQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.QueryFilterFuncletizationSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SharedTypeQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SimpleQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SqlExecutorSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTGearsOfWarQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTInheritanceQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.TPTRelationshipsQuerySingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.WarningsSingleStoreTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreDatabaseModelFactoryTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonMicrosoftTypeMappingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreJsonNewtonsoftTypeMappingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreTypeMappingTest.'
((TOTAL_FAILURES += $? != 0))
dotnet test -f net7.0 -c Release --no-build --filter 'FullyQualifiedName~.SingleStoreUpdateSqlGeneratorTest.'
((TOTAL_FAILURES += $? != 0))

cd ../../

if [[ $TOTAL_FAILURES -ne 0 ]]; then
    echo "Number of tests failed: ${TOTAL_FAILURES}"
    exit 1
else
    exit 0
fi
