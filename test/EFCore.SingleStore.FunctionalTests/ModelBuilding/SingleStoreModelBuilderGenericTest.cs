using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.ModelBuilding;

public class SingleStoreModelBuilderGenericTest : SingleStoreModelBuilderTestBase
{
    public class SingleStoreGenericNonRelationship(SingleStoreModelBuilderFixture fixture) : SingleStoreNonRelationship(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class SingleStoreGenericComplexType(SingleStoreModelBuilderFixture fixture) : SingleStoreComplexType(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class SingleStoreGenericInheritance(SingleStoreModelBuilderFixture fixture) : SingleStoreInheritance(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class SingleStoreGenericOneToMany(SingleStoreModelBuilderFixture fixture) : SingleStoreOneToMany(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class SingleStoreGenericManyToOne(SingleStoreModelBuilderFixture fixture) : SingleStoreManyToOne(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class SingleStoreGenericOneToOne(SingleStoreModelBuilderFixture fixture) : SingleStoreOneToOne(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class SingleStoreGenericManyToMany(SingleStoreModelBuilderFixture fixture) : SingleStoreManyToMany(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    internal class SingleStoreGenericOwnedTypes(SingleStoreModelBuilderFixture fixture) : SingleStoreOwnedTypes(fixture)
    {
        // MySQL stored procedures do not support result columns.
        public override void Can_use_sproc_mapping_with_owned_reference()
            => Assert.Throws<InvalidOperationException>(() => base.Can_use_sproc_mapping_with_owned_reference());

        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }
}
