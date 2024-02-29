using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using SingleStoreConnector;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FromSqlQuerySingleStoreTest : FromSqlQueryTestBase<NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public FromSqlQuerySingleStoreTest(NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Bad_data_error_handling_invalid_cast(bool async)
        {
            using var context = CreateContext();
            var query = context.Set<Product>().FromSqlRaw(
                NormalizeDelimitersInRawString(
                    @"SELECT [ProductID], [SupplierID] AS [UnitPrice], [ProductName], [SupplierID], [UnitsInStock], [Discontinued]
                      FROM [Products]"));

            Assert.Equal(
                CoreStrings.ErrorMaterializingPropertyInvalidCast("Product", "UnitPrice", typeof(decimal?), typeof(long)),
                (async
                ? await Assert.ThrowsAsync<InvalidOperationException>(() => query.ToListAsync())
                : Assert.Throws<InvalidOperationException>(() => query.ToList())).Message);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Bad_data_error_handling_invalid_cast_projection(bool async)
        {
            using var context = CreateContext();
            var query = context.Set<Product>().FromSqlRaw(
                    NormalizeDelimitersInRawString(
                        @"SELECT [ProductID], [SupplierID] AS [UnitPrice], [ProductName], [UnitsInStock], [Discontinued]
                      FROM [Products]"))
                .Select(p => p.UnitPrice);

            Assert.Equal(
                RelationalStrings.ErrorMaterializingValueInvalidCast(typeof(decimal?), typeof(long)),
                (async
                    ? await Assert.ThrowsAsync<InvalidOperationException>(() => query.ToListAsync())
                    : Assert.Throws<InvalidOperationException>(() => query.ToList())).Message);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new SingleStoreParameter
            {
                ParameterName = name,
                Value = value
            };
    }
}
