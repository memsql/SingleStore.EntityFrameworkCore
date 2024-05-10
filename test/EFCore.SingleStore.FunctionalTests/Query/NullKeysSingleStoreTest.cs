using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NullKeysSingleStoreTest : NullKeysTestBase<NullKeysSingleStoreTest.NullKeysSingleStoreFixture>
    {
        public NullKeysSingleStoreTest(NullKeysSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class NullKeysSingleStoreFixture : NullKeysFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
        }
    }
}
