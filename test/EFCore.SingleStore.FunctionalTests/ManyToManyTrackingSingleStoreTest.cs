namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests.
    internal class ManyToManyTrackingSingleStoreTest
        : ManyToManyTrackingSingleStoreTestBase<ManyToManyTrackingSingleStoreTest.ManyToManyTrackingSingleStoreFixture>
    {
        public ManyToManyTrackingSingleStoreTest(ManyToManyTrackingSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class ManyToManyTrackingSingleStoreFixture : ManyToManyTrackingSingleStoreFixtureBase
        {
            protected override string StoreName
                => "ManyToManyTrackingSingleStoreTest";
        }
    }
}
