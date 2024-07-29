using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class EntitySplittingSingleStoreTest : EntitySplittingTestBase
{
    public EntitySplittingSingleStoreTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
