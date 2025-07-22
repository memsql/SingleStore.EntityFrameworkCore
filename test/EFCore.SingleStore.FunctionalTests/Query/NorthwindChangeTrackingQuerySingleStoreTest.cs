using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestModels.Northwind;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindChangeTrackingQuerySingleStoreTest : NorthwindChangeTrackingQueryTestBase<
        NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindChangeTrackingQuerySingleStoreTest(NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void Multiple_entities_can_revert()
        {
            using var context = CreateContext();

            var customerPostalCodes = context.Customers.OrderBy(c => c.CustomerID).Select(c => c.PostalCode).ToList();
            var customerRegion = context.Customers.OrderBy(c => c.CustomerID).Select(c => c.Region).ToList();

            foreach (var customer in context.Customers)
            {
                customer.PostalCode = "98052";
                customer.Region = "'Murica";
            }

            Assert.Equal(91, context.ChangeTracker.Entries().Count());
            Assert.Equal("98052", context.Customers.First().PostalCode);
            Assert.Equal("'Murica", context.Customers.First().Region);

            foreach (var entityEntry in context.ChangeTracker.Entries().ToList())
            {
                entityEntry.State = EntityState.Unchanged;
            }

            var newCustomerPostalCodes = context.Customers.OrderBy(c => c.CustomerID).Select(c => c.PostalCode);
            var newCustomerRegion = context.Customers.OrderBy(c => c.CustomerID).Select(c => c.Region);

            Assert.Equal(customerPostalCodes, newCustomerPostalCodes);
            Assert.Equal(customerRegion, newCustomerRegion);
        }

        protected override NorthwindContext CreateNoTrackingContext()
            => new NorthwindSingleStoreContext(
                new DbContextOptionsBuilder(Fixture.CreateOptions())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options);
    }
}
