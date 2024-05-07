using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class DatabindingSingleStoreTest : DatabindingTestBase<F1SingleStoreFixture>
    {
        public DatabindingSingleStoreTest(F1SingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public override void DbSet_Local_calls_DetectChanges()
        {
            base.DbSet_Local_calls_DetectChanges();
        }
    }
}
