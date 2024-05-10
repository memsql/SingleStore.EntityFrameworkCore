using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class QueryNoClientEvalSingleStoreTest : QueryNoClientEvalTestBase<QueryNoClientEvalSingleStoreFixture>
    {
        public QueryNoClientEvalSingleStoreTest(QueryNoClientEvalSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public override void Doesnt_throw_when_from_sql_not_composed()
        {
            using (var context = CreateContext())
            {
                var customers
                    = context.Customers
                        .FromSqlRaw(@"select * from `Customers`")
                        .ToList();

                Assert.Equal(91, customers.Count);
            }
        }
    }
}
