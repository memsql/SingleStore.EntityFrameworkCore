using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class CaseSensitiveWithStringComparisonNorthwindQuerySingleStoreFixture<TModelCustomizer> : CaseSensitiveNorthwindQuerySingleStoreFixture<TModelCustomizer>
    where TModelCustomizer : ITestModelCustomizer, new()
{
    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    {
        var optionsBuilder = base.AddOptions(builder);
        new SingleStoreDbContextOptionsBuilder(optionsBuilder).EnableStringComparisonTranslations();
        return optionsBuilder;
    }
}
