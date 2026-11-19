using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Associations;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting;
using Microsoft.EntityFrameworkCore.Query.Associations.Navigations;
using Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson;
using Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations;
using Microsoft.EntityFrameworkCore.Query.Associations.OwnedTableSplitting;
using Microsoft.EntityFrameworkCore.Types;
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
            typeof(AdHocJsonQueryRelationalTestBase), // relational variant of AdHocJsonQueryTestBase
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
            typeof(IncludeOneToOneTestBase<>),

            // TODO: PLAT-6409 (requires JSON column support): these test bases need `ToJson()` or complex collections, which EF Core
            // can only map to JSON columns.
            typeof(OwnedJsonBulkUpdateRelationalTestBase<>),
            typeof(OwnedJsonCollectionRelationalTestBase<>),
            typeof(OwnedJsonMiscellaneousRelationalTestBase<>),
            typeof(OwnedJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(OwnedJsonProjectionRelationalTestBase<>),
            typeof(OwnedJsonStructuralEqualityRelationalTestBase<>),
            typeof(ComplexJsonBulkUpdateRelationalTestBase<>),
            typeof(ComplexJsonCollectionRelationalTestBase<>),
            typeof(ComplexJsonMiscellaneousRelationalTestBase<>),
            typeof(ComplexJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(ComplexJsonProjectionRelationalTestBase<>),
            typeof(ComplexJsonSetOperationsRelationalTestBase<>),
            typeof(ComplexJsonStructuralEqualityRelationalTestBase<>),
            typeof(ComplexPropertiesCollectionTestBase<>), // only implemented by the ComplexJson tests
            typeof(ComplexPropertiesSetOperationsTestBase<>), // only implemented by the ComplexJson tests
            typeof(ComplexCollectionJsonUpdateTestBase<>),
            typeof(ModelBuilderTest.ComplexCollectionTestBase),
            typeof(RelationalModelBuilderTest.RelationalComplexCollectionTestBase),

            // Association tests that rely on relationships between tables, which SingleStore doesn't enforce (like the
            // GraphUpdates and ManyToMany tests above).
            typeof(AssociationsBulkUpdateTestBase<>),
            typeof(AssociationsCollectionTestBase<>),
            typeof(AssociationsMiscellaneousTestBase<>),
            typeof(AssociationsPrimitiveCollectionTestBase<>),
            typeof(AssociationsProjectionTestBase<>),
            typeof(AssociationsSetOperationsTestBase<>),
            typeof(AssociationsStructuralEqualityTestBase<>),
            typeof(NavigationsCollectionTestBase<>),
            typeof(NavigationsIncludeTestBase<>),
            typeof(NavigationsMiscellaneousTestBase<>),
            typeof(NavigationsPrimitiveCollectionTestBase<>),
            typeof(NavigationsProjectionTestBase<>),
            typeof(NavigationsSetOperationsTestBase<>),
            typeof(NavigationsStructuralEqualityTestBase<>),
            typeof(NavigationsCollectionRelationalTestBase<>),
            typeof(NavigationsIncludeRelationalTestBase<>),
            typeof(NavigationsMiscellaneousRelationalTestBase<>),
            typeof(NavigationsPrimitiveCollectionRelationalTestBase<>),
            typeof(NavigationsProjectionRelationalTestBase<>),
            typeof(NavigationsSetOperationsRelationalTestBase<>),
            typeof(NavigationsStructuralEqualityRelationalTestBase<>),
            typeof(OwnedNavigationsCollectionTestBase<>),
            typeof(OwnedNavigationsMiscellaneousTestBase<>),
            typeof(OwnedNavigationsPrimitiveCollectionTestBase<>),
            typeof(OwnedNavigationsProjectionTestBase<>),
            typeof(OwnedNavigationsSetOperationsTestBase<>),
            typeof(OwnedNavigationsStructuralEqualityTestBase<>),
            typeof(OwnedNavigationsCollectionRelationalTestBase<>),
            typeof(OwnedNavigationsMiscellaneousRelationalTestBase<>),
            typeof(OwnedNavigationsPrimitiveCollectionRelationalTestBase<>),
            typeof(OwnedNavigationsProjectionRelationalTestBase<>),
            typeof(OwnedNavigationsSetOperationsRelationalTestBase<>),
            typeof(OwnedNavigationsStructuralEqualityRelationalTestBase<>),
            typeof(OwnedTableSplittingMiscellaneousRelationalTestBase<>),
            typeof(OwnedTableSplittingPrimitiveCollectionRelationalTestBase<>),
            typeof(OwnedTableSplittingProjectionRelationalTestBase<>),
            typeof(OwnedTableSplittingStructuralEqualityRelationalTestBase<>),

            // TODO: PLAT-7535 (primitive collections support): complex table splitting association tests. They don't need
            // relationships, but the shared model has `List<int>` properties, which need primitive collections support.
            typeof(ComplexPropertiesBulkUpdateTestBase<>),
            typeof(ComplexPropertiesMiscellaneousTestBase<>),
            typeof(ComplexPropertiesPrimitiveCollectionTestBase<>),
            typeof(ComplexPropertiesProjectionTestBase<>),
            typeof(ComplexPropertiesStructuralEqualityTestBase<>),
            typeof(ComplexTableSplittingBulkUpdateRelationalTestBase<>),
            typeof(ComplexTableSplittingMiscellaneousRelationalTestBase<>),
            typeof(ComplexTableSplittingPrimitiveCollectionRelationalTestBase<>),
            typeof(ComplexTableSplittingProjectionRelationalTestBase<>),
            typeof(ComplexTableSplittingStructuralEqualityRelationalTestBase<>),

            // TODO: 10.0 - Type tests
            typeof(TypeTestBase<,>),
            typeof(RelationalTypeTestBase<,>),
        };

        protected override Assembly TargetAssembly { get; } = typeof(SingleStoreComplianceTest).Assembly;
    }
}
