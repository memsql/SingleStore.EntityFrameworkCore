using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class QueryNoClientEvalSingleStoreFixture : NorthwindQuerySingleStoreFixture<NoopModelCustomizer>
    {
        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder);
    }
}
