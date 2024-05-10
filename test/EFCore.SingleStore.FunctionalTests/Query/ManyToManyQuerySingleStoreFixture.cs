using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class ManyToManyQuerySingleStoreFixture : ManyToManyQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;
    }
}
