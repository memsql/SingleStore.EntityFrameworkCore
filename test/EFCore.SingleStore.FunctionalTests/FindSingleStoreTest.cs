using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public abstract class FindSingleStoreTest : FindTestBase<FindSingleStoreTest.FindSingleStoreFixture>
{
    protected FindSingleStoreTest(FindSingleStoreFixture fixture)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
    }

    public class FindSingleStoreTestSet : FindSingleStoreTest
    {
        public FindSingleStoreTestSet(FindSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override TestFinder Finder { get; } = new FindViaSetFinder();
    }

    public class FindSingleStoreTestContext : FindSingleStoreTest
    {
        public FindSingleStoreTestContext(FindSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override TestFinder Finder { get; } = new FindViaContextFinder();
    }

    public class FindSingleStoreTestNonGeneric : FindSingleStoreTest
    {
        public FindSingleStoreTestNonGeneric(FindSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override TestFinder Finder { get; } = new FindViaNonGenericContextFinder();
    }

    public class FindSingleStoreFixture : FindFixtureBase
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
    }
}
