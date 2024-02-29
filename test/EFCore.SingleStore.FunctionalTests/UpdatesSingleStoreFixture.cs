using System;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class UpdatesSingleStoreFixture : UpdatesRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            // Previously this method contained method that was adding principal key (see comment below).
            // Principal key is playing a role of an alternative key which supposed to serve as an unique identifier,
            // but in order to be created in SingleStore database it must contain all columns specified in the shard key (primary key in our case).
            // Additionally, there was a block of entities that were created for 'Identifiers_are_generated_correctly' test.
            // It was deleted because feature 'Adding an INDEX with multiple columns on a columnstore table where
            // any column in the new index is already in an index' is not supported by SingleStore.

            modelBuilder.Entity<Product>().HasMany(e => e.ProductCategories).WithOne()
                .HasForeignKey(e => e.ProductId);
            modelBuilder.Entity<ProductWithBytes>().HasMany(e => e.ProductCategories).WithOne()
                .HasForeignKey(e => e.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasKey(p => new { p.CategoryId, p.ProductId });

            modelBuilder.Entity<Product>().HasOne<Category>().WithMany()
                .HasForeignKey(e => e.DependentId);
                // .HasPrincipalKey(e => e.PrincipalId);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Parent)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Category>().HasMany(e => e.ProductCategories).WithOne()
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<AFewBytes>()
                .Property(e => e.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<ProductViewTable>().HasBaseType((string)null).ToTable("ProductView");
            modelBuilder.Entity<ProductTableWithView>().HasBaseType((string)null).ToView("ProductView").ToTable("ProductTable");
            modelBuilder.Entity<ProductTableView>().HasBaseType((string)null).ToView("ProductTable");

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<Models.Issue1300.Flavor>()
                .Property(e => e.FlavorId)
                .HasColumnType("bigint");
            modelBuilder.Entity<Person>()
                .Property(e => e.PersonId)
                .HasColumnType("bigint");

            Models.Issue1300.Setup(modelBuilder, context);
        }

        public static class Models
        {
            public static class Issue1300
            {
                public static void Setup(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.Entity<Flavor>(
                        entity =>
                        {
                            entity.HasKey(e => new {e.FlavorId, e.DiscoveryDate});
                            entity.Property(e => e.FlavorId)
                                .ValueGeneratedOnAdd();
                            entity.Property(e => e.DiscoveryDate)
                                .ValueGeneratedOnAdd();
                        });
                }

                public class Flavor
                {
                    public int FlavorId { get; set; }
                    public DateTime DiscoveryDate { get; set; }
                }
            }
        }
    }
}
