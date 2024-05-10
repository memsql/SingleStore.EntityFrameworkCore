using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class InheritanceQuerySingleStoreTest : InheritanceRelationalQueryTestBase<InheritanceQuerySingleStoreFixture>
    {
        public InheritanceQuerySingleStoreTest(InheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "https://github.com/mysql-net/SingleStoreConnector/pull/896")]
        public override Task Byte_enum_value_constant_used_in_projection(bool async)
        {
            return base.Byte_enum_value_constant_used_in_projection(async);
        }

        [ConditionalFact(Skip = "Feature 'FOREIGN KEY' is not supported by SingleStore.")]
        public override void Setting_foreign_key_to_a_different_type_throws()
        {
            base.Setting_foreign_key_to_a_different_type_throws();
        }
    }
}
