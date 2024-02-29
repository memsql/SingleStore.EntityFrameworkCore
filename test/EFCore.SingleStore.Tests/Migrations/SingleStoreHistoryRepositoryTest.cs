using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Migrations.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore.Migrations;

public class SingleStoreHistoryRepositoryTest
{
    [ConditionalFact]
    public void ExistsSql_respects_SchemaBehavior_explicit()
    {
        var sql = CreateHistoryRepository("IgnoreThisDefaultSchema")
            .ExistsSql;

        Assert.Equal(
            @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='DummyDatabase' AND TABLE_NAME='__EFMigrationsHistory';",
            sql);
    }

    [ConditionalFact]
    public void ExistsSql_respects_SchemaBehavior()
    {
        var sql = CreateHistoryRepository("IgnoreThisExplicitSchema")
            .ExistsSql;

        Assert.Equal(
            @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='DummyDatabase' AND TABLE_NAME='__EFMigrationsHistory';",
            sql);
    }

    private static TestMysqlHistoryRepository CreateHistoryRepository(string schema = null)
        => (TestMysqlHistoryRepository)new TestDbContext(
                new DbContextOptionsBuilder(
                        SingleStoreTestHelpers.Instance.CreateOptions(builder => builder
                            .SchemaBehavior(SingleStoreSchemaBehavior.Ignore)
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema)))
                    .UseInternalServiceProvider(
                        SingleStoreTestHelpers.Instance.CreateServiceProvider(
                            new ServiceCollection()
                                .AddScoped<IHistoryRepository, TestMysqlHistoryRepository>()))
                    .Options)
            .GetService<IHistoryRepository>();

    private class TestMysqlHistoryRepository : SingleStoreHistoryRepository
    {
        public TestMysqlHistoryRepository([NotNull] HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        public new string ExistsSql => base.ExistsSql;
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("IgnoreThisDefaultSchema");
        }
    }

    private class Blog
    {
        public int Id { get; set; }
    }
}
