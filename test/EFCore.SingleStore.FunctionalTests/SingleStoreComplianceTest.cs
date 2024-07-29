using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreComplianceTest : RelationalComplianceTestBase
    {
        // TODO: Implement remaining 3.x tests.
        protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>
        {
            typeof(UdfDbFunctionTestBase<>),
            typeof(TransactionInterceptionTestBase),
            typeof(CommandInterceptionTestBase),
            typeof(NorthwindQueryTaggingQueryTestBase<>),

            // TODO: Reenable LoggingSingleStoreTest once its issue has been fixed in EF Core upstream.
            typeof(LoggingTestBase),
            typeof(LoggingRelationalTestBase<,>),

            // We have our own JSON support for now
            typeof(JsonUpdateTestBase<>),
            typeof(JsonQueryTestBase<>),
            typeof(JsonQueryAdHocTestBase),

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
