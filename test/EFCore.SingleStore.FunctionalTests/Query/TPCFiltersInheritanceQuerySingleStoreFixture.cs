namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPCFiltersInheritanceQuerySingleStoreFixture : TPCInheritanceQuerySingleStoreFixture
{
    protected override bool EnableFilters
        => true;
}
