using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class ConcurrencyDetectorEnabledSingleStoreTest : ConcurrencyDetectorEnabledRelationalTestBase<
        ConcurrencyDetectorEnabledSingleStoreTest.ConcurrencyDetectorSingleStoreFixture>
    {
        public ConcurrencyDetectorEnabledSingleStoreTest(ConcurrencyDetectorSingleStoreFixture fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        protected override async Task ConcurrencyDetectorTest(Func<ConcurrencyDetectorDbContext, Task<object>> test)
        {
            await base.ConcurrencyDetectorTest(test);

            Assert.Empty(Fixture.TestSqlLoggerFactory.SqlStatements);
        }

        public class ConcurrencyDetectorSingleStoreFixture : ConcurrencyDetectorFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;
        }
    }
}
