using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class WarningsSingleStoreTest : WarningsTestBase<QueryNoClientEvalSingleStoreFixture>
    {
        public WarningsSingleStoreTest(QueryNoClientEvalSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override void FirstOrDefault_without_orderby_and_filter_issues_warning_subquery()
        {
            base.FirstOrDefault_without_orderby_and_filter_issues_warning_subquery();
        }

        [ConditionalFact(Skip = "Feature 'Correlated subselect that can not be transformed and does not match on shard keys' is not supported by SingleStore")]
        public override void LastOrDefault_with_order_by_does_not_issue_client_eval_warning()
        {
            base.LastOrDefault_with_order_by_does_not_issue_client_eval_warning();
        }
    }
}
