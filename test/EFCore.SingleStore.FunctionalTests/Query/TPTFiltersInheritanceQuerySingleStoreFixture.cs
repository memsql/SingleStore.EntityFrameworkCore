namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQuerySingleStoreFixture : TPTInheritanceQuerySingleStoreFixture
    {
        protected override bool EnableFilters
            => true;
    }
}
