using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class InheritanceRelationshipsQuerySingleStoreTest : InheritanceRelationshipsQueryRelationalTestBase<InheritanceRelationshipsQuerySingleStoreFixture>
    {
        public InheritanceRelationshipsQuerySingleStoreTest(InheritanceRelationshipsQuerySingleStoreFixture fixture)
            : base(fixture)
        {
        }
    }
}
