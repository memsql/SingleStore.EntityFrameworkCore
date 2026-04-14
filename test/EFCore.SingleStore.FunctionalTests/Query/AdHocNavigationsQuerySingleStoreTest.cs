using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocNavigationsQuerySingleStoreTest : AdHocNavigationsQueryRelationalTestBase
{
    [ConditionalFact]
    public override async Task ThenInclude_with_interface_navigations()
    {
        var contextFactory = await InitializeAsync<Context3409>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context3409.Parent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context3409.Child>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var results = context.Parents
                .Include(p => p.ChildCollection)
                .ThenInclude(c => c.SelfReferenceCollection)
                .ToList();

            Assert.Single(results);
            Assert.Equal(1, results[0].ChildCollection.Count);
            Assert.Equal(2, results[0].ChildCollection.Single().SelfReferenceCollection.Count);
        }

        using (var context = contextFactory.CreateContext())
        {
            var results = context.Children
                .Select(
                    c => new { c.SelfReferenceBackNavigation, c.SelfReferenceBackNavigation.ParentBackNavigation })
                .ToList();

            Assert.Equal(3, results.Count);
            Assert.Equal(2, results.Count(c => c.SelfReferenceBackNavigation != null));
            Assert.Equal(2, results.Count(c => c.ParentBackNavigation != null));
        }

        using (var context = contextFactory.CreateContext())
        {
            var results = context.Children
                .Select(
                    c => new
                    {
                        SelfReferenceBackNavigation
                            = EF.Property<Context3409.IChild>(c, "SelfReferenceBackNavigation"),
                        ParentBackNavigationB
                            = EF.Property<Context3409.IParent>(
                                EF.Property<Context3409.IChild>(c, "SelfReferenceBackNavigation"),
                                "ParentBackNavigation")
                    })
                .ToList();

            Assert.Equal(3, results.Count);
            Assert.Equal(2, results.Count(c => c.SelfReferenceBackNavigation != null));
            Assert.Equal(2, results.Count(c => c.ParentBackNavigationB != null));
        }

        using (var context = contextFactory.CreateContext())
        {
            var results = context.Children
                .Include(c => c.SelfReferenceBackNavigation)
                .ThenInclude(c => c.ParentBackNavigation)
                .ToList();

            Assert.Equal(3, results.Count);
            Assert.Equal(2, results.Count(c => c.SelfReferenceBackNavigation != null));
            Assert.Equal(1, results.Count(c => c.ParentBackNavigation != null));
        }
    }

    [ConditionalFact]
    public override async Task Customer_collections_materialize_properly()
    {
        var contextFactory = await InitializeAsync<Context3758>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context3758.Customer>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context3758.Order>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using var ctx = contextFactory.CreateContext();

        var query1 = ctx.Customers.Select(c => c.Orders1);
        var result1 = query1.ToList();

        Assert.Equal(2, result1.Count);
        Assert.IsType<HashSet<Context3758.Order>>(result1[0]);
        Assert.Equal(2, result1[0].Count);
        Assert.Equal(2, result1[1].Count);

        var query2 = ctx.Customers.Select(c => c.Orders2);
        var result2 = query2.ToList();

        Assert.Equal(2, result2.Count);
        Assert.IsType<Context3758.MyGenericCollection<Context3758.Order>>(result2[0]);
        Assert.Equal(2, result2[0].Count);
        Assert.Equal(2, result2[1].Count);

        var query3 = ctx.Customers.Select(c => c.Orders3);
        var result3 = query3.ToList();

        Assert.Equal(2, result3.Count);
        Assert.IsType<Context3758.MyNonGenericCollection>(result3[0]);
        Assert.Equal(2, result3[0].Count);
        Assert.Equal(2, result3[1].Count);

        var query4 = ctx.Customers.Select(c => c.Orders4);

        Assert.Equal(
            CoreStrings.NavigationCannotCreateType(
                "Orders4", typeof(Context3758.Customer).Name,
                typeof(Context3758.MyInvalidCollection<Context3758.Order>).ShortDisplayName()),
            Assert.Throws<InvalidOperationException>(() => query4.ToList()).Message);
    }

    [ConditionalFact]
    public override async Task Reference_include_on_derived_type_with_sibling_works()
    {
        var contextFactory = await InitializeAsync<Context7312>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context7312.Proposal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context7312.ProposalLeaveType>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Proposals.OfType<Context7312.ProposalLeave>().Include(l => l.LeaveType).ToList();

            Assert.Single(query);
        }
    }

    [ConditionalFact]
    public override async Task Include_collection_optional_reference_collection()
    {
        var contextFactory = await InitializeAsync<Context9038>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context9038.Person9038>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context9038.PersonFamily9038>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var result = await context.People.OfType<Context9038.PersonTeacher9038>()
                .Include(m => m.Students)
                .ThenInclude(m => m.Family)
                .ThenInclude(m => m.Members)
                .ToListAsync();

            Assert.Equal(2, result.Count);
            Assert.True(result.All(r => r.Students.Count > 0));
        }

        using (var context = contextFactory.CreateContext())
        {
            var result = await context.Set<Context9038.PersonTeacher9038>()
                .Include(m => m.Family.Members)
                .Include(m => m.Students)
                .ToListAsync();

            Assert.Equal(2, result.Count);
            Assert.True(result.All(r => r.Students.Count > 0));
            Assert.Null(result.Single(t => t.Name == "Ms. Frizzle").Family);
            Assert.NotNull(result.Single(t => t.Name == "Mr. Garrison").Family);
        }
    }

    [ConditionalFact]
    public override async Task Include_with_order_by_on_interface_key()
    {
        var contextFactory = await InitializeAsync<Context10635>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context10635.Parent10635>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context10635.Child10635>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using (var context = contextFactory.CreateContext())
        {
            var query = context.Parents.Include(p => p.Children).OrderBy(p => p.Id).ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Parents.OrderBy(p => p.Id).Select(p => p.Children.ToList()).ToList();
        }
    }

    [ConditionalTheory]
    public override async Task Select_enumerable_navigation_backed_by_collection(bool async, bool split)
    {
        var contextFactory = await InitializeAsync<Context21803>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context21803.AppEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context21803.OtherEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var query = context.Set<Context21803.AppEntity>().Select(appEntity => appEntity.OtherEntities);

        if (split)
        {
            query = query.AsSplitQuery();
        }

        if (async)
        {
            await query.ToListAsync();
        }
        else
        {
            query.ToList();
        }
    }

    [ConditionalFact]
    public override async Task Collection_without_setter_materialized_correctly()
    {
        var contextFactory = await InitializeAsync<Context11923>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context11923.Blog>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context11923.Post>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context11923.Comment>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var query1 = context.Blogs
            .Select(
                b => new
                {
                    Collection1 = b.Posts1,
                    Collection2 = b.Posts2,
                    Collection3 = b.Posts3
                }).ToList();

        var query2 = context.Blogs
            .Select(
                b => new
                {
                    Collection1 = b.Posts1.OrderBy(p => p.Id).First().Comments.Count,
                    Collection2 = b.Posts2.OrderBy(p => p.Id).First().Comments.Count,
                    Collection3 = b.Posts3.OrderBy(p => p.Id).First().Comments.Count
                }).ToList();

        Assert.Throws<InvalidOperationException>(
            () => context.Blogs
                .Select(
                    b => new
                    {
                        Collection1 = b.Posts1.OrderBy(p => p.Id),
                        Collection2 = b.Posts2.OrderBy(p => p.Id),
                        Collection3 = b.Posts3.OrderBy(p => p.Id)
                    }).ToList());
    }

    [ConditionalFact]
    public override async Task Include_collection_works_when_defined_on_intermediate_type()
    {
        var contextFactory = await InitializeAsync<Context11944>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context11944.Student>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context11944.School>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Schools.Include(s => ((Context11944.ElementarySchool)s).Students);
            var result = query.ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(2, result.OfType<Context11944.ElementarySchool>().Single().Students.Count);
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Schools.Select(s => ((Context11944.ElementarySchool)s).Students.Where(ss => true).ToList());
            var result = query.ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Count() == 2);
        }
    }

    [ConditionalFact]
    public override async Task Let_multiple_references_with_reference_to_outer()
    {
        var contextFactory = await InitializeAsync<Context12456>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context12456.Activity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context12456.ActivityType>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context12456.ActivityTypePoints>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context12456.CompetitionSeason>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context12456.Point>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using (var context = contextFactory.CreateContext())
        {
            var users = (from a in context.Activities
                         let cs = context.CompetitionSeasons.First(s => s.StartDate <= a.DateTime && a.DateTime < s.EndDate)
                         select new { cs.Id, Points = a.ActivityType.Points.Where(p => p.CompetitionSeason == cs) }).ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var users = context.Activities
                .Select(
                    a => new
                    {
                        Activity = a,
                        CompetitionSeason = context.CompetitionSeasons
                            .First(s => s.StartDate <= a.DateTime && a.DateTime < s.EndDate)
                    })
                .Select(
                    a => new
                    {
                        a.Activity,
                        CompetitionSeasonId = a.CompetitionSeason.Id,
                        Points = a.Activity.Points
                            ?? a.Activity.ActivityType.Points
                                .Where(p => p.CompetitionSeason == a.CompetitionSeason)
                                .Select(p => p.Points).SingleOrDefault()
                    }).ToList();
        }
    }

    [ConditionalFact]
    public override async Task Include_collection_with_OfType_base()
    {
        var contextFactory = await InitializeAsync<Context12582>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context12582.Employee>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context12582.EmployeeDevice>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Employees
                .Include(i => i.Devices)
                .OfType<Context12582.IEmployee>()
                .ToList();

            Assert.Single(query);

            var employee = (Context12582.Employee)query[0];
            Assert.Equal(2, employee.Devices.Count);
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Employees
                .Select(e => e.Devices.Where(d => d.Device != "foo").Cast<Context12582.IEmployeeDevice>())
                .ToList();

            Assert.Single(query);
            var result = query[0];
            Assert.Equal(2, result.Count());
        }
    }

    [ConditionalFact]
    public override async Task Correlated_collection_correctly_associates_entities_with_byte_array_keys()
    {
        var contextFactory = await InitializeAsync<Context12748>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context12748.Comment>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var query = from blog in context.Blogs
                    select new
                    {
                        blog.Name,
                        Comments = blog.Comments.Select(
                            u => new { u.Id }).ToArray()
                    };
        var result = query.ToList();
        Assert.Single(result[0].Comments);
    }

    [ConditionalFact]
    public override async Task Cycles_in_auto_include()
    {
        var contextFactory = await InitializeAsync<Context22568>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context22568.PrincipalOneToOne>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.PrincipalOneToMany>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.PrincipalManyToMany>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.DependentOneToOne>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.DependentOneToMany>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.DependentManyToMany>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.CycleA>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.CycleB>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context22568.CycleC>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using (var context = contextFactory.CreateContext())
        {
            var principals = context.Set<Context22568.PrincipalOneToOne>().ToList();
            Assert.Single(principals);
            Assert.NotNull(principals[0].Dependent);
            Assert.NotNull(principals[0].Dependent.Principal);

            var dependents = context.Set<Context22568.DependentOneToOne>().ToList();
            Assert.Single(dependents);
            Assert.NotNull(dependents[0].Principal);
            Assert.NotNull(dependents[0].Principal.Dependent);
        }

        using (var context = contextFactory.CreateContext())
        {
            var principals = context.Set<Context22568.PrincipalOneToMany>().ToList();
            Assert.Single(principals);
            Assert.NotNull(principals[0].Dependents);
            Assert.True(principals[0].Dependents.All(e => e.Principal != null));

            var dependents = context.Set<Context22568.DependentOneToMany>().ToList();
            Assert.Equal(2, dependents.Count);
            Assert.True(dependents.All(e => e.Principal != null));
            Assert.True(dependents.All(e => e.Principal.Dependents != null));
            Assert.True(dependents.All(e => e.Principal.Dependents.All(i => i.Principal != null)));
        }

        using (var context = contextFactory.CreateContext())
        {
            Assert.Equal(
                CoreStrings.AutoIncludeNavigationCycle("'PrincipalManyToMany.Dependents', 'DependentManyToMany.Principals'"),
                Assert.Throws<InvalidOperationException>(() => context.Set<Context22568.PrincipalManyToMany>().ToList()).Message);

            Assert.Equal(
                CoreStrings.AutoIncludeNavigationCycle("'DependentManyToMany.Principals', 'PrincipalManyToMany.Dependents'"),
                Assert.Throws<InvalidOperationException>(() => context.Set<Context22568.DependentManyToMany>().ToList()).Message);

            context.Set<Context22568.PrincipalManyToMany>().IgnoreAutoIncludes().ToList();
            context.Set<Context22568.DependentManyToMany>().IgnoreAutoIncludes().ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            Assert.Equal(
                CoreStrings.AutoIncludeNavigationCycle("'CycleA.Bs', 'CycleB.C', 'CycleC.As'"),
                Assert.Throws<InvalidOperationException>(() => context.Set<Context22568.CycleA>().ToList()).Message);

            Assert.Equal(
                CoreStrings.AutoIncludeNavigationCycle("'CycleB.C', 'CycleC.As', 'CycleA.Bs'"),
                Assert.Throws<InvalidOperationException>(() => context.Set<Context22568.CycleB>().ToList()).Message);

            Assert.Equal(
                CoreStrings.AutoIncludeNavigationCycle("'CycleC.As', 'CycleA.Bs', 'CycleB.C'"),
                Assert.Throws<InvalidOperationException>(() => context.Set<Context22568.CycleC>().ToList()).Message);

            context.Set<Context22568.CycleA>().IgnoreAutoIncludes().ToList();
            context.Set<Context22568.CycleB>().IgnoreAutoIncludes().ToList();
            context.Set<Context22568.CycleC>().IgnoreAutoIncludes().ToList();
        }
    }

    [ConditionalFact]
    public override async Task Can_ignore_invalid_include_path_error()
    {
        var contextFactory = await InitializeAsync<Context20609>(
            onConfiguring: o => o.ConfigureWarnings(x => x.Ignore(CoreEventId.InvalidIncludePathError)),
            onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context20609.SubA>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context20609.SubB>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

        using var context = contextFactory.CreateContext();
        var result = context.Set<Context20609.ClassA>().Include("SubB").ToList();
    }

    [ConditionalFact]
    public override async Task SelectMany_and_collection_in_projection_in_FirstOrDefault()
    {
        var contextFactory = await InitializeAsync<Context20813>(onModelCreating: modelBuilder =>
        {
            // Nothing to override - uses Guid IDs
        });

        using var context = contextFactory.CreateContext();
        var referenceId = "a";
        var customerId = new Guid("1115c816-6c4c-4016-94df-d8b60a22ffa1");
        var query = context.Orders
            .Where(o => o.ExternalReferenceId == referenceId && o.CustomerId == customerId)
            .Select(
                o => new
                {
                    IdentityDocuments = o.IdentityDocuments.Select(
                        id => new
                        {
                            Images = o.IdentityDocuments
                                .SelectMany(id => id.Images)
                                .Select(i => new { i.Image }),
                        })
                }).SingleOrDefault();
    }

    [ConditionalFact]
    public override async Task Using_explicit_interface_implementation_as_navigation_works()
    {
        var contextFactory = await InitializeAsync<Context21768>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context21768.Book>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context21768.BookCover>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context21768.CoverIllustration>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        Expression<Func<Context21768.IBook, Context21768.BookViewModel>> projection =
            b => new Context21768.BookViewModel
            {
                FirstPage = b.FrontCover.Illustrations.FirstOrDefault(
                        i => i.State >= Context21768.IllustrationState.Approved)
                    != null
                        ? new Context21768.PageViewModel
                        {
                            Uri = b.FrontCover.Illustrations
                                .FirstOrDefault(i => i.State >= Context21768.IllustrationState.Approved).Uri
                        }
                        : null,
            };

        var result = context.Books.Where(b => b.Id == 1).Select(projection).SingleOrDefault();
    }

    [ConditionalFact]
    public override async Task Walking_back_include_tree_is_not_allowed_1()
    {
        var contextFactory = await InitializeAsync<Context23674>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context23674.Principal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.ManyDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.SingleDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Set<Context23674.Principal>()
                .Include(p => p.ManyDependents)
                .ThenInclude(m => m.Principal.SingleDependent);

            Assert.Equal(
                CoreStrings.WarningAsErrorTemplate(
                    CoreEventId.NavigationBaseIncludeIgnored.ToString(),
                    CoreResources.LogNavigationBaseIncludeIgnored(new TestLogger<TestLoggingDefinitions>())
                        .GenerateMessage("ManyDependent.Principal"),
                    "CoreEventId.NavigationBaseIncludeIgnored"),
                Assert.Throws<InvalidOperationException>(
                    () => query.ToList()).Message);
        }
    }

    [ConditionalFact]
    public override async Task Walking_back_include_tree_is_not_allowed_2()
    {
        var contextFactory = await InitializeAsync<Context23674>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context23674.Principal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.ManyDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.SingleDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Set<Context23674.Principal>().Include(p => p.SingleDependent.Principal.ManyDependents);

            Assert.Equal(
                CoreStrings.WarningAsErrorTemplate(
                    CoreEventId.NavigationBaseIncludeIgnored.ToString(),
                    CoreResources.LogNavigationBaseIncludeIgnored(new TestLogger<TestLoggingDefinitions>())
                        .GenerateMessage("SingleDependent.Principal"),
                    "CoreEventId.NavigationBaseIncludeIgnored"),
                Assert.Throws<InvalidOperationException>(
                    () => query.ToList()).Message);
        }
    }

    [ConditionalFact]
    public override async Task Walking_back_include_tree_is_not_allowed_3()
    {
        var contextFactory = await InitializeAsync<Context23674>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context23674.Principal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.ManyDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.SingleDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            // This does not warn because after round-tripping from one-to-many from dependent side, the number of dependents could be larger.
            var query = context.Set<Context23674.ManyDependent>()
                .Include(p => p.Principal.ManyDependents)
                .ThenInclude(m => m.SingleDependent)
                .ToList();
        }
    }

    [ConditionalFact]
    public override async Task Walking_back_include_tree_is_not_allowed_4()
    {
        var contextFactory = await InitializeAsync<Context23674>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context23674.Principal>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.ManyDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23674.SingleDependent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Set<Context23674.SingleDependent>().Include(p => p.ManyDependent.SingleDependent.Principal);

            Assert.Equal(
                CoreStrings.WarningAsErrorTemplate(
                    CoreEventId.NavigationBaseIncludeIgnored.ToString(),
                    CoreResources.LogNavigationBaseIncludeIgnored(new TestLogger<TestLoggingDefinitions>())
                        .GenerateMessage("ManyDependent.SingleDependent"),
                    "CoreEventId.NavigationBaseIncludeIgnored"),
                Assert.Throws<InvalidOperationException>(
                    () => query.ToList()).Message);
        }
    }

    [ConditionalFact]
    public override async Task Projection_with_multiple_includes_and_subquery_with_set_operation()
    {
        var contextFactory = await InitializeAsync<Context23676>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context23676.PersonEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23676.PersonImageEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23676.ActorEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23676.MovieActorEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23676.DirectorEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23676.MovieDirectorEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context23676.MovieEntity>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using var context = contextFactory.CreateContext();
        var id = 1;
        var person = await context.Persons
            .Include(p => p.Images)
            .Include(p => p.Actor)
            .ThenInclude(a => a.Movies)
            .ThenInclude(p => p.Movie)
            .Include(p => p.Director)
            .ThenInclude(a => a.Movies)
            .ThenInclude(p => p.Movie)
            .Select(
                x => new
                {
                    x.Id,
                    x.Name,
                    x.Surname,
                    x.Birthday,
                    x.Hometown,
                    x.Bio,
                    x.AvatarUrl,
                    Images = x.Images
                        .Select(
                            i => new
                            {
                                i.Id,
                                i.ImageUrl,
                                i.Height,
                                i.Width
                            }).ToList(),
                    KnownByFilms = x.Actor.Movies
                        .Select(m => m.Movie)
                        .Union(
                            x.Director.Movies
                                .Select(m => m.Movie))
                        .Select(
                            m => new
                            {
                                m.Id,
                                m.Name,
                                m.PosterUrl,
                                m.Rating
                            }).ToList()
                })
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Count_member_over_IReadOnlyCollection_works(bool async)
    {
        var contextFactory = await InitializeAsync<Context26433>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context26433.Author>()
                .Property(e => e.AuthorId)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context26433.Book>()
                .Property(e => e.BookId)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var query = context.Authors
            .Select(a => new { BooksCount = a.Books.Count });

        var authors = async
            ? await query.ToListAsync()
            : query.ToList();

        Assert.Equal(3, Assert.Single(authors).BooksCount);
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
