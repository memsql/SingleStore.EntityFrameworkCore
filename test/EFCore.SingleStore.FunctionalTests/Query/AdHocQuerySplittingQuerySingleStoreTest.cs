using System;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class AdHocQuerySplittingQuerySingleStoreTest : AdHocQuerySplittingQueryTestBase
{
    [ConditionalFact]
    public override async Task Can_configure_SingleQuery_at_context_level()
    {
        var contextFactory = await InitializeAsync<Context21355>(
            seed: c => c.SeedAsync(),
            onConfiguring: o => SetQuerySplittingBehavior(o, QuerySplittingBehavior.SingleQuery),
            onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context21355.Child>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context21355.AnotherChild>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

        using (var context = contextFactory.CreateContext())
        {
            var result = context.Parents.Include(p => p.Children1).ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var result = context.Parents.Include(p => p.Children1).AsSplitQuery().ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            context.Parents.Include(p => p.Children1).Include(p => p.Children2).ToList();
        }
    }

    [ConditionalFact]
    public override async Task Can_configure_SplitQuery_at_context_level()
    {
        var contextFactory = await InitializeAsync<Context21355>(
            seed: c => c.SeedAsync(),
            onConfiguring: o => SetQuerySplittingBehavior(o, QuerySplittingBehavior.SplitQuery),
            onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context21355.Child>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context21355.AnotherChild>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

        using (var context = contextFactory.CreateContext())
        {
            var result = context.Parents.Include(p => p.Children1).ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            var result = context.Parents.Include(p => p.Children1).AsSingleQuery().ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            context.Parents.Include(p => p.Children1).Include(p => p.Children2).ToList();
        }
    }

    [ConditionalFact]
    public override async Task Unconfigured_query_splitting_behavior_throws_a_warning()
    {
        var contextFactory = await InitializeAsync<Context21355>(
            seed: c => c.SeedAsync(),
            onConfiguring: o => ClearQuerySplittingBehavior(o),
            onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context21355.Child>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Context21355.AnotherChild>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

        using (var context = contextFactory.CreateContext())
        {
            context.Parents.Include(p => p.Children1).Include(p => p.Children2).AsSplitQuery().ToList();
        }

        using (var context = contextFactory.CreateContext())
        {
            Assert.Contains(
                RelationalResources.LogMultipleCollectionIncludeWarning(new TestLogger<TestRelationalLoggingDefinitions>())
                    .GenerateMessage(),
                Assert.Throws<InvalidOperationException>(
                    () => context.Parents.Include(p => p.Children1).Include(p => p.Children2).ToList()).Message);
        }
    }

    [ConditionalFact]
    public override async Task Using_AsSingleQuery_without_context_configuration_does_not_throw_warning()
    {
        var contextFactory = await InitializeAsync<Context21355>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context21355.Child>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context21355.AnotherChild>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });
        using var context = contextFactory.CreateContext();
        context.Parents.Include(p => p.Children1).Include(p => p.Children2).AsSingleQuery().ToList();
    }

    [ConditionalFact]
    public override async Task SplitQuery_disposes_inner_data_readers()
    {
        var contextFactory = await InitializeAsync<Context21355>(seed: c => c.SeedAsync(), onModelCreating: modelBuilder =>
        {
            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Context21355.Child>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
            modelBuilder.Entity<Context21355.AnotherChild>()
                .Property(e => e.Id)
                .HasColumnType("bigint");
        });

        ((RelationalTestStore)contextFactory.TestStore).CloseConnection();

        using (var context = contextFactory.CreateContext())
        {
            context.Parents.Include(p => p.Children1).Include(p => p.Children2).AsSplitQuery().ToList();

            Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
        }

        using (var context = contextFactory.CreateContext())
        {
            await context.Parents.Include(p => p.Children1).Include(p => p.Children2).AsSplitQuery().ToListAsync();

            Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
        }

        using (var context = contextFactory.CreateContext())
        {
            context.Parents.Include(p => p.Children1).Include(p => p.Children2).OrderBy(e => e.Id).AsSplitQuery().Single();

            Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
        }

        using (var context = contextFactory.CreateContext())
        {
            await context.Parents.Include(p => p.Children1).Include(p => p.Children2).OrderBy(e => e.Id).AsSplitQuery().SingleAsync();

            Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
        }
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public override async Task NoTracking_split_query_creates_only_required_instances(bool async)
    {
        var contextFactory = await InitializeAsync<Context25400>(
            seed: c => c.SeedAsync(),
            onConfiguring: o => SetQuerySplittingBehavior(o, QuerySplittingBehavior.SplitQuery),
            onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Context25400.Test>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

        using var context = contextFactory.CreateContext();
        Context25400.Test.ConstructorCallCount = 0;

        var query = context.Set<Context25400.Test>().AsNoTracking().OrderBy(e => e.Id);
        var test = async
            ? await query.FirstOrDefaultAsync()
            : query.FirstOrDefault();

        Assert.Equal(1, Context25400.Test.ConstructorCallCount);
    }

    protected override DbContextOptionsBuilder SetQuerySplittingBehavior(
        DbContextOptionsBuilder optionsBuilder,
        QuerySplittingBehavior splittingBehavior)
    {
        new SingleStoreDbContextOptionsBuilder(optionsBuilder).UseQuerySplittingBehavior(splittingBehavior);

        return optionsBuilder;
    }

    protected override DbContextOptionsBuilder ClearQuerySplittingBehavior(DbContextOptionsBuilder optionsBuilder)
    {
        var extension = optionsBuilder.Options.FindExtension<SingleStoreOptionsExtension>();
        if (extension == null)
        {
            extension = new SingleStoreOptionsExtension();
        }
        else
        {
            _querySplittingBehaviorFieldInfo.SetValue(extension, null);
        }

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static readonly FieldInfo _querySplittingBehaviorFieldInfo =
        typeof(RelationalOptionsExtension).GetField("_querySplittingBehavior", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
