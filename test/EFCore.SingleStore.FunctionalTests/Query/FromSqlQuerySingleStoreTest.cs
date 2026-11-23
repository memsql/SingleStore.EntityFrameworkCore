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
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
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

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CommonTableExpressions))]
        public override Task FromSqlRaw_composed_with_common_table_expression(bool async)
        {
            return base.FromSqlRaw_composed_with_common_table_expression(async);
        }

        // Same as the base test, except for the result: SingleStoreConnector ignores `Size` for input parameters, so the
        // second parameter isn't truncated to "Sea" (as it is on SQL Server) and both subqueries return the same customer.
        public override async Task Multiple_occurrences_of_FromSql_with_db_parameter_adds_two_parameters(bool async)
        {
            using var context = CreateContext();
            var city = "Seattle";

            var dbParameter1 = CreateDbParameter("city", city);
            dbParameter1.Size = 7;
            var subquery1 = context.Database.SqlQueryRaw<UnmappedCustomer>(
                NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"),
                dbParameter1);

            var dbParameter2 = CreateDbParameter("city", city);
            dbParameter2.Size = 3;
            var subquery2 = context.Database.SqlQueryRaw<UnmappedCustomer>(
                NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"),
                dbParameter2);

            var query = subquery1.Intersect(subquery2);

            var actual = async
                ? await query.ToArrayAsync()
                : query.ToArray();

            var customer = Assert.Single(actual);
            Assert.Equal("WHITC", customer.CustomerID);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new SingleStoreParameter
            {
                ParameterName = name,
                Value = value
            };
    }
}
