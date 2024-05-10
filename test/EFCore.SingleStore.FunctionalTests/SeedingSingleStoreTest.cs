using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SeedingSingleStoreTest : SeedingTestBase
    {
        protected override TestStore TestStore => SingleStoreTestStore.Create("SeedingTest");

        protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
            => new SeedingSingleStoreContext(testId);

        protected class SeedingSingleStoreContext : SeedingContext
        {
            public SeedingSingleStoreContext(string testId)
                : base(testId)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSingleStore(SingleStoreTestStore.CreateConnectionString($"Seeds{TestId}"));
        }
    }
}
