using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests
    internal class StoreGeneratedFixupSingleStoreTest : StoreGeneratedFixupRelationalTestBase<StoreGeneratedFixupSingleStoreTest.StoreGeneratedFixupSingleStoreFixture>
    {
        public StoreGeneratedFixupSingleStoreTest(StoreGeneratedFixupSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public void Temp_values_can_be_made_permanent()
        {
            using (var context = CreateContext())
            {
                var entry = context.Add(new TestTemp());

                Assert.True(entry.Property(e => e.Id).IsTemporary);
                Assert.False(entry.Property(e => e.NotId).IsTemporary);

                var tempValue = entry.Property(e => e.Id).CurrentValue;

                entry.Property(e => e.Id).IsTemporary = false;

                context.SaveChanges();

                Assert.False(entry.Property(e => e.Id).IsTemporary);
                Assert.Equal(tempValue, entry.Property(e => e.Id).CurrentValue);
            }
        }

        protected override bool EnforcesFKs => true;

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class StoreGeneratedFixupSingleStoreFixture : StoreGeneratedFixupRelationalFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
        }
    }
}
