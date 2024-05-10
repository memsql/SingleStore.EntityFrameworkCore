using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindQuerySingleStoreFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreNorthwindTestStoreFactory.Instance;

        protected override bool ShouldLogCategory(string logCategory)
            => logCategory == DbLoggerCategory.Query.Name || logCategory == DbLoggerCategory.Database.Command.Name;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder)
                .EnableDetailedErrors();

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<CustomerQuery>().ToSqlQuery("SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region` FROM `Customers` AS `c`");
            modelBuilder.Entity<OrderQuery>().ToSqlQuery(@"select * from `Orders`");
        }
    }
}
