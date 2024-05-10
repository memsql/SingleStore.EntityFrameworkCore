using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FieldsOnlyLoadSingleStoreTest : FieldsOnlyLoadTestBase<FieldsOnlyLoadSingleStoreTest.FieldsOnlyLoadSingleStoreFixture>
    {
        public FieldsOnlyLoadSingleStoreTest(FieldsOnlyLoadSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class FieldsOnlyLoadSingleStoreFixture : FieldsOnlyLoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;
        }
    }
}
