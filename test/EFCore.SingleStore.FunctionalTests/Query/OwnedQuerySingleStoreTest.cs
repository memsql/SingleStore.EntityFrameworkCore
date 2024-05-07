using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class OwnedQuerySingleStoreTest : OwnedQueryRelationalTestBase<OwnedQuerySingleStoreTest.OwnedQuerySingleStoreFixture>
    {
        public OwnedQuerySingleStoreTest(OwnedQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        public class OwnedQuerySingleStoreFixture : RelationalOwnedQueryFixture
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Navigation_rewrite_on_owned_collection_with_composition(bool async)
        {
            return base.Navigation_rewrite_on_owned_collection_with_composition(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Navigation_rewrite_on_owned_collection_with_composition_complex(bool async)
        {
            return base.Navigation_rewrite_on_owned_collection_with_composition_complex(async);
        }

        [ConditionalTheory(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override Task Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection_count(bool async)
        {
            return base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection_count(async);
        }
    }
}
