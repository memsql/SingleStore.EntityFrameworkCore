using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NullSemanticsQuerySingleStoreFixture : NullSemanticsQueryFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
    }
}
