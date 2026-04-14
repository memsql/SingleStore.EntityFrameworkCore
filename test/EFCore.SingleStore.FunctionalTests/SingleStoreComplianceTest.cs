using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreComplianceTest : RelationalComplianceTestBase
    {
        // TODO: Implement remaining 3.x tests.
        protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>
        {
            // There are two classes that can lead to a MySqlEndOfStreamException, if *both* test classes are included in the run:
            //     - RelationalModelBuilderTest.RelationalComplexTypeTestBase
            //     - RelationalModelBuilderTest.RelationalOwnedTypesTestBase
            //
            // The exception is thrown for MySQL most of the time, though in rare cases also for MariaDB.
            // We disable `RelationalModelBuilderTest.RelationalOwnedTypesTestBase` for now.

            // typeof(RelationalModelBuilderTest.RelationalNonRelationshipTestBase),
            // typeof(RelationalModelBuilderTest.RelationalComplexTypeTestBase),
            // typeof(RelationalModelBuilderTest.RelationalInheritanceTestBase),
            // typeof(RelationalModelBuilderTest.RelationalOneToManyTestBase),
            // typeof(RelationalModelBuilderTest.RelationalManyToOneTestBase),
            // typeof(RelationalModelBuilderTest.RelationalOneToOneTestBase),
            // typeof(RelationalModelBuilderTest.RelationalManyToManyTestBase),
            typeof(RelationalModelBuilderTest.RelationalOwnedTypesTestBase),
            typeof(ModelBuilderTest.OwnedTypesTestBase), // base class of RelationalModelBuilderTest.RelationalOwnedTypesTestBase

            typeof(UdfDbFunctionTestBase<>),
            typeof(TransactionInterceptionTestBase),
            typeof(CommandInterceptionTestBase),
            typeof(NorthwindQueryTaggingQueryTestBase<>),
            typeof(NonSharedModelUpdatesTestBase),
            typeof(FindSingleStoreTest),

            // TODO: 9.0
            typeof(AdHocComplexTypeQueryTestBase),
            typeof(AdHocPrecompiledQueryRelationalTestBase),
            typeof(PrecompiledQueryRelationalTestBase),
            typeof(PrecompiledSqlPregenerationQueryRelationalTestBase),

            // TODO: Reenable LoggingSingleStoreTest once its issue has been fixed in EF Core upstream.
            typeof(LoggingTestBase),
            typeof(LoggingRelationalTestBase<,>),

            // We have our own JSON support for now
            typeof(AdHocJsonQueryTestBase),
            typeof(JsonQueryRelationalTestBase<>),
            typeof(JsonQueryTestBase<>),
            typeof(JsonTypesRelationalTestBase),
            typeof(JsonTypesTestBase),
            typeof(JsonUpdateTestBase<>),
            typeof(OptionalDependentQueryTestBase<>),

            typeof(FieldMappingTestBase<>),
            typeof(GraphUpdatesTestBase<>),
            typeof(LazyLoadProxyTestBase<>),
            typeof(LoadTestBase<>),
            typeof(ManyToManyLoadTestBase<>),
            typeof(ManyToManyTrackingTestBase<>),
            typeof(MonsterFixupTestBase<>),
            typeof(ProxyGraphUpdatesTestBase<>),
            typeof(SpatialTestBase<>),
            typeof(StoreGeneratedFixupTestBase<>),
            typeof(StoreGeneratedFixupRelationalTestBase<>),
            typeof(ManyToManyFieldsLoadTestBase<>),
            typeof(ManyToManyNoTrackingQueryTestBase<>),
            typeof(ManyToManyQueryTestBase<>),
            typeof(ManyToManyTrackingRelationalTestBase<>),
            typeof(SpatialQueryTestBase<>),
            typeof(ManyToManyNoTrackingQueryRelationalTestBase<>),
            typeof(ManyToManyQueryRelationalTestBase<>),
            typeof(SpatialQueryRelationalTestBase<>),
            typeof(TPTManyToManyNoTrackingQueryRelationalTestBase<>),
            typeof(TPTManyToManyQueryRelationalTestBase<>),
            typeof(WithConstructorsTestBase<>),
            typeof(StoreGeneratedTestBase<>),
            typeof(IncludeOneToOneTestBase<>)
        };

        protected override Assembly TargetAssembly { get; } = typeof(SingleStoreComplianceTest).Assembly;
    }
}
