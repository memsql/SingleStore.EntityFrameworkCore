using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.ModelBuilding;

public class SingleStoreModelBuilderTestBase : RelationalModelBuilderTest
{
    public abstract class SingleStoreNonRelationship(SingleStoreModelBuilderFixture fixture)
        : RelationalNonRelationshipTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreComplexType(SingleStoreModelBuilderFixture fixture)
        : RelationalComplexTypeTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreInheritance(SingleStoreModelBuilderFixture fixture)
        : RelationalInheritanceTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreOneToMany(SingleStoreModelBuilderFixture fixture)
        : RelationalOneToManyTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreManyToOne(SingleStoreModelBuilderFixture fixture)
        : RelationalManyToOneTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreOneToOne(SingleStoreModelBuilderFixture fixture)
        : RelationalOneToOneTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreManyToMany(SingleStoreModelBuilderFixture fixture)
        : RelationalManyToManyTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public abstract class SingleStoreOwnedTypes(SingleStoreModelBuilderFixture fixture)
        : RelationalOwnedTypesTestBase(fixture), IClassFixture<SingleStoreModelBuilderFixture>;

    public class SingleStoreModelBuilderFixture : RelationalModelBuilderFixture
    {
        public override TestHelpers TestHelpers
            => SingleStoreTestHelpers.Instance;
    }
}
