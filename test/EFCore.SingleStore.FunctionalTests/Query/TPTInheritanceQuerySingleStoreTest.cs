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

        [ConditionalTheory]
        public override Task Can_use_of_type_animal(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_animal(async);
        }

        [ConditionalTheory]
        public override Task Can_use_is_kiwi(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_is_kiwi(async);
        }

        [ConditionalTheory]
        public override Task Can_use_is_kiwi_with_other_predicate(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_is_kiwi_with_other_predicate(async);
        }

        [ConditionalTheory]
        public override Task Can_use_is_kiwi_in_projection(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_is_kiwi_in_projection(async);
        }

        [ConditionalTheory]
        public override Task Can_use_of_type_bird(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_bird(async);
        }

        [ConditionalTheory]
        public override Task Can_use_of_type_bird_predicate(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_bird_predicate(async);
        }

        [ConditionalTheory]
        public override Task Can_use_of_type_bird_with_projection(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_bird_with_projection(async);
        }

        [ConditionalTheory]
        public override Task Can_use_of_type_bird_first(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_bird_first(async);
        }

        [ConditionalTheory]
        public override Task Can_use_of_type_kiwi(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_kiwi(async);
        }
    }
}
