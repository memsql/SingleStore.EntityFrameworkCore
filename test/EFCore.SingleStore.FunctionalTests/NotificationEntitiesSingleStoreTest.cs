using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class NotificationEntitiesSingleStoreTest : NotificationEntitiesTestBase<NotificationEntitiesSingleStoreTest.NotificationEntitiesSingleStoreFixture>
    {
        public NotificationEntitiesSingleStoreTest(NotificationEntitiesSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class NotificationEntitiesSingleStoreFixture : NotificationEntitiesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
        }
    }
}
