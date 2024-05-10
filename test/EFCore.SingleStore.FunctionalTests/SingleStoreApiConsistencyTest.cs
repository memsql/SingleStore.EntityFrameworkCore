using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreApiConsistencyTest : ApiConsistencyTestBase<SingleStoreApiConsistencyTest.SingleStoreApiConsistencyFixture>
    {
        public SingleStoreApiConsistencyTest(SingleStoreApiConsistencyFixture fixture)
            : base(fixture)
        {
        }

        protected override void AddServices(ServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkSingleStore();

        protected override Assembly TargetAssembly => typeof(SingleStoreRelationalConnection).Assembly;

        public class SingleStoreApiConsistencyFixture : ApiConsistencyFixtureBase
        {
            public override HashSet<Type> FluentApiTypes { get; } = new()
            {
                typeof(SingleStoreDbContextOptionsBuilder),
                typeof(SingleStoreDbContextOptionsBuilderExtensions),
                typeof(SingleStoreMigrationBuilderExtensions),
                typeof(SingleStoreIndexBuilderExtensions),
                typeof(SingleStoreModelBuilderExtensions),
                typeof(SingleStorePropertyBuilderExtensions),
                typeof(SingleStoreEntityTypeBuilderExtensions),
                typeof(SingleStoreServiceCollectionExtensions)
            };

            public override
                List<(Type Type,
                    Type ReadonlyExtensions,
                    Type MutableExtensions,
                    Type ConventionExtensions,
                    Type ConventionBuilderExtensions,
                    Type RuntimeExtensions)> MetadataExtensionTypes { get; }
                = new()
                {
                    (
                        typeof(IReadOnlyModel),
                        typeof(SingleStoreModelExtensions),
                        typeof(SingleStoreModelExtensions),
                        typeof(SingleStoreModelExtensions),
                        typeof(SingleStoreModelBuilderExtensions),
                        null
                    ),
                    (
                        typeof(IReadOnlyEntityType),
                        typeof(SingleStoreEntityTypeExtensions),
                        typeof(SingleStoreEntityTypeExtensions),
                        typeof(SingleStoreEntityTypeExtensions),
                        typeof(SingleStoreEntityTypeBuilderExtensions),
                        null
                    ),
                    (
                        typeof(IReadOnlyProperty),
                        typeof(SingleStorePropertyExtensions),
                        typeof(SingleStorePropertyExtensions),
                        typeof(SingleStorePropertyExtensions),
                        typeof(SingleStorePropertyBuilderExtensions),
                        null
                    ),
                    (
                        typeof(IReadOnlyIndex),
                        typeof(SingleStoreIndexExtensions),
                        typeof(SingleStoreIndexExtensions),
                        typeof(SingleStoreIndexExtensions),
                        typeof(SingleStoreIndexBuilderExtensions),
                        null
                    )
                };

            public override HashSet<MethodInfo> UnmatchedMetadataMethods { get; } = new()
            {
                typeof(SingleStoreModelBuilderExtensions).GetMethod(
                    nameof(SingleStoreModelBuilderExtensions.UseCollation),
                    new[] {typeof(IConventionModelBuilder), typeof(string), typeof(DelegationModes?), typeof(bool)}),
                typeof(SingleStoreModelBuilderExtensions).GetMethod(
                    nameof(SingleStoreModelBuilderExtensions.UseCollation),
                    new[] {typeof(IConventionModelBuilder), typeof(string), typeof(bool?), typeof(bool)}),
            };
        }
    }
}
