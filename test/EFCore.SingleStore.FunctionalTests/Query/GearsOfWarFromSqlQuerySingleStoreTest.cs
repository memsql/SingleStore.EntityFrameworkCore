using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class GearsOfWarFromSqlQuerySingleStoreTest : GearsOfWarFromSqlQueryTestBase<GearsOfWarQuerySingleStoreFixture>
    {
        public GearsOfWarFromSqlQuerySingleStoreTest(GearsOfWarQuerySingleStoreFixture fixture)
            : base(fixture)
        {
        }
    }
}
