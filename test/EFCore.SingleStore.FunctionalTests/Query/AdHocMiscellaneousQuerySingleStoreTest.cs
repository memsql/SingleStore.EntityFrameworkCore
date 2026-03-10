using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using NameSpace1;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocMiscellaneousQuerySingleStoreTest : AdHocMiscellaneousQueryRelationalTestBase
{
    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;

    protected override Task Seed2951(Context2951 context)
        => context.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE `ZeroKey` (`Id` int);
            INSERT INTO `ZeroKey` VALUES (NULL)
            """);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Aggregate_over_subquery_in_group_by_projection(bool async)
    {
        var contextFactory = await InitializeAsync<Context27083>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context27083.Customer>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context27083.Order>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context27083.Project>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context27083.TimeSheet>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using var context = contextFactory.CreateContext();

        Expression<Func<Context27083.Order, bool>> someFilterFromOutside = x => x.Number != "A1";

        var query = context
            .Set<Context27083.Order>()
            .Where(someFilterFromOutside)
            .GroupBy(x => new { x.CustomerId, x.Number })
            .Select(
                x => new
                {
                    x.Key.CustomerId,
                    CustomerMinHourlyRate =
                        context.Set<Context27083.Order>().Where(n => n.CustomerId == x.Key.CustomerId).Min(h => h.HourlyRate),
                    HourlyRate = x.Min(f => f.HourlyRate),
                    Count = x.Count()
                });

        var orders = async
            ? await query.ToListAsync()
            : query.ToList();

        Assert.Collection(
            orders.OrderBy(x => x.CustomerId),
            t =>
            {
                Assert.Equal(1, t.CustomerId);
                Assert.Equal(10, t.CustomerMinHourlyRate);
                Assert.Equal(11, t.HourlyRate);
                Assert.Equal(1, t.Count);
            },
            t =>
            {
                Assert.Equal(2, t.CustomerId);
                Assert.Equal(20, t.CustomerMinHourlyRate);
                Assert.Equal(20, t.HourlyRate);
                Assert.Equal(1, t.Count);
            });
    }

    [SkippableTheory(Skip = "Feature 'Dependent aggregate inside subselect' is not supported by SingleStore.")]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Aggregate_over_subquery_in_group_by_projection_2(bool async)
    {
        var contextFactory = await InitializeAsync<Context27094>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context27094.Table>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = from t in context.Tables
            group t.Id by t.Value
            into tg
            select new
            {
                A = tg.Key, B = context.Tables.Where(t => t.Value == tg.Max() * 6).Max(t => (int?)t.Id),
            };

        var orders = async
            ? await query.ToListAsync()
            : query.ToList();
    }

    public override async Task Average_with_cast()
    {
        var contextFactory = await InitializeAsync<Context11885>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context11885.PriceEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var prices = context.Prices.ToList();

        Assert.Equal(prices.Average(e => e.Price), context.Prices.Average(e => e.Price));
        Assert.Equal(prices.Average(e => e.IntColumn), context.Prices.Average(e => e.IntColumn));
        Assert.Equal(prices.Average(e => e.NullableIntColumn), context.Prices.Average(e => e.NullableIntColumn));
        Assert.Equal(prices.Average(e => e.LongColumn), context.Prices.Average(e => e.LongColumn));
        Assert.Equal(prices.Average(e => e.NullableLongColumn), context.Prices.Average(e => e.NullableLongColumn));
        Assert.Equal(prices.Average(e => e.FloatColumn), context.Prices.Average(e => e.FloatColumn));
        Assert.Equal(prices.Average(e => e.NullableFloatColumn), context.Prices.Average(e => e.NullableFloatColumn));
        Assert.Equal(prices.Average(e => e.DoubleColumn), context.Prices.Average(e => e.DoubleColumn));
        Assert.Equal(prices.Average(e => e.NullableDoubleColumn), context.Prices.Average(e => e.NullableDoubleColumn));
        Assert.Equal(prices.Average(e => e.DecimalColumn), context.Prices.Average(e => e.DecimalColumn));
        Assert.Equal(prices.Average(e => e.NullableDecimalColumn), context.Prices.Average(e => e.NullableDecimalColumn));
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Bool_discriminator_column_works(bool async)
    {
        var contextFactory = await InitializeAsync<Context24657>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context24657.Author>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context24657.Blog>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = context.Authors.Include(e => e.Blog);

        var authors = async
            ? await query.ToListAsync()
            : query.ToList();

        Assert.Equal(2, authors.Count);
    }

    [ConditionalFact]
    public override async Task Conditional_expression_with_conditions_does_not_collapse_if_nullable_bool()
    {
        var contextFactory = await InitializeAsync<Context9468>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context9468.Cart>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context9468.Configuration>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var query = context.Carts.Select(
            t => new { Processing = t.Configuration != null ? !t.Configuration.Processed : (bool?)null }).ToList();

        Assert.Single(query.Where(t => t.Processing == null));
        Assert.Single(query.Where(t => t.Processing == true));
        Assert.Single(query.Where(t => t.Processing == false));
    }

    [ConditionalFact]
    public override async Task Discriminator_type_is_handled_correctly()
    {
        var contextFactory = await InitializeAsync<Context7359>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context7359.Product>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var ctx = contextFactory.CreateContext())
        {
            var query = ctx.Products.OfType<Context7359.SpecialProduct>().ToList();

            Assert.Single(query);
        }

        using (var ctx = contextFactory.CreateContext())
        {
            var query = ctx.Products.Where(p => p is Context7359.SpecialProduct).ToList();

            Assert.Single(query);
        }
    }

    [ConditionalFact]
    public override async Task Enum_has_flag_applies_explicit_cast_for_constant()
    {
        var contextFactory = await InitializeAsync<Context8538>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context8538.Entity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Entities.Where(e => e.Permission.HasFlag(Context8538.Permission.READ_WRITE)).ToList();
            Assert.Single(query);
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Entities.Where(e => e.PermissionShort.HasFlag(Context8538.PermissionShort.READ_WRITE)).ToList();
            Assert.Single(query);
        }
    }

    [ConditionalFact]
    public override async Task Enum_has_flag_does_not_apply_explicit_cast_for_non_constant()
    {
        var contextFactory = await InitializeAsync<Context8538>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context8538.Entity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Entities.Where(e => e.Permission.HasFlag(e.Permission)).ToList();
            Assert.Equal(3, query.Count);
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Entities.Where(e => e.PermissionByte.HasFlag(e.PermissionByte)).ToList();
            Assert.Equal(3, query.Count);
        }
    }

    [ConditionalFact]
    public override async Task First_FirstOrDefault_ix_async()
    {
        var contextFactory = await InitializeAsync<Context603>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context603.Product>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using (var context = contextFactory.CreateContext())
        {
            var product = await context.Products.OrderBy(p => p.Id).FirstAsync();
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        using (var context = contextFactory.CreateContext())
        {
            context.Products.Add(new Context603.Product { Name = "Product 1" });
            await context.SaveChangesAsync();
        }

        using (var context = contextFactory.CreateContext())
        {
            var product = await context.Products.OrderBy(p => p.Id).FirstOrDefaultAsync();
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }

    [SkippableTheory(Skip = "Feature 'Dependent aggregate inside subselect' is not supported by SingleStore.")]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Group_by_aggregate_in_subquery_projection_after_group_by(bool async)
    {
        var contextFactory = await InitializeAsync<Context27094>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context27094.Table>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = from t in context.Tables
            group t.Id by t.Value
            into tg
            select new
            {
                A = tg.Key,
                B = tg.Sum(),
                C = (from t in context.Tables
                        group t.Id by t.Value
                        into tg2
                        select tg.Sum() + tg2.Sum()
                    ).OrderBy(e => 1).FirstOrDefault()
            };

        var orders = async
            ? await query.ToListAsync()
            : query.ToList();
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task GroupBy_Aggregate_over_navigations_repeated(bool async)
    {
        var contextFactory = await InitializeAsync<Context27083>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context27083.Customer>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context27083.Order>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context27083.Project>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context27083.TimeSheet>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = context
            .Set<Context27083.TimeSheet>()
            .Where(x => x.OrderId != null)
            .GroupBy(x => x.OrderId)
            .Select(
                x => new
                {
                    HourlyRate = x.Min(f => f.Order.HourlyRate),
                    CustomerId = x.Min(f => f.Project.Customer.Id),
                    CustomerName = x.Min(f => f.Project.Customer.Name),
                });

        var timeSheets = async
            ? await query.ToListAsync()
            : query.ToList();

        Assert.Equal(2, timeSheets.Count);
    }

    [ConditionalFact]
    public override async Task New_instances_in_projection_are_not_shared_across_results()
    {
        var contextFactory = await InitializeAsync<Context7983>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context7983.Blog>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context7983.Post>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var list = context.Posts.Select(p => new Context7983.PostDTO().From(p)).ToList();

        Assert.Equal(3, list.Count);
        Assert.Equal(new[] { "First", "Second", "Third" }, list.OrderBy(dto => dto.Title).Select(dto => dto.Title));
    }

    [ConditionalFact]
    public override async Task Operators_combine_nullability_of_entity_shapers()
    {
        var contextFactory = await InitializeAsync<Context19253>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context19253.A>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context19253.B>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            Expression<Func<Context19253.A, string>> leftKeySelector = x => x.forkey;
            Expression<Func<Context19253.B, string>> rightKeySelector = y => y.forkey;

            var query = context.As.GroupJoin(
                    context.Bs,
                    leftKeySelector,
                    rightKeySelector,
                    (left, rightg) => new { left, rightg })
                .SelectMany(
                    r => r.rightg.DefaultIfEmpty(),
                    (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = x.left, Right = y })
                .Concat(
                    context.Bs.GroupJoin(
                            context.As,
                            rightKeySelector,
                            leftKeySelector,
                            (right, leftg) => new { leftg, right })
                        .SelectMany(
                            l => l.leftg.DefaultIfEmpty(),
                            (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = y, Right = x.right })
                        .Where(z => z.Left.Equals(null)))
                .ToList();

            Assert.Equal(3, query.Count);
        }

        using (var context = contextFactory.CreateContext())
        {
            Expression<Func<Context19253.A, string>> leftKeySelector = x => x.forkey;
            Expression<Func<Context19253.B, string>> rightKeySelector = y => y.forkey;

            var query = context.As.GroupJoin(
                    context.Bs,
                    leftKeySelector,
                    rightKeySelector,
                    (left, rightg) => new { left, rightg })
                .SelectMany(
                    r => r.rightg.DefaultIfEmpty(),
                    (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = x.left, Right = y })
                .Union(
                    context.Bs.GroupJoin(
                            context.As,
                            rightKeySelector,
                            leftKeySelector,
                            (right, leftg) => new { leftg, right })
                        .SelectMany(
                            l => l.leftg.DefaultIfEmpty(),
                            (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = y, Right = x.right })
                        .Where(z => z.Left.Equals(null)))
                .ToList();

            Assert.Equal(3, query.Count);
        }

        using (var context = contextFactory.CreateContext())
        {
            Expression<Func<Context19253.A, string>> leftKeySelector = x => x.forkey;
            Expression<Func<Context19253.B, string>> rightKeySelector = y => y.forkey;

            var query = context.As.GroupJoin(
                    context.Bs,
                    leftKeySelector,
                    rightKeySelector,
                    (left, rightg) => new { left, rightg })
                .SelectMany(
                    r => r.rightg.DefaultIfEmpty(),
                    (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = x.left, Right = y })
                .Except(
                    context.Bs.GroupJoin(
                            context.As,
                            rightKeySelector,
                            leftKeySelector,
                            (right, leftg) => new { leftg, right })
                        .SelectMany(
                            l => l.leftg.DefaultIfEmpty(),
                            (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = y, Right = x.right }))
                .ToList();

            Assert.Single(query);
        }

        using (var context = contextFactory.CreateContext())
        {
            Expression<Func<Context19253.A, string>> leftKeySelector = x => x.forkey;
            Expression<Func<Context19253.B, string>> rightKeySelector = y => y.forkey;

            var query = context.As.GroupJoin(
                    context.Bs,
                    leftKeySelector,
                    rightKeySelector,
                    (left, rightg) => new { left, rightg })
                .SelectMany(
                    r => r.rightg.DefaultIfEmpty(),
                    (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = x.left, Right = y })
                .Intersect(
                    context.Bs.GroupJoin(
                            context.As,
                            rightKeySelector,
                            leftKeySelector,
                            (right, leftg) => new { leftg, right })
                        .SelectMany(
                            l => l.leftg.DefaultIfEmpty(),
                            (x, y) => new Context19253.JoinResult<Context19253.A, Context19253.B> { Left = y, Right = x.right }))
                .ToList();

            Assert.Single(query);
        }
    }

    [ConditionalFact]
    public override async Task Parameterless_ctor_on_inner_DTO_gets_called_for_every_row()
    {
        var contextFactory = await InitializeAsync<Context12274>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context12274.MyEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var results = context.Entities.Select(
            x =>
                new Context12274.OuterDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Inner = new Context12274.InnerDTO()
                }).ToList();
        Assert.Equal(4, results.Count);
        Assert.False(ReferenceEquals(results[0].Inner, results[1].Inner));
        Assert.False(ReferenceEquals(results[1].Inner, results[2].Inner));
        Assert.False(ReferenceEquals(results[2].Inner, results[3].Inner));
    }

    [ConditionalFact]
    public override async Task QueryBuffer_requirement_is_computed_when_querying_base_type_while_derived_type_has_shadow_prop()
    {
        var contextFactory = await InitializeAsync<Context11104>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context11104.Base>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context11104.Stuff>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var query = context.Bases.ToList();

        var derived1 = Assert.Single(query);
        Assert.Equal(typeof(Context11104.Derived1), derived1.GetType());
    }

    [ConditionalFact]
    public override async Task Repeated_parameters_in_generated_query_sql()
    {
        var contextFactory = await InitializeAsync<Context15215>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context15215.Auto>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context15215.EqualAuto>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var k = 1;
        var a = context.Autos.Where(e => e.Id == k).First();
        var b = context.Autos.Where(e => e.Id == k + 1).First();

        var equalQuery = (from d in context.EqualAutos
            where (d.Auto == a && d.AnotherAuto == b)
                  || (d.Auto == b && d.AnotherAuto == a)
            select d).ToList();

        Assert.Single(equalQuery);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task SelectMany_where_Select(bool async)
    {
        var contextFactory = await InitializeAsync<Context26744>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context26744.Parent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26744.Child>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = context.Parents
            .SelectMany(
                p => p.Children
                    .Where(c => c.SomeNullableDateTime == null)
                    .OrderBy(c => c.SomeInteger)
                    .Take(1))
            .Where(c => c.SomeOtherNullableDateTime != null)
            .Select(c => c.SomeNullableDateTime);

        var result = async
            ? await query.ToListAsync()
            : query.ToList();

        Assert.Single(result);
    }

    [ConditionalFact]
    public override async Task Shadow_property_with_inheritance()
    {
        var contextFactory = await InitializeAsync<Context6986>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context6986.Contact>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context6986.Employer>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context6986.ServiceOperator>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            // can_query_base_type_when_derived_types_contain_shadow_properties
            var query = context.Contacts.ToList();

            Assert.Equal(4, query.Count);
            Assert.Equal(2, query.OfType<Context6986.EmployerContact>().Count());
            Assert.Single(query.OfType<Context6986.ServiceOperatorContact>());
        }

        using (var context = contextFactory.CreateContext())
        {
            // can_include_dependent_to_principal_navigation_of_derived_type_with_shadow_fk
            var query = context.Contacts.OfType<Context6986.ServiceOperatorContact>().Include(e => e.ServiceOperator)
                .ToList();

            Assert.Single(query);
            Assert.NotNull(query[0].ServiceOperator);
        }

        using (var context = contextFactory.CreateContext())
        {
            // can_project_shadow_property_using_ef_property
            var query = context.Contacts.OfType<Context6986.ServiceOperatorContact>().Select(
                c => new { c, Prop = EF.Property<int>(c, "ServiceOperatorId") }).ToList();

            Assert.Single(query);
            Assert.Equal(1, query[0].Prop);
        }
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Subquery_first_member_compared_to_null(bool async)
    {
        var contextFactory = await InitializeAsync<Context26744>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context26744.Parent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26744.Child>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = context.Parents
            .Where(
                p => p.Children.Any(c => c.SomeNullableDateTime == null)
                     && p.Children.Where(c => c.SomeNullableDateTime == null)
                         .OrderBy(c => c.SomeInteger)
                         .First().SomeOtherNullableDateTime
                     != null)
            .Select(
                p => p.Children.Where(c => c.SomeNullableDateTime == null)
                    .OrderBy(c => c.SomeInteger)
                    .First().SomeOtherNullableDateTime);

        var result = async
            ? await query.ToListAsync()
            : query.ToList();

        Assert.Single(result);
    }

    [ConditionalFact]
    public override async Task Union_and_insert_works_correctly_together()
    {
        var contextFactory = await InitializeAsync<Context12549>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context12549.Table1>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context12549.Table2>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var id1 = 1;
            var id2 = 2;

            var ids1 = context.Set<Context12549.Table1>()
                .Where(x => x.Id == id1)
                .Select(x => x.Id);

            var ids2 = context.Set<Context12549.Table2>()
                .Where(x => x.Id == id2)
                .Select(x => x.Id);

            var results = ids1.Union(ids2).ToList();

            context.AddRange(
                new Context12549.Table1(),
                new Context12549.Table2(),
                new Context12549.Table1(),
                new Context12549.Table2());

            await context.SaveChangesAsync();
        }
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Unwrap_convert_node_over_projection_when_translating_contains_over_subquery(bool async)
    {
        var contextFactory = await InitializeAsync<Context26593>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context26593.User>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26593.Group>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26593.Membership>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var currentUserId = 1;

        var currentUserGroupIds = context.Memberships
            .Where(m => m.UserId == currentUserId)
            .Select(m => m.GroupId);

        var hasMembership = context.Memberships
            .Where(m => currentUserGroupIds.Contains(m.GroupId))
            .Select(m => m.User);

        var query = context.Users
            .Select(u => new { HasAccess = hasMembership.Contains(u) });

        var users = async
            ? await query.ToListAsync()
            : query.ToList();
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Unwrap_convert_node_over_projection_when_translating_contains_over_subquery_2(bool async)
    {
        var contextFactory = await InitializeAsync<Context26593>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context26593.User>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26593.Group>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26593.Membership>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var currentUserId = 1;

        var currentUserGroupIds = context.Memberships
            .Where(m => m.UserId == currentUserId)
            .Select(m => m.Group);

        var hasMembership = context.Memberships
            .Where(m => currentUserGroupIds.Contains(m.Group))
            .Select(m => m.User);

        var query = context.Users
            .Select(u => new { HasAccess = hasMembership.Contains(u) });

        var users = async
            ? await query.ToListAsync()
            : query.ToList();
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Unwrap_convert_node_over_projection_when_translating_contains_over_subquery_3(bool async)
    {
        var contextFactory = await InitializeAsync<Context26593>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context26593.User>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26593.Group>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26593.Membership>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var currentUserId = 1;

        var currentUserGroupIds = context.Memberships
            .Where(m => m.UserId == currentUserId)
            .Select(m => m.GroupId);

        var hasMembership = context.Memberships
            .Where(m => currentUserGroupIds.Contains(m.GroupId))
            .Select(m => m.User);

        var query = context.Users
            .Select(u => new { HasAccess = hasMembership.Any(e => e == u) });

        var users = async
            ? await query.ToListAsync()
            : query.ToList();
    }

    public override async Task Multiple_different_entity_type_from_different_namespaces(bool async)
    {
        // The only change is the FromSqlRaw SQL string:
        //     Original: SELECT cast(null as int) AS MyValue
        //     Changed:  SELECT cast(null as signed) AS MyValue
        // The other comments are part of the base implementation.

        var contextFactory = await InitializeAsync<Context23981>();
        using var context = contextFactory.CreateContext();
        //var good1 = context.Set<NameSpace1.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        //var good2 = context.Set<NameSpace2.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        var bad = context.Set<TestQuery>().FromSqlRaw(@"SELECT cast(null as signed) AS MyValue").ToList(); // Exception
    }
}
