using System;
using System.Linq;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Xunit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;


namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public partial class UpdatesSingleStoreTest : UpdatesRelationalTestBase<UpdatesSingleStoreFixture>
    {
        public UpdatesSingleStoreTest(UpdatesSingleStoreFixture fixture)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        [ConditionalFact(Skip = "Feature 'Adding an INDEX with multiple columns on a columnstore table where any column in the new index is already in an index' is not supported by SingleStore.")]
        public override void Identifiers_are_generated_correctly()
        {
            using (var context = CreateContext())
            {
                var entityType = context.Model.FindEntityType(typeof(
                    LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly));
                Assert.Equal("LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIs~", entityType.GetTableName());
                Assert.Equal("PK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~", entityType.GetKeys().Single().GetName());
                Assert.Equal("FK_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~", entityType.GetForeignKeys().Single().GetConstraintName());
                Assert.Equal("IX_LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameTha~", entityType.GetIndexes().Single().GetDatabaseName());
            }
        }

        [ConditionalFact]
        public override void Can_add_and_remove_self_refs()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
		    // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            base.Can_add_and_remove_self_refs();
        }
    }
}
