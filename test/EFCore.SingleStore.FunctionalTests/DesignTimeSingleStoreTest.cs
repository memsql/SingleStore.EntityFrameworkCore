using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Design.Internal;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class DesignTimeSingleStoreTest : DesignTimeTestBase<DesignTimeSingleStoreTest.DesignTimeSingleStoreFixture>
    {
        public DesignTimeSingleStoreTest(DesignTimeSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override Assembly ProviderAssembly
            => typeof(SingleStoreDesignTimeServices).Assembly;

        public class DesignTimeSingleStoreFixture : DesignTimeFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;
        }
    }
}
