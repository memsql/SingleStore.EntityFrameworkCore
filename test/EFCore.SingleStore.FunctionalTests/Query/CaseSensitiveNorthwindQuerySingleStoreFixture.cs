using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class CaseSensitiveNorthwindQuerySingleStoreFixture<TModelCustomizer> : NorthwindQuerySingleStoreFixture<TModelCustomizer>
        where TModelCustomizer : ITestModelCustomizer, new()
    {
        protected override string StoreName => "NorthwindCs";
        protected override ITestStoreFactory TestStoreFactory => SingleStoreNorthwindTestStoreFactory.InstanceCs;
    }
}
