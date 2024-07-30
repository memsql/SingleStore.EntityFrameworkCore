using System;
using System.Linq;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using EntityFrameworkCore.SingleStore.ValueGeneration.Internal;
using Xunit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;


namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class UpdatesSingleStoreTest : UpdatesRelationalTestBase<UpdatesSingleStoreTest.UpdatesSingleStoreFixture>
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


        [SupportedServerVersionCondition(nameof(ServerVersionSupport.DefaultExpression), nameof(ServerVersionSupport.AlternativeDefaultExpression))]
        [SupportedServerVersionCondition(nameof(ServerVersionSupport.Returning))]
        public override void Save_with_shared_foreign_key()
        {
            base.Save_with_shared_foreign_key();
        }

        public class UpdatesSingleStoreFixture : UpdatesRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // Necessary for test `Save_with_shared_foreign_key` to run correctly.
                if (AppConfig.ServerVersion.Supports.DefaultExpression ||
                    AppConfig.ServerVersion.Supports.AlternativeDefaultExpression)
                {
                    modelBuilder.Entity<ProductBase>()
                        .Property(p => p.Id).HasDefaultValueSql("(UUID())");
                }

                Models.Issue1300.Setup(modelBuilder, context);
            }

            public static class Models
            {
                public static class Issue1300
                {
                    public static void Setup(ModelBuilder modelBuilder, DbContext context)
                    {
                        modelBuilder.Entity<Flavor>(
                            entity =>
                            {
                                entity.HasKey(e => new {e.FlavorId, e.DiscoveryDate});
                                entity.Property(e => e.FlavorId)
                                    .ValueGeneratedOnAdd();
                                entity.Property(e => e.DiscoveryDate)
                                    .ValueGeneratedOnAdd();
                            });
                    }

                    public class Flavor
                    {
                        public int FlavorId { get; set; }
                        public DateTime DiscoveryDate { get; set; }
                    }
                }
            }
        }
    }
}
