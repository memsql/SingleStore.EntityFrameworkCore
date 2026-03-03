using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
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
                """
                SELECT `e`.`Id`, `c2`.`Id`, `c2`.`EntityId`, `c2`.`Owned_IsDeleted`, `c2`.`Owned_Value`, `c2`.`Type`, `c2`.`c`, `c4`.`Id`, `c4`.`EntityId`, `c4`.`Owned_IsDeleted`, `c4`.`Owned_Value`, `c4`.`Type`, `c4`.`c`
                FROM `Entities` AS `e`
                LEFT JOIN (
                    SELECT `c1`.`Id`, `c1`.`EntityId`, `c1`.`Owned_IsDeleted`, `c1`.`Owned_Value`, `c1`.`Type`, `c1`.`c`
                    FROM (
                        SELECT `c`.`Id`, `c`.`EntityId`, `c`.`Owned_IsDeleted`, `c`.`Owned_Value`, `c`.`Type`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `c`.`EntityId` ORDER BY `c`.`EntityId`, `c`.`Id`) AS `row`
                        FROM `Child` AS `c`
                        WHERE `c`.`Type` = 1
                    ) AS `c1`
                    WHERE `c1`.`row` <= 1
                ) AS `c2` ON `e`.`Id` = `c2`.`EntityId`
                LEFT JOIN (
                    SELECT `c3`.`Id`, `c3`.`EntityId`, `c3`.`Owned_IsDeleted`, `c3`.`Owned_Value`, `c3`.`Type`, `c3`.`c`
                    FROM (
                        SELECT `c0`.`Id`, `c0`.`EntityId`, `c0`.`Owned_IsDeleted`, `c0`.`Owned_Value`, `c0`.`Type`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `c0`.`EntityId` ORDER BY `c0`.`EntityId`, `c0`.`Id`) AS `row`
                        FROM `Child` AS `c0`
                        WHERE `c0`.`Type` = 2
                    ) AS `c3`
                    WHERE `c3`.`row` <= 1
                ) AS `c4` ON `e`.`Id` = `c4`.`EntityId`
                """);
        }

        [ConditionalTheory]
        public override async Task Owned_entity_with_all_null_properties_entity_equality_when_not_containing_another_owned_entity(
            bool async)
        {
            var contextFactory = await InitializeAsync<Context28247>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context28247.RotRutCase>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using var context = contextFactory.CreateContext();
            var query = context.RotRutCases.AsNoTracking().Select(e => e.Rot).Where(e => e != null);

            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Collection(
                result,
                t =>
                {
                    Assert.Equal(1, t.ServiceType);
                    Assert.Equal("1", t.ApartmentNo);
                });
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
            var contextFactory = await InitializeAsync<Context18582>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context18582.Warehouse>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context18582.Warehouse>().OwnsMany(
                    e => e.DestinationCountries,
                    b =>
                    {
                        b.Property("Id")
                            .HasConversion<long>()
                            .HasColumnType("bigint");
                    });
            });

            using var context = contextFactory.CreateContext();
            var query = context.Warehouses.Select(
                x => new Context18582.WarehouseModel
                {
                    WarehouseCode = x.WarehouseCode,
                    DestinationCountryCodes = x.DestinationCountries.OrderBy(c => c.CountryCode).Select(c => c.CountryCode).ToArray()
                }).AsNoTracking();

            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            var warehouseModel = Assert.Single(result);
            Assert.Equal("W001", warehouseModel.WarehouseCode);
            Assert.True(new[] { "CA", "US" }.SequenceEqual(warehouseModel.DestinationCountryCodes));
        }

        // Use base implementation once https://github.com/dotnet/efcore/pull/32509#issuecomment-1948812777 is fixed and the base
        // implementation has been fixed to use a deterministic order.
        public override async Task Correlated_subquery_with_owned_navigation_being_compared_to_null_works()
        {
            var contextFactory = await InitializeAsync<Context13157>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context13157.Partner>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context13157.Address>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using (var context = contextFactory.CreateContext())
            {
                var partners = context.Partners
                    .Select(
                        x => new
                        {
                            Addresses = x.Addresses.Select(
                                    y => new
                                    {
                                        Turnovers = y.Turnovers == null
                                            ? null
                                            : new { y.Turnovers.AmountIn }
                                    })
                                .ToList()
                        }).ToList();

                Assert.Single(partners);
                Assert.Collection(
                    partners[0].Addresses
                        .OrderBy(a => a.Turnovers is null), // <-- explicitly order to make deterministic
                    t =>
                    {
                        Assert.NotNull(t.Turnovers);
                        Assert.Equal(10, t.Turnovers.AmountIn);
                    },
                    t =>
                    {
                        Assert.Null(t.Turnovers);
                    });
            }

            AssertSql(
                """
                SELECT `p`.`Id`, `a`.`Turnovers_AmountIn` IS NULL, `a`.`Turnovers_AmountIn`, `a`.`Id`
                FROM `Partners` AS `p`
                LEFT JOIN `Address` AS `a` ON `p`.`Id` = `a`.`PartnerId`
                ORDER BY `p`.`Id`
                """);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Owned_entity_with_all_null_properties_materializes_when_not_containing_another_owned_entity(bool async)
        {
            var contextFactory = await InitializeAsync<Context28247>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context28247.RotRutCase>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using var context = contextFactory.CreateContext();
            var query = context.RotRutCases.OrderBy(e => e.Buyer);

            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Collection(
                result,
                t =>
                {
                    Assert.Equal("Buyer1", t.Buyer);
                    Assert.NotNull(t.Rot);
                    Assert.Equal(1, t.Rot.ServiceType);
                    Assert.Equal("1", t.Rot.ApartmentNo);
                    Assert.NotNull(t.Rut);
                    Assert.Equal(1, t.Rut.Value);
                },
                t =>
                {
                    Assert.Equal("Buyer2", t.Buyer);
                    Assert.Null(t.Rot);
                    Assert.Null(t.Rut);
                });
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Owned_entity_with_all_null_properties_property_access_when_not_containing_another_owned_entity(bool async)
        {
            var contextFactory = await InitializeAsync<Context28247>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context28247.RotRutCase>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using var context = contextFactory.CreateContext();
            var query = context.RotRutCases.AsNoTracking().Select(e => e.Rot.ApartmentNo);

            var result = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Collection(
                result,
                t =>
                {
                    Assert.Equal("1", t);
                },
                t =>
                {
                    Assert.Null(t);
                });
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Owned_reference_mapped_to_different_table_nested_updated_correctly_after_subquery_pushdown(bool async)
        {
            var contextFactory = await InitializeAsync<MyContext26592>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<MyContext26592.Company>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<MyContext26592.Owner>()
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
            var contextFactory = await InitializeAsync<MyContext26592>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<MyContext26592.Company>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<MyContext26592.Owner>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            await base.Owned_references_on_same_level_expanded_at_different_times_around_take_helper(context, async);
        }

        [ConditionalFact]
        public override async Task Accessing_scalar_property_in_derived_type_projection_does_not_load_owned_navigations()
        {
            var contextFactory = await InitializeAsync<Context19138>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context19138.BaseEntity>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context19138.OtherEntity>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();
            var result = context.BaseEntities
                .Select(b => context.OtherEntities.Where(o => o.OtherEntityData == ((Context19138.SubEntity)b).Data).FirstOrDefault())
                .ToList();

            Assert.Equal("A", Assert.Single(result).OtherEntityData);
        }

        [ConditionalFact]
        public override async Task Can_auto_include_navigation_from_model()
        {
            var contextFactory = await InitializeAsync<Context21540>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context21540.OtherSide>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context21540.Parent>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context21540.Collection>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context21540.Reference>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using (var context = contextFactory.CreateContext())
            {
                var query = context.Parents.AsNoTracking().ToList();
                var result = Assert.Single(query);
                Assert.NotNull(result.OwnedReference);
                Assert.NotNull(result.Reference);
                Assert.NotNull(result.Collection);
                Assert.Equal(2, result.Collection.Count);
                Assert.NotNull(result.SkipOtherSide);
                Assert.Single(result.SkipOtherSide);
            }

            using (var context = contextFactory.CreateContext())
            {
                var query = context.Parents.AsNoTracking().IgnoreAutoIncludes().ToList();
                var result = Assert.Single(query);
                Assert.NotNull(result.OwnedReference);
                Assert.Null(result.Reference);
                Assert.Null(result.Collection);
                Assert.Null(result.SkipOtherSide);
            }
        }

        [ConditionalFact]
        public override async Task Include_collection_for_entity_with_owned_type_works()
        {
            var contextFactory = await InitializeAsync<Context9202>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context9202.Movie>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context9202.Actor>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using (var context = contextFactory.CreateContext())
            {
                var query = context.Movies.Include(m => m.Cast);
                var result = query.ToList();

                Assert.Single(result);
                Assert.Equal(3, result[0].Cast.Count);
                Assert.NotNull(result[0].Details);
                Assert.True(result[0].Cast.All(a => a.Details != null));
            }

            using (var context = contextFactory.CreateContext())
            {
                var query = context.Movies.Include("Cast");
                var result = query.ToList();

                Assert.Single(result);
                Assert.Equal(3, result[0].Cast.Count);
                Assert.NotNull(result[0].Details);
                Assert.True(result[0].Cast.All(a => a.Details != null));
            }
        }

        [ConditionalFact]
        public override async Task Multilevel_owned_entities_determine_correct_nullability()
        {
            var contextFactory = await InitializeAsync<Context13079>(onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context13079.BaseEntity>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();
            await context.AddAsync(new Context13079.BaseEntity());
            await context.SaveChangesAsync();
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed")]
        [MemberData("IsAsyncData", new object[] { })]
        public override async Task Projecting_owned_collection_and_aggregate(bool async)
        {
            await base.Projecting_owned_collection_and_aggregate(async);
        }

        [ConditionalTheory(Skip = "It's impossible to override this test so it could be run against SingleStore")]
        [MemberData("IsAsyncData", new object[] { })]
        public override async Task Join_selects_with_duplicating_aliases_and_owned_expansion_uniquifies_correctly(
            bool async)
        {
            await base.Join_selects_with_duplicating_aliases_and_owned_expansion_uniquifies_correctly(async);
        }

        [ConditionalTheory(Skip = "It's impossible to override this test so it could be run against SingleStore")]
        [MemberData("IsAsyncData", new object[] { })]
        public override async Task Owned_entity_with_all_null_properties_in_compared_to_non_null_in_conditional_projection(
            bool async)
        {
            await base.Owned_entity_with_all_null_properties_in_compared_to_non_null_in_conditional_projection(async);
        }

        [ConditionalTheory(Skip = "It's impossible to override this test so it could be run against SingleStore")]
        [MemberData("IsAsyncData", new object[] { })]
        public override async Task Owned_entity_with_all_null_properties_in_compared_to_null_in_conditional_projection(
            bool async)
        {
            await base.Owned_entity_with_all_null_properties_in_compared_to_null_in_conditional_projection(async);
        }

        [ConditionalTheory(Skip = "It's impossible to override this test so it could be run against SingleStore")]
        public override async Task Owned_entity_multiple_level_in_aggregate()
        {
            await base.Owned_entity_multiple_level_in_aggregate();
        }
    }
}
