namespace EntityFrameworkCore.SingleStore.FunctionalTests.BulkUpdates;

public class FiltersInheritanceBulkUpdatesSingleStoreFixture : InheritanceBulkUpdatesSingleStoreFixture
{
    protected override string StoreName
        => "FiltersInheritanceBulkUpdatesTest";

    protected override bool EnableFilters
        => true;
}
