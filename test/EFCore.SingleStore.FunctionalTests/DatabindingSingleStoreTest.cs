using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class DatabindingSingleStoreTest : DataBindingTestBase<F1SingleStoreFixture>
    {
        public DatabindingSingleStoreTest(F1SingleStoreFixture fixture)
            : base(fixture)
        {
        }
    }
}
