using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class FiltersInheritanceQuerySingleStoreTest : FiltersInheritanceQueryTestBase<FiltersInheritanceQuerySingleStoreFixture>
    {
        public FiltersInheritanceQuerySingleStoreTest(FiltersInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }


        [ConditionalTheory]
        public override async Task Can_use_IgnoreQueryFilters_and_GetDatabaseValues(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            await base.Can_use_IgnoreQueryFilters_and_GetDatabaseValues(async);
        }
    }
}
