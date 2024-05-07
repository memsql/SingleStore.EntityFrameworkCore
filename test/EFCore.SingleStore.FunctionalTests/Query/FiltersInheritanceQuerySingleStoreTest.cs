using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FiltersInheritanceQuerySingleStoreTest : FiltersInheritanceQueryTestBase<FiltersInheritanceQuerySingleStoreFixture>
    {
        public FiltersInheritanceQuerySingleStoreTest(FiltersInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
