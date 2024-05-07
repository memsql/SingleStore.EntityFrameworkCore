using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreNetTopologySuiteApiConsistencyTest : ApiConsistencyTestBase<SingleStoreNetTopologySuiteApiConsistencyTest.SingleStoreNetTopologySuiteApiConsistencyFixture>
    {
        public SingleStoreNetTopologySuiteApiConsistencyTest(SingleStoreNetTopologySuiteApiConsistencyFixture fixture)
            : base(fixture)
        {
        }

        protected override void AddServices(ServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkSingleStoreNetTopologySuite();

        protected override Assembly TargetAssembly
            => typeof(SingleStoreNetTopologySuiteServiceCollectionExtensions).Assembly;

        public class SingleStoreNetTopologySuiteApiConsistencyFixture : ApiConsistencyFixtureBase
        {
            public override HashSet<Type> FluentApiTypes { get; } = new()
            {
                typeof(SingleStoreNetTopologySuiteDbContextOptionsBuilderExtensions),
                typeof(SingleStoreNetTopologySuiteServiceCollectionExtensions)
            };
        }
    }
}
