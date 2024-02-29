using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class ManyToManyHeterogeneousQuerySingleStoreTest : ManyToManyHeterogeneousQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        public override async Task Many_to_many_load_works_when_join_entity_has_custom_key(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            var contextFactory = await InitializeAsync<Context20277>(onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<ManyM_DB>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ManyN_DB>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ManyMN_DB>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            int id;
            using (var context = contextFactory.CreateContext())
            {
                var m = new ManyM_DB();
                var n = new ManyN_DB();
                context.AddRange(m, n);
                m.ManyN_DB = new List<ManyN_DB> { n };

                context.SaveChanges();

                id = m.Id;
            }

            ClearLog();

            using (var context = contextFactory.CreateContext())
            {
                var m = context.Find<ManyM_DB>(id);

                if (async)
                {
                    await context.Entry(m).Collection(x => x.ManyN_DB).LoadAsync();
                }
                else
                {
                    context.Entry(m).Collection(x => x.ManyN_DB).Load();
                }

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
                Assert.Equal(1, m.ManyN_DB.Count);
                Assert.Equal(1, m.ManyN_DB.Single().ManyM_DB.Count);
                Assert.Equal(1, m.ManyNM_DB.Count);
                Assert.Equal(1, m.ManyN_DB.Single().ManyNM_DB.Count);

                id = m.ManyN_DB.Single().Id;
            }

            using (var context = contextFactory.CreateContext())
            {
                var n = context.Find<ManyN_DB>(id);

                if (async)
                {
                    await context.Entry(n).Collection(x => x.ManyM_DB).LoadAsync();
                }
                else
                {
                    context.Entry(n).Collection(x => x.ManyM_DB).Load();
                }

                Assert.Equal(3, context.ChangeTracker.Entries().Count());
                Assert.Equal(1, n.ManyM_DB.Count);
                Assert.Equal(1, n.ManyM_DB.Single().ManyN_DB.Count);
                Assert.Equal(1, n.ManyNM_DB.Count);
                Assert.Equal(1, n.ManyM_DB.Single().ManyNM_DB.Count);
            }

            AssertSql(
                @"@__p_0='1'

SELECT `m`.`Id`
FROM `ManyM_DB` AS `m`
WHERE `m`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__p_0='1'

SELECT `t`.`Id`, `m`.`Id`, `t`.`Id0`, `t0`.`Id`, `t0`.`ManyM_Id`, `t0`.`ManyN_Id`, `t0`.`Id0`
FROM `ManyM_DB` AS `m`
INNER JOIN (
    SELECT `m1`.`Id`, `m0`.`Id` AS `Id0`, `m0`.`ManyM_Id`
    FROM `ManyMN_DB` AS `m0`
    LEFT JOIN `ManyN_DB` AS `m1` ON `m0`.`ManyN_Id` = `m1`.`Id`
) AS `t` ON `m`.`Id` = `t`.`ManyM_Id`
LEFT JOIN (
    SELECT `m2`.`Id`, `m2`.`ManyM_Id`, `m2`.`ManyN_Id`, `m3`.`Id` AS `Id0`
    FROM `ManyMN_DB` AS `m2`
    INNER JOIN `ManyM_DB` AS `m3` ON `m2`.`ManyM_Id` = `m3`.`Id`
    WHERE `m3`.`Id` = @__p_0
) AS `t0` ON `t`.`Id` = `t0`.`ManyN_Id`
WHERE `m`.`Id` = @__p_0
ORDER BY `m`.`Id`, `t`.`Id0`, `t`.`Id`, `t0`.`Id`",
                //
                @"@__p_0='1'

SELECT `m`.`Id`
FROM `ManyN_DB` AS `m`
WHERE `m`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__p_0='1'

SELECT `t`.`Id`, `m`.`Id`, `t`.`Id0`, `t0`.`Id`, `t0`.`ManyM_Id`, `t0`.`ManyN_Id`, `t0`.`Id0`
FROM `ManyN_DB` AS `m`
INNER JOIN (
    SELECT `m1`.`Id`, `m0`.`Id` AS `Id0`, `m0`.`ManyN_Id`
    FROM `ManyMN_DB` AS `m0`
    INNER JOIN `ManyM_DB` AS `m1` ON `m0`.`ManyM_Id` = `m1`.`Id`
) AS `t` ON `m`.`Id` = `t`.`ManyN_Id`
LEFT JOIN (
    SELECT `m2`.`Id`, `m2`.`ManyM_Id`, `m2`.`ManyN_Id`, `m3`.`Id` AS `Id0`
    FROM `ManyMN_DB` AS `m2`
    INNER JOIN `ManyN_DB` AS `m3` ON `m2`.`ManyN_Id` = `m3`.`Id`
    WHERE `m3`.`Id` = @__p_0
) AS `t0` ON `t`.`Id` = `t0`.`ManyM_Id`
WHERE `m`.`Id` = @__p_0
ORDER BY `m`.`Id`, `t`.`Id0`, `t`.`Id`, `t0`.`Id`");
        }
    }
}
