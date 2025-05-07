// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class SharedTypeQuerySingleStoreTest : SharedTypeQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        public override async Task Can_use_shared_type_entity_type_in_query_filter(bool async)
        {
            var contextFactory = await InitializeAsync<MyContext24601>(
                seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                modelBuilder.SharedTypeEntity<Dictionary<string, object>>("STET",
                    b =>
                    {
                        // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                        // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                        b.IndexerProperty<long>("Id");
                        b.IndexerProperty<string>("Value");
                    });
            });

            using var context = contextFactory.CreateContext();
            var query = context.Set<ViewQuery24601>();
            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Empty(result);
            AssertSql(
                @"SELECT `v`.`Value`
FROM `ViewQuery24601` AS `v`
WHERE EXISTS (
    SELECT 1
    FROM `STET` AS `s`
    WHERE (`s`.`Value` = `v`.`Value`) OR (`s`.`Value` IS NULL AND (`v`.`Value` IS NULL)))");
        }

        public override async Task Can_use_shared_type_entity_type_in_query_filter_with_from_sql(bool async)
        {
            var contextFactory = await InitializeAsync<MyContextRelational24601>(
                seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                modelBuilder.SharedTypeEntity<Dictionary<string, object>>("STET",
                    b =>
                    {
                        // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                        // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                        b.IndexerProperty<long>("Id");
                        b.IndexerProperty<string>("Value");
                    });
            });

            using var context = contextFactory.CreateContext();
            var query = context.Set<ViewQuery24601>();
            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Empty(result);

            AssertSql(
                @"SELECT `v`.`Value`
FROM `ViewQuery24601` AS `v`
WHERE EXISTS (
    SELECT 1
    FROM (
        Select * from STET
    ) AS `s`
    WHERE (`s`.`Value` = `v`.`Value`) OR (`s`.`Value` IS NULL AND (`v`.`Value` IS NULL)))");
        }

        [ConditionalFact]
        public override void Ad_hoc_query_for_shared_type_entity_type_works()
        {
            var contextFactory = Initialize<MyContextRelational24601>(
                seed: c => c.Seed(), onModelCreating: modelBuilder =>
                {
                    modelBuilder.SharedTypeEntity<Dictionary<string, object>>("STET",
                        b =>
                        {
                            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                            b.IndexerProperty<long>("Id");
                            b.IndexerProperty<string>("Value");
                        });
                });

            using var context = contextFactory.CreateContext();

            var result = context.Database.SqlQueryRaw<ViewQuery24601>(
                ((RelationalTestStore)TestStore).NormalizeDelimitersInRawString(@"SELECT * FROM [ViewQuery24601]"));

            Assert.Empty(result);
        }

        [ConditionalFact]
        public override void Ad_hoc_query_for_default_shared_type_entity_type_throws()
        {
            var contextFactory = Initialize<MyContextRelational24601>(
                seed: c => c.Seed(), onModelCreating: modelBuilder =>
                {
                    modelBuilder.SharedTypeEntity<Dictionary<string, object>>("STET",
                        b =>
                        {
                            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                            b.IndexerProperty<long>("Id");
                            b.IndexerProperty<string>("Value");
                        });
                });

            using var context = contextFactory.CreateContext();

            Assert.Equal(
                CoreStrings.ClashingSharedType("Dictionary<string, object>"),
                Assert.Throws<InvalidOperationException>(
                    () => context.Database.SqlQueryRaw<Dictionary<string, object>>(@"SELECT * FROM X")).Message);
        }
    }
}
