namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPCFiltersInheritanceQuerySingleStoreFixture : TPCInheritanceQuerySingleStoreFixture
{
    public override bool EnableFilters
        => true;
}
