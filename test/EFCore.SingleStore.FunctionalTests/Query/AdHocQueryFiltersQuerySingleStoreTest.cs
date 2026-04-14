using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocQueryFiltersQuerySingleStoreTest : AdHocQueryFiltersQueryRelationalTestBase
{
    [ConditionalFact]
    public override async Task Query_filter_with_contains_evaluates_correctly()
    {
        var contextFactory = await InitializeAsync<Context10295>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context10295.MyEntity10295>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        var result = context.Entities.ToList();
        Assert.Single(result);
    }

    [ConditionalFact]
    public override async Task MultiContext_query_filter_test()
    {
        var contextFactory = await InitializeAsync<FilterContext10301>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<FilterContextBase10301.Blog10301>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            Assert.Empty(context.Blogs.ToList());

            context.Tenant = 1;
            Assert.Single(context.Blogs.ToList());

            context.Tenant = 2;
            Assert.Equal(2, context.Blogs.Count());
        }
    }

    [ConditionalFact]
    public override async Task Query_filter_with_pk_fk_optimization()
    {
        var contextFactory = await InitializeAsync<Context13517>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context13517.Entity13517>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context13517.RefEntity13517>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();

        var result = context.Entities
            .Select(
                s => new Context13517.EntityDto13517
                {
                    Id = s.Id,
                    RefEntity = s.RefEntity == null
                        ? null
                        : new Context13517.RefEntityDto13517
                        {
                            Id = s.RefEntity.Id,
                            Public = s.RefEntity.Public
                        },
                    RefEntityId = s.RefEntityId
                })
            .Single();

        Assert.NotNull(result);
        Assert.Null(result.RefEntity);
        Assert.NotNull(result.RefEntityId);
    }

    [ConditionalFact]
    public override async Task Self_reference_in_query_filter_works()
    {
        var contextFactory = await InitializeAsync<Context17253>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context17253.EntityWithQueryFilterSelfReference17253>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context17253.EntityReferencingEntityWithQueryFilterSelfReference17253>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context17253.EntityWithQueryFilterCycle17253_1>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context17253.EntityWithQueryFilterCycle17253_2>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context17253.EntityWithQueryFilterCycle17253_3>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        using (var context = contextFactory.CreateContext())
        {
            var query = context.EntitiesWithQueryFilterSelfReference.Where(e => e.Name != "Foo");
            var result = query.ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.EntitiesReferencingEntityWithQueryFilterSelfReference.Where(e => e.Name != "Foo");
            var result = query.ToList();
        }
    }

    [ConditionalFact]
    public override async Task Invoke_inside_query_filter_gets_correctly_evaluated_during_translation()
    {
        var contextFactory = await InitializeAsync<Context18510>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context18510.MyEntity18510>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        context.TenantId = 1;

        var query1 = context.Entities.ToList();
        Assert.True(query1.All(x => x.TenantId == 1));

        context.TenantId = 2;
        var query2 = context.Entities.ToList();
        Assert.True(query2.All(x => x.TenantId == 2));
    }

    [ConditionalFact]
    public override async Task GroupJoin_SelectMany_gets_flattened()
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var contextFactory = await InitializeAsync<Context19708>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context19708.Customer19708>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context19708.CustomerMembership19708>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using (var context = contextFactory.CreateContext())
        {
            var query = context.CustomerFilters.ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Set<Context19708.CustomerView19708>()
                .OrderBy(e => e.Id)
                .ThenBy(e => e.CustomerMembershipId)
                .ToList();

            Assert.Collection(
                query,
                t => AssertCustomerView(t, 1, "First", 1, "FirstChild"),
                t => AssertCustomerView(t, 2, "Second", 2, "SecondChild1"),
                t => AssertCustomerView(t, 2, "Second", 3, "SecondChild2"),
                t => AssertCustomerView(t, 3, "Third", null, ""));

            static void AssertCustomerView(
                Context19708.CustomerView19708 actual,
                int id,
                string name,
                int? customerMembershipId,
                string customerMembershipName)
            {
                Assert.Equal(id, actual.Id);
                Assert.Equal(name, actual.Name);
                Assert.Equal(customerMembershipId, actual.CustomerMembershipId);
                Assert.Equal(customerMembershipName, actual.CustomerMembershipName);
            }
        }
    }


    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Group_by_multiple_aggregate_joining_different_tables(bool async)
    {
        var contextFactory = await InitializeAsync<Context27163>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Parent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Child1>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Child2>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        var supportsThisQueryShape =
            AppConfig.ServerVersion.Supports.Version(ServerVersion.Parse("9.0"));

        if (!supportsThisQueryShape)
        {
            using var context = contextFactory.CreateContext();
            await Assert.ThrowsAsync<SingleStoreException>(() => async
                ? context.Parents
                    .GroupBy(x => new { })
                    .Select(
                        g => new
                        {
                            Test1 = g
                                .Select(x => x.Child1.Value1)
                                .Distinct()
                                .Count(),
                            Test2 = g
                                .Select(x => x.Child2.Value2)
                                .Distinct()
                                .Count()
                        }).ToListAsync()
                : Task.FromResult(context.Parents
                    .GroupBy(x => new { })
                    .Select(
                        g => new
                        {
                            Test1 = g
                                .Select(x => x.Child1.Value1)
                                .Distinct()
                                .Count(),
                            Test2 = g
                                .Select(x => x.Child2.Value2)
                                .Distinct()
                                .Count()
                        }).ToList()));
            return;
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Parents
                .GroupBy(x => new { })
                .Select(
                    g => new
                    {
                        Test1 = g
                            .Select(x => x.Child1.Value1)
                            .Distinct()
                            .Count(),
                        Test2 = g
                            .Select(x => x.Child2.Value2)
                            .Distinct()
                            .Count()
                    });

            var orders = async
                ? await query.ToListAsync()
                : query.ToList();
        }
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Group_by_multiple_aggregate_joining_different_tables_with_query_filter(bool async)
    {
        var contextFactory = await InitializeAsync<Context27163>(onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Parent>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<ChildFilter1>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<ChildFilter2>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        var supportsThisQueryShape =
            AppConfig.ServerVersion.Supports.Version(ServerVersion.Parse("9.0"));

        if (!supportsThisQueryShape)
        {
            using var context = contextFactory.CreateContext();
            await Assert.ThrowsAsync<SingleStoreException>(() => async
                ? context.Parents
                    .GroupBy(x => new { })
                    .Select(
                        g => new
                        {
                            Test1 = g
                                .Select(x => x.ChildFilter1.Value1)
                                .Distinct()
                                .Count(),
                            Test2 = g
                                .Select(x => x.ChildFilter2.Value2)
                                .Distinct()
                                .Count()
                        }).ToListAsync()
                : Task.FromResult(context.Parents
                    .GroupBy(x => new { })
                    .Select(
                        g => new
                        {
                            Test1 = g
                                .Select(x => x.ChildFilter1.Value1)
                                .Distinct()
                                .Count(),
                            Test2 = g
                                .Select(x => x.ChildFilter2.Value2)
                                .Distinct()
                                .Count()
                        }).ToList()));
            return;
        }

        using (var context = contextFactory.CreateContext())
        {
            var query = context.Parents
                .GroupBy(x => new { })
                .Select(
                    g => new
                    {
                        Test1 = g
                            .Select(x => x.ChildFilter1.Value1)
                            .Distinct()
                            .Count(),
                        Test2 = g
                            .Select(x => x.ChildFilter2.Value2)
                            .Distinct()
                            .Count()
                    });

            var orders = async
                ? await query.ToListAsync()
                : query.ToList();
        }
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}

