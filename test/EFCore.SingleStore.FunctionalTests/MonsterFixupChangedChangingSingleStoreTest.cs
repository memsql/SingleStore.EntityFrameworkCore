using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests.
    internal class MonsterFixupChangedChangingSingleStoreTest : MonsterFixupTestBase<MonsterFixupChangedChangingSingleStoreTest.MonsterFixupChangedChangingSingleStoreFixture>
    {
        public MonsterFixupChangedChangingSingleStoreTest(MonsterFixupChangedChangingSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class MonsterFixupChangedChangingSingleStoreFixture : MonsterFixupChangedChangingFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(
                ModelBuilder builder)
            {
                base.OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(builder);

                builder.Entity<TMessage>().HasKey(e => e.MessageId);
                builder.Entity<TProductPhoto>().HasKey(e => e.PhotoId);
                builder.Entity<TProductReview>().HasKey(e => e.ReviewId);
            }
        }
    }
}
