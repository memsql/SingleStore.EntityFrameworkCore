using System;
using System.Linq;
using System.Threading.Tasks;
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

        [ConditionalFact]
        public override void Can_use_shared_columns_with_conversion()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
		    // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            base.Can_use_shared_columns_with_conversion();
        }

        [ConditionalFact]
        public override void Can_change_enums_with_conversion()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            base.Can_change_enums_with_conversion();
        }

        [ConditionalTheory]
        public override async Task Can_change_type_of__dependent_by_replacing_with_new_dependent(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            await base.Can_change_type_of__dependent_by_replacing_with_new_dependent(async);
        }

        [ConditionalTheory]
        public override async Task Can_change_type_of_pk_to_pk_dependent_by_replacing_with_new_dependent(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            await base.Can_change_type_of_pk_to_pk_dependent_by_replacing_with_new_dependent(async);
        }

        [ConditionalFact]
        public override void Save_replaced_principal()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            base.Save_replaced_principal();
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public new Task SaveChanges_false_processes_all_tracked_entities_without_calling_AcceptAllChanges(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.SaveChanges_false_processes_all_tracked_entities_without_calling_AcceptAllChanges(async);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public new Task SaveChanges_processes_all_tracked_entities(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.SaveChanges_processes_all_tracked_entities(async);
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
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Category>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Models.Issue1300.Flavor>()
                    .Property(e => e.FlavorId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Gift>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Lift>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Person>()
                    .Property(e => e.PersonId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Profile>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<GiftObscurer>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LiftObscurer>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectly>()
                    .Property(e => e.ProfileId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ProductCategory>()
                    .Property(e => e.CategoryId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LoginEntityTypeWithAnExtremelyLongAndOverlyConvolutedNameThatIsUsedToVerifyThatTheStoreIdentifierGenerationLengthLimitIsWorkingCorrectlyDetails>()
                    .Property(e => e.ProfileId)
                    .HasColumnType("bigint");
                base.OnModelCreating(modelBuilder, context);

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
