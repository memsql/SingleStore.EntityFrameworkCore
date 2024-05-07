using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class OverzealousInitializationSingleStoreTest
        : OverzealousInitializationTestBase<OverzealousInitializationSingleStoreTest.OverzealousInitializationSingleStoreFixture>
    {
        public OverzealousInitializationSingleStoreTest(OverzealousInitializationSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class OverzealousInitializationSingleStoreFixture : OverzealousInitializationFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
        }
    }
}
