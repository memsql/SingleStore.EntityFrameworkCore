using System;
using System.Linq;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

// ReSharper disable InconsistentNaming
namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests.
    internal class DefaultValuesTest : IDisposable
    {
        private readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddEntityFrameworkSingleStore()
            .BuildServiceProvider();

        [Fact]
        public void Can_use_SingleStore_default_values()
        {
            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var honeyDijon = context.Add(new KettleChips { Name = "Honey Dijon", AllergenLabeling = "Mustard" }).Entity;
                var buffaloBleu = context.Add(new KettleChips { Name = "Buffalo Bleu", AllergenLabeling = "None", BestBuyDate = new DateTime(2111, 1, 11) }).Entity;

                context.SaveChanges();

                Assert.Equal(new DateTime(2035, 9, 25), honeyDijon.BestBuyDate);
                Assert.Equal(new DateTime(2111, 1, 11), buffaloBleu.BestBuyDate);
            }

            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                Assert.Equal(new DateTime(2035, 9, 25), context.Chips.Single(c => c.Name == "Honey Dijon").BestBuyDate);
                Assert.Equal(new DateTime(2111, 1, 11), context.Chips.Single(c => c.Name == "Buffalo Bleu").BestBuyDate);
            }
        }

        [Fact]
        public void Can_use_SingleStore_default_values_depending_on_expression_syntax()
        {
            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var seaSalt = context.Add(new KettleChips()).Entity;

                context.SaveChanges();

                if (AppConfig.ServerVersion.Supports.DefaultExpression ||
                    AppConfig.ServerVersion.Supports.AlternativeDefaultExpression)
                {
                    Assert.Equal("Sea Salt", seaSalt.Name);
                    Assert.Equal("None", seaSalt.AllergenLabeling);
                }
                else
                {
                    Assert.Null(seaSalt.Name);
                    Assert.Null(seaSalt.AllergenLabeling);
                }

                Assert.Equal(new DateTime(2035, 9, 25), seaSalt.BestBuyDate);
            }

            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                if (AppConfig.ServerVersion.Supports.DefaultExpression ||
                    AppConfig.ServerVersion.Supports.AlternativeDefaultExpression)
                {
                    Assert.Equal("Sea Salt", context.Chips.Single().Name);
                    Assert.Equal("None", context.Chips.Single().AllergenLabeling);
                }
                else
                {
                    Assert.Null(context.Chips.Single().Name);
                    Assert.Null(context.Chips.Single().AllergenLabeling);
                }
                Assert.Equal(new DateTime(2035, 9, 25), context.Chips.Single().BestBuyDate);
            }
        }

        public void Dispose()
        {
            using (var context = new ChipsContext(_serviceProvider, "DefaultKettleChips"))
            {
                context.Database.EnsureDeleted();
            }
        }

        private class ChipsContext : DbContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly string _databaseName;

            public ChipsContext(IServiceProvider serviceProvider, string databaseName)
            {
                _serviceProvider = serviceProvider;
                _databaseName = databaseName;
            }

            public DbSet<KettleChips> Chips { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseSingleStore(SingleStoreTestStore.CreateConnectionString(_databaseName, false))
                    .UseInternalServiceProvider(_serviceProvider);

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.Entity<KettleChips>(
                    entity =>
                    {
                        entity.Property(e => e.Name)
                            .HasDefaultValue("Sea Salt");

                        entity.Property(e => e.AllergenLabeling)
                            .HasDefaultValueSql("('None')");

                        entity.Property(e => e.BestBuyDate)
                            .ValueGeneratedOnAdd()
                            .HasDefaultValue(new DateTime(2035, 9, 25));
                    });
        }

        private class KettleChips
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string AllergenLabeling { get; set; }
            public DateTime BestBuyDate { get; set; }
        }
    }
}
