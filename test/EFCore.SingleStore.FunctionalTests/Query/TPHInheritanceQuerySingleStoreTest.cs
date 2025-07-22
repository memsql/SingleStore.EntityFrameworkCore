using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPHInheritanceQuerySingleStoreTest : TPHInheritanceQueryTestBase<TPHInheritanceQuerySingleStoreFixture>
{
    public TPHInheritanceQuerySingleStoreTest(TPHInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }
}
