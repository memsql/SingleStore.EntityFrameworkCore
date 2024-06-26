using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCore.SingleStore.IntegrationTests.Models
{
    public class Product
    {
        public Product()
        {
            ProductCategories = new List<ProductCategory>();
        }

        public long ProductId { get; set; }
        public string Name { get; set; }
        public long? ParentProductId { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

        [ForeignKey(nameof(ParentProductId))]
        public Product ParentProduct { get; set; }
        [InverseProperty(nameof(ParentProduct))]
        public List<Product> ChildProducts { get; set; }
    }

    public class ProductCategory
    {
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public class Category
    {
        public Category()
        {
            ProductCategories = new List<ProductCategory>();
        }

        public long CategoryId { get; set; }
        public string Name { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
    }
}
