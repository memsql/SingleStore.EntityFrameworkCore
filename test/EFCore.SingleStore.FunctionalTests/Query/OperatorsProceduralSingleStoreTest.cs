using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class OperatorsProceduralSingleStoreTest : OperatorsProceduralQueryTestBase
{
    public OperatorsProceduralSingleStoreTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
