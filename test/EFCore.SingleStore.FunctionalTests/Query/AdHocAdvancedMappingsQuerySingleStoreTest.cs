using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocAdvancedMappingsQuerySingleStoreTest : AdHocAdvancedMappingsQueryRelationalTestBase
{
    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;

    [SkippableTheory]
    public override async Task Query_generates_correct_datetime2_parameter_definition(int? fractionalSeconds, string postfix)
    {
        if (fractionalSeconds is not (0 or 6))
        {
            Skip.If(true, "SingleStore supports DATETIME precision only 0 or 6.");
        }

        await base.Query_generates_correct_datetime2_parameter_definition(fractionalSeconds, postfix);
    }

    [SkippableTheory]
    public override async Task Query_generates_correct_datetimeoffset_parameter_definition(int? fractionalSeconds, string postfix)
    {
        if (fractionalSeconds is not (0 or 6))
        {
            Skip.If(true, "SingleStore supports DATETIME precision only 0 or 6.");
        }

        await base.Query_generates_correct_datetimeoffset_parameter_definition(fractionalSeconds, postfix);
    }

    [SkippableTheory]
    public override async Task Query_generates_correct_timespan_parameter_definition(int? fractionalSeconds, string postfix)
    {
        if (fractionalSeconds is not (0 or 6))
        {
            Skip.If(true, "SingleStore supports DATETIME precision only 0 or 6.");
        }

        await base.Query_generates_correct_timespan_parameter_definition(fractionalSeconds, postfix);
    }

    public override async Task Two_similar_complex_properties_projected_with_split_query1()
    {
        await base.Two_similar_complex_properties_projected_with_split_query1();

        AssertSql(
"""
SELECT `o`.`Id`
FROM `Offers` AS `o`
ORDER BY `o`.`Id`
""",
                //
                """
SELECT `s`.`Id`, `s`.`NestedId`, `s`.`OfferId`, `s`.`payment_brutto`, `s`.`payment_netto`, `s`.`Id0`, `s`.`payment_brutto0`, `s`.`payment_netto0`, `o`.`Id`
FROM `Offers` AS `o`
INNER JOIN (
    SELECT `v`.`Id`, `v`.`NestedId`, `v`.`OfferId`, `v`.`payment_brutto`, `v`.`payment_netto`, `n`.`Id` AS `Id0`, `n`.`payment_brutto` AS `payment_brutto0`, `n`.`payment_netto` AS `payment_netto0`
    FROM `Variation` AS `v`
    LEFT JOIN `NestedEntity` AS `n` ON `v`.`NestedId` = `n`.`Id`
) AS `s` ON `o`.`Id` = `s`.`OfferId`
ORDER BY `o`.`Id`
""");
    }

    public override async Task Two_similar_complex_properties_projected_with_split_query2()
    {
        await base.Two_similar_complex_properties_projected_with_split_query2();

        AssertSql(
"""
SELECT `o`.`Id`
FROM `Offers` AS `o`
WHERE `o`.`Id` = 1
ORDER BY `o`.`Id`
LIMIT 2
""",
                //
                """
SELECT `s`.`Id`, `s`.`NestedId`, `s`.`OfferId`, `s`.`payment_brutto`, `s`.`payment_netto`, `s`.`Id0`, `s`.`payment_brutto0`, `s`.`payment_netto0`, `o0`.`Id`
FROM (
    SELECT `o`.`Id`
    FROM `Offers` AS `o`
    WHERE `o`.`Id` = 1
    LIMIT 1
) AS `o0`
INNER JOIN (
    SELECT `v`.`Id`, `v`.`NestedId`, `v`.`OfferId`, `v`.`payment_brutto`, `v`.`payment_netto`, `n`.`Id` AS `Id0`, `n`.`payment_brutto` AS `payment_brutto0`, `n`.`payment_netto` AS `payment_netto0`
    FROM `Variation` AS `v`
    LEFT JOIN `NestedEntity` AS `n` ON `v`.`NestedId` = `n`.`Id`
) AS `s` ON `o0`.`Id` = `s`.`OfferId`
ORDER BY `o0`.`Id`
""");
    }

    public override async Task Projecting_one_of_two_similar_complex_types_picks_the_correct_one()
    {
        await base.Projecting_one_of_two_similar_complex_types_picks_the_correct_one();

        AssertSql(
"""
@__p_0='10'

SELECT `a`.`Id`, `s`.`Info_Created0` AS `Created`
FROM (
    SELECT `c`.`Id`, `b`.`AId`, `b`.`Info_Created` AS `Info_Created0`
    FROM `Cs` AS `c`
    INNER JOIN `Bs` AS `b` ON `c`.`BId` = `b`.`Id`
    WHERE `b`.`AId` = 1
    ORDER BY `c`.`Id`
    LIMIT @__p_0
) AS `s`
LEFT JOIN `As` AS `a` ON `s`.`AId` = `a`.`Id`
ORDER BY `s`.`Id`
""");
    }

    public override async Task Projection_failing_with_EnumToStringConverter()
    {
        var contextFactory = await InitializeAsync<Context15684>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context15684.Product>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Context15684.Category>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using var context = contextFactory.CreateContext();
        var query = from p in context.Products
            join c in context.Categories on p.CategoryId equals c.Id into grouping
            from c in grouping.DefaultIfEmpty()
            select new Context15684.ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = c == null ? "Other" : c.Name,
                CategoryStatus = c == null ? Context15684.CategoryStatus.Active : c.Status
            };
        var result = query.ToList();
        Assert.Equal(2, result.Count);
    }

    public override async Task Projecting_correlated_collection_along_with_non_mapped_property()
    {
        var contextFactory = await InitializeAsync<Context11835>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context11835.Blog>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Context11835.Post>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var result = context.Blogs.Select(
                e => new
                {
                    e.Id,
                    e.Title,
                    FirstPostName = e.Posts.Where(i => i.Name.Contains("2")).ToList()
                }).ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var result = context.Blogs.Select(
                e => new
                {
                    e.Id,
                    e.Title,
                    FirstPostName = e.Posts.OrderBy(i => i.Id).FirstOrDefault().Name
                }).ToList();
        }
    }

    [ConditionalFact]
    public override async Task Double_convert_interface_created_expression_tree()
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var contextFactory = await InitializeAsync<Context17794>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context17794.Offer>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<Context17794.OfferAction>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using var context = contextFactory.CreateContext();
        var expression = Context17794.HasAction17794<Context17794.Offer>(Context17794.Actions.Accepted);
        var query = context.Offers.Where(expression).Count();

        Assert.Equal(1, query);
    }

    public override async Task Casts_are_removed_from_expression_tree_when_redundant()
    {
        var contextFactory = await InitializeAsync<Context18087>(
            seed: c => c.SeedAsync(),
            onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the field from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column.
                modelBuilder.Entity<Context18087.MockEntity>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

        using (var context = contextFactory.CreateContext())
        {
            var expected = context.MockEntities
                .OrderBy(e => e.Id)
                .Select(e => e.Id)
                .First();

            var queryBase = (IQueryable)context.MockEntities;
            var query = queryBase
                .Cast<Context18087.IDomainEntity>()
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            Assert.NotNull(query);
            Assert.Equal(expected, query.Id);
        }

        using (var context = contextFactory.CreateContext())
        {
            var queryBase = (IQueryable)context.MockEntities;
            var query = queryBase.Cast<object>().Count();

            Assert.Equal(3, query);
        }

        using (var context = contextFactory.CreateContext())
        {
            var queryBase = (IQueryable)context.MockEntities;
            var id = context.MockEntities
                .OrderBy(e => e.Id)
                .Select(e => e.Id)
                .First();

            var message = Assert.Throws<InvalidOperationException>(
                () => queryBase.Cast<Context18087.IDummyEntity>().FirstOrDefault(x => x.Id == id)).Message;

            Assert.Equal(
                CoreStrings.TranslationFailed(
                    @"DbSet<MockEntity>()    .Cast<IDummyEntity>()    .Where(e => e.Id == __id_0)"),
                message.Replace("\r", "").Replace("\n", ""));
        }
    }

    public override async Task Can_query_hierarchy_with_non_nullable_property_on_derived()
    {
        var contextFactory = await InitializeAsync<Context18346>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context18346.Business>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using var context = contextFactory.CreateContext();
        var query = context.Businesses.ToList();
        Assert.Equal(3, query.Count);
    }
}
