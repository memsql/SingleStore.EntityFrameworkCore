using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPCInheritanceQuerySingleStoreTest : TPCInheritanceQueryTestBase<TPCInheritanceQuerySingleStoreFixture>
{
    public TPCInheritanceQuerySingleStoreTest(
        TPCInheritanceQuerySingleStoreFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact(Skip = "Feature 'FOREIGN KEY' is not supported by SingleStore.")]
    public override void Setting_foreign_key_to_a_different_type_throws()
    {
        base.Setting_foreign_key_to_a_different_type_throws();
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());
}
