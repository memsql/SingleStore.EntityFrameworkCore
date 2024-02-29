using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class ConcurrencyDetectorDisabledSingleStoreTest : ConcurrencyDetectorDisabledRelationalTestBase<
        ConcurrencyDetectorDisabledSingleStoreTest.ConcurrencyDetectorSingleStoreFixture>
    {
        public ConcurrencyDetectorDisabledSingleStoreTest(ConcurrencyDetectorSingleStoreFixture fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        protected override async Task ConcurrencyDetectorTest(Func<ConcurrencyDetectorDbContext, Task<object>> test)
        {
            await base.ConcurrencyDetectorTest(test);

            Assert.NotEmpty(Fixture.TestSqlLoggerFactory.SqlStatements);
        }

        public class ConcurrencyDetectorSingleStoreFixture : ConcurrencyDetectorFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => builder.EnableThreadSafetyChecks(false);
        }
    }
}
