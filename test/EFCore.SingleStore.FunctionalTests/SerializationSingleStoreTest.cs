using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class SerializationSingleStoreTest : SerializationTestBase<F1SingleStoreFixture>
    {
        public SerializationSingleStoreTest(F1SingleStoreFixture fixture)
            : base(fixture)
        {
        }
    }
}
