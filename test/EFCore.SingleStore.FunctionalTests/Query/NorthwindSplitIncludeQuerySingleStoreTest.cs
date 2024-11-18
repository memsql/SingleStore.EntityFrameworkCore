﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class NorthwindSplitIncludeQuerySingleStoreTest : NorthwindSplitIncludeQueryTestBase<NorthwindQuerySingleStoreFixture<NoopModelCustomizer>>
    {
        public NorthwindSplitIncludeQuerySingleStoreTest(NorthwindQuerySingleStoreFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //TestSqlLoggerFactory.CaptureOutput(testOutputHelper);
        }

        public override Task Include_collection_with_multiple_conditional_order_by(bool async)
        {
            // The order of `Orders` can be different, because it is not explicitly sorted.
            // This is the case on MariaDB.
            return AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Include(c => c.OrderDetails)
                    .OrderBy(o => o.OrderID > 0)
                    .ThenBy(o => o.Customer != null ? o.Customer.City : string.Empty)
                    .ThenBy(o => o.OrderID)
                    .Take(5),
                elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Order>(o => o.OrderDetails)),
                entryCount: 14);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task Include_collection_SelectMany_GroupBy_Select(bool async)
        {
            return base.Include_collection_SelectMany_GroupBy_Select(async);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task Join_Include_collection_GroupBy_Select(bool async)
        {
            return base.Join_Include_collection_GroupBy_Select(async);
        }

        [ConditionalTheory(Skip = "Further investigation is needed to determine why it is failing with SingleStore")]
        public override Task Include_collection_Join_GroupBy_Select(bool async)
        {
            return base.Include_collection_Join_GroupBy_Select(async);
        }

        public override Task SelectMany_Include_collection_GroupBy_Select(bool async)
        {
            // The original EF Core query depends on a specific implicit order of the Order.OrderDetails collection.
            // This order is not returned for some database server implementations (which is not a bug of the DBMS, but an inaccuracy of the
            // EF Core LINQ query definition).
            // Because it is tricky to manipulate the order of the Order.OrderDetails collection for this query, we just filter it to the
            // entities in question.
            return AssertQuery(
                async,
                ss => (from od in ss.Set<OrderDetail>().Where(od => od.OrderID == 10248)
                            .Where(od => od.ProductID == 72) // <-- explicit filtering needed for some MariaDB versions, because we cannot manually influence the order of the Order.OrderDetails property
                       from o in ss.Set<Order>().Include(o => o.OrderDetails) select o)
                    .GroupBy(e => e.OrderID)
                    .Select(e => e.OrderBy(o => o.OrderID).FirstOrDefault()),
                entryCount: 2985);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_duplicate_collection_result_operator(bool async)
        {
            // The order of `Orders` can be different, because it is not explicitly sorted.
            // This is the case on MariaDB.
            return AssertQuery(
                async,
                ss => (from c1 in ss.Set<Customer>().Include(c => c.Orders).OrderBy(c => c.CustomerID).ThenBy(c => c.Orders.FirstOrDefault() != null ? c.Orders.FirstOrDefault().CustomerID : null).Take(2)
                    from c2 in ss.Set<Customer>().Include(c => c.Orders).OrderBy(c => c.CustomerID).ThenBy(c => c.Orders.FirstOrDefault() != null ? c.Orders.FirstOrDefault().CustomerID : null).Skip(2).Take(2)
                    select new { c1, c2 }).OrderBy(t => t.c1.CustomerID).ThenBy(t => t.c2.CustomerID).Take(1),
                elementSorter: e => (e.c1.CustomerID, e.c2.CustomerID),
                elementAsserter: (e, a) =>
                {
                    AssertInclude(e.c1, a.c1, new ExpectedInclude<Customer>(c => c.Orders));
                    AssertInclude(e.c2, a.c2, new ExpectedInclude<Customer>(c => c.Orders));
                },
                entryCount: 15);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_duplicate_collection_result_operator2(bool async)
        {
            // The order of `Orders` can be different, because it is not explicitly sorted.
            // The order of the end result can be different as well.
            // This is the case on MariaDB.
            return AssertQuery(
                async,
                ss => (from c1 in ss.Set<Customer>().Include(c => c.Orders).OrderBy(c => c.CustomerID).ThenBy(c => c.Orders.FirstOrDefault() != null ? c.Orders.FirstOrDefault().CustomerID : null).Take(2)
                    from c2 in ss.Set<Customer>().OrderBy(c => c.CustomerID).Skip(2).Take(2)
                    select new { c1, c2 }).OrderBy(t => t.c1.CustomerID).ThenBy(t => t.c2.CustomerID).Take(1),
                elementSorter: e => (e.c1.CustomerID, e.c2.CustomerID),
                elementAsserter: (e, a) =>
                {
                    AssertInclude(e.c1, a.c1, new ExpectedInclude<Customer>(c => c.Orders));
                    AssertEqual(e.c2, a.c2);
                },
                entryCount: 8);
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/21202")]
        public override Task Include_collection_skip_no_order_by(bool async)
        {
            return base.Include_collection_skip_no_order_by(async);
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/21202")]
        public override Task Include_collection_skip_take_no_order_by(bool async)
        {
            return base.Include_collection_skip_take_no_order_by(async);
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/21202")]
        public override Task Include_collection_take_no_order_by(bool async)
        {
            return base.Include_collection_take_no_order_by(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Then_include_collection_order_by_collection_column(bool async)
        {
            return base.Then_include_collection_order_by_collection_column(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_collection_OrderBy_list_contains(bool async)
        {
            return base.Include_collection_OrderBy_list_contains(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_collection_order_by_collection_column(bool async)
        {
            return base.Include_collection_order_by_collection_column(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: correlated subselect in ORDER BY")]
        public override Task Include_collection_order_by_subquery(bool async)
        {
            return base.Include_collection_order_by_subquery(async);
        }

        [ConditionalTheory(Skip = "Feature 'scalar subselect inside the GROUP/ORDER BY of a pushed down query' is not supported by SingleStore")]
        public override Task Include_collection_OrderBy_empty_list_contains(bool async)
        {
            return base.Include_collection_OrderBy_empty_list_contains(async);
        }

        [ConditionalTheory(Skip = "Feature 'scalar subselect inside the GROUP/ORDER BY of a pushed down query' is not supported by SingleStore")]
        public override Task Include_collection_OrderBy_empty_list_does_not_contains(bool async)
        {
            return base.Include_collection_OrderBy_empty_list_does_not_contains(async);
        }

        public override Task Repro9735(bool async)
        {
            return AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Include(b => b.OrderDetails)
                    .OrderBy(b => b.Customer.CustomerID != null)
                    .ThenBy(b => b.Customer != null ? b.Customer.CustomerID : string.Empty)
                    .ThenBy(b => b.EmployeeID) // Needs to be explicitly ordered by EmployeeID as well
                    .Take(2),
                entryCount: 6);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
