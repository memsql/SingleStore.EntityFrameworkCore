using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPHInheritanceQuerySingleStoreTest : TPHInheritanceQueryTestBase<TPHInheritanceQuerySingleStoreFixture>
{
    public TPHInheritanceQuerySingleStoreTest(TPHInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    [ConditionalFact(Skip = "Feature 'FOREIGN KEY' is not supported by SingleStore Distributed.")]
    public override void Setting_foreign_key_to_a_different_type_throws()
    {
        base.Setting_foreign_key_to_a_different_type_throws();
    }
}
