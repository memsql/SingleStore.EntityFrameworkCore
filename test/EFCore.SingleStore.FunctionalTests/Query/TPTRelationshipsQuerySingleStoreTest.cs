using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTRelationshipsQuerySingleStoreTest
        : TPTRelationshipsQueryTestBase<TPTRelationshipsQuerySingleStoreTest.TPTRelationshipsQuerySingleStoreFixture>
    {
        public TPTRelationshipsQuerySingleStoreTest(
            TPTRelationshipsQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture)
            => fixture.TestSqlLoggerFactory.Clear();

        public class TPTRelationshipsQuerySingleStoreFixture : TPTRelationshipsQueryRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;
        }
    }
}
