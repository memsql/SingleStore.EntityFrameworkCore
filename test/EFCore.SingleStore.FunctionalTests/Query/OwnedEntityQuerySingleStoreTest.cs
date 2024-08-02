using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class OwnedEntityQuerySingleStoreTest : OwnedEntityQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        public override async Task Multiple_single_result_in_projection_containing_owned_types(bool async)
        {
            await base.Multiple_single_result_in_projection_containing_owned_types(async);

            AssertSql(
                @"SELECT `e`.`Id`, `t0`.`Id`, `t0`.`Entity20277Id`, `t0`.`Owned_IsDeleted`, `t0`.`Owned_Value`, `t0`.`Type`, `t0`.`c`, `t1`.`Id`, `t1`.`Entity20277Id`, `t1`.`Owned_IsDeleted`, `t1`.`Owned_Value`, `t1`.`Type`, `t1`.`c`
FROM `Entities` AS `e`
LEFT JOIN (
    SELECT `t`.`Id`, `t`.`Entity20277Id`, `t`.`Owned_IsDeleted`, `t`.`Owned_Value`, `t`.`Type`, `t`.`c`
    FROM (
        SELECT `c`.`Id`, `c`.`Entity20277Id`, `c`.`Owned_IsDeleted`, `c`.`Owned_Value`, `c`.`Type`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `c`.`Entity20277Id` ORDER BY `c`.`Entity20277Id`, `c`.`Id`) AS `row`
        FROM `Child20277` AS `c`
        WHERE `c`.`Type` = 1
    ) AS `t`
    WHERE `t`.`row` <= 1
) AS `t0` ON `e`.`Id` = `t0`.`Entity20277Id`
LEFT JOIN (
    SELECT `t2`.`Id`, `t2`.`Entity20277Id`, `t2`.`Owned_IsDeleted`, `t2`.`Owned_Value`, `t2`.`Type`, `t2`.`c`
    FROM (
        SELECT `c0`.`Id`, `c0`.`Entity20277Id`, `c0`.`Owned_IsDeleted`, `c0`.`Owned_Value`, `c0`.`Type`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `c0`.`Entity20277Id` ORDER BY `c0`.`Entity20277Id`, `c0`.`Id`) AS `row`
        FROM `Child20277` AS `c0`
        WHERE `c0`.`Type` = 2
    ) AS `t2`
    WHERE `t2`.`row` <= 1
) AS `t1` ON `e`.`Id` = `t1`.`Entity20277Id`");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Owned_reference_mapped_to_different_table_nested_updated_correctly_after_subquery_pushdown(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            var contextFactory = await InitializeAsync<MyContext26592>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Company>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Owner>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            await base.Owned_references_on_same_level_nested_expanded_at_different_times_around_take_helper(context, async);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Owned_reference_mapped_to_different_table_updated_correctly_after_subquery_pushdown(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            var contextFactory = await InitializeAsync<MyContext26592>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Company>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Owner>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            await base.Owned_references_on_same_level_expanded_at_different_times_around_take_helper(context, async);
        }

        public override async Task Owned_collection_basic_split_query(bool async)
        {
            // Use custom context to set prefix length, so we don't exhaust the max. key length.
            var contextFactory = await InitializeAsync<Context25680>(onModelCreating: modelBuilder =>
            {
                modelBuilder.Entity<Location25680>().OwnsMany(e => e.PublishTokenTypes,
                    b =>
                    {
                        b.WithOwner(e => e.Location).HasForeignKey(e => e.LocationId);
                        b.HasKey(e => new { e.LocationId, e.ExternalId, e.VisualNumber, e.TokenGroupId })
                            .HasPrefixLength(0, 128, 128, 128); // <-- set prefix length, so we don't exhaust the max. key length
                    });
            });

            using var context = contextFactory.CreateContext();

            var id = new Guid("6c1ae3e5-30b9-4c77-8d98-f02075974a0a");
            var query = context.Set<Location25680>().Where(e => e.Id == id).AsSplitQuery();
            var result = async
                ? await query.FirstOrDefaultAsync()
                : query.FirstOrDefault();
        }

        public override async Task Projecting_correlated_collection_property_for_owned_entity(bool async)
        {
            var contextFactory = await InitializeAsync<MyContext18582>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Warehouse>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using var context = contextFactory.CreateContext();
            var query = context.Warehouses.Select(x => new WarehouseModel
            {
                WarehouseCode = x.WarehouseCode,
                DestinationCountryCodes = x.DestinationCountries.Select(c => c.CountryCode)
                    .OrderByDescending(c => c) // <-- explicitly order
                    .ToArray()
            }).AsNoTracking();

            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            var warehouseModel = Assert.Single(result);
            Assert.Equal("W001", warehouseModel.WarehouseCode);
            Assert.True(new[] { "US", "CA" }.SequenceEqual(warehouseModel.DestinationCountryCodes));
        }
    }
}
