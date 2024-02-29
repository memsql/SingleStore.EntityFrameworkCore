using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class ComplexNavigationsCollectionsSplitSharedTypeQuerySingleStoreTest : ComplexNavigationsCollectionsSplitSharedTypeQueryRelationalTestBase<ComplexNavigationsSharedTypeQuerySingleStoreFixture>
    {
        public ComplexNavigationsCollectionsSplitSharedTypeQuerySingleStoreTest(
            ComplexNavigationsSharedTypeQuerySingleStoreFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore Distributed.")]
        public override Task Filtered_include_context_accessed_inside_filter_correlated(bool async)
        {
            return base.Filtered_include_context_accessed_inside_filter_correlated(async);
        }
    }
}
