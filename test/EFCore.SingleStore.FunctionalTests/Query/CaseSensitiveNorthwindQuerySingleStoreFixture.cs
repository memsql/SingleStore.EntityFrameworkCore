using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class CaseSensitiveNorthwindQuerySingleStoreFixture<TModelCustomizer> : NorthwindQuerySingleStoreFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        protected override string StoreName => "NorthwindCs";
        protected override ITestStoreFactory TestStoreFactory => SingleStoreNorthwindTestStoreFactory.InstanceCs;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new SingleStoreDbContextOptionsBuilder(optionsBuilder).EnableStringComparisonTranslations();
            return optionsBuilder;
        }
    }
}
