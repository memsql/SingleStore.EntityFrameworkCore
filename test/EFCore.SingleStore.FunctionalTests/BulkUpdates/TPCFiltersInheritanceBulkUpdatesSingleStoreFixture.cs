namespace EntityFrameworkCore.SingleStore.FunctionalTests.BulkUpdates;

public class TPCFiltersInheritanceBulkUpdatesSingleStoreFixture : TPCInheritanceBulkUpdatesSingleStoreFixture
{
    protected override string StoreName
        => "TPCFiltersInheritanceBulkUpdatesTest";

    protected override bool EnableFilters
        => true;
}
