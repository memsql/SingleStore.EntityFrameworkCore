using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class F1SingleStoreFixture : F1SingleStoreFixture<byte[]>
    {
    }

    public abstract class F1SingleStoreFixture<TRowVersion> : F1RelationalFixture<TRowVersion>
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        public override TestHelpers TestHelpers
            => SingleStoreTestHelpers.Instance;
    }
}
