using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class InheritanceRelationshipsQuerySingleStoreFixture : InheritanceRelationshipsQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;
    }
}
