using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;
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

        [ConditionalFact]
        public override void Can_insert_update_delete()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            base.Can_insert_update_delete();
        }

        [ConditionalTheory]
        public override Task Can_include_prey(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_include_prey(async);
        }
    }
}
