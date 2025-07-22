namespace EntityFrameworkCore.SingleStore.FunctionalTests.BulkUpdates;

public class TPCFiltersInheritanceBulkUpdatesSingleStoreFixture : TPCInheritanceBulkUpdatesSingleStoreFixture
{
    protected override string StoreName
        => "TPCFiltersInheritanceBulkUpdatesTest";

    public override bool EnableFilters
        => true;
}
