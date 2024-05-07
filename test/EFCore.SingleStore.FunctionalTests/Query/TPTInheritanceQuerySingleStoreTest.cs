using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTInheritanceQuerySingleStoreTest : TPTInheritanceQueryTestBase<TPTInheritanceQuerySingleStoreFixture>
    {
        public TPTInheritanceQuerySingleStoreTest(TPTInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "Feature 'FOREIGN KEY' is not supported by SingleStore.")]
        public override void Setting_foreign_key_to_a_different_type_throws()
        {
            base.Setting_foreign_key_to_a_different_type_throws();
        }
    }
}
