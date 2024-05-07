using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SingleStoreServiceCollectionExtensionsTest : RelationalServiceCollectionExtensionsTestBase
    {
        public SingleStoreServiceCollectionExtensionsTest()
            : base(SingleStoreTestHelpers.Instance)
        {
        }
    }
}
