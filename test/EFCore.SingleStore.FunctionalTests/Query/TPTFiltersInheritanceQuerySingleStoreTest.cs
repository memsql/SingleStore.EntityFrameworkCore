using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQuerySingleStoreTest : TPTFiltersInheritanceQueryTestBase<TPTFiltersInheritanceQuerySingleStoreFixture>
    {
        public TPTFiltersInheritanceQuerySingleStoreTest(TPTFiltersInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
