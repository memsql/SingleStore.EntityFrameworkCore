using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class KeysWithConvertersSingleStoreTest : KeysWithConvertersTestBase<
        KeysWithConvertersSingleStoreTest.KeysWithConvertersSingleStoreFixture>
    {
        public KeysWithConvertersSingleStoreTest(KeysWithConvertersSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class KeysWithConvertersSingleStoreFixture : KeysWithConvertersFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => builder.UseSingleStore(b => b.MinBatchSize(1));
        }
    }
}
