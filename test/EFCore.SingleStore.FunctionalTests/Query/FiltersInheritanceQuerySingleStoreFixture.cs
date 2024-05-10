namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FiltersInheritanceQuerySingleStoreFixture : InheritanceQuerySingleStoreFixture
    {
        protected override bool EnableFilters => true;
    }
}
