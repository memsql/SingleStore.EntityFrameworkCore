using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPHInheritanceQuerySingleStoreTest : TPHInheritanceQueryTestBase<TPHInheritanceQuerySingleStoreFixture>
{
    public TPHInheritanceQuerySingleStoreTest(TPHInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    [ConditionalFact(Skip = "Feature 'FOREIGN KEY' is not supported by SingleStore Distributed.")]
    public override void Setting_foreign_key_to_a_different_type_throws()
    {
        base.Setting_foreign_key_to_a_different_type_throws();
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override Task Can_include_prey(bool async)
    {
        // Skipping this test when running on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return Task.CompletedTask;
        }

        return base.Can_include_prey(async);
    }

    [ConditionalFact]
    public override void Can_insert_update_delete()
    {
        // Skipping this test when running on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        base.Can_insert_update_delete();
    }
}
