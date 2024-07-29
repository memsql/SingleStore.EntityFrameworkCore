namespace EntityFrameworkCore.SingleStore.FunctionalTests.BulkUpdates;

public class TPTFiltersInheritanceBulkUpdatesSingleStoreFixture : TPTInheritanceBulkUpdatesSingleStoreFixture
{
    protected override string StoreName
        => "TPTFiltersInheritanceBulkUpdatesTest";

    protected override bool EnableFilters
        => true;
}
