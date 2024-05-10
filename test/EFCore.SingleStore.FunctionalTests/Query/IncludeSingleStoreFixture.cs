using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class IncludeSingleStoreFixture : NorthwindQuerySingleStoreFixture<NoopModelCustomizer>
    {
        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder);

        protected override bool ShouldLogCategory(string logCategory)
            => base.ShouldLogCategory(logCategory) || logCategory == DbLoggerCategory.Query.Name;
    }
}
