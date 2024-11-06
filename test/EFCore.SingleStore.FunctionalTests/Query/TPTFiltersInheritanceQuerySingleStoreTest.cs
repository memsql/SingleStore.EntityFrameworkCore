using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQuerySingleStoreTest : TPTFiltersInheritanceQueryTestBase<TPTFiltersInheritanceQuerySingleStoreFixture>
    {
        public TPTFiltersInheritanceQuerySingleStoreTest(TPTFiltersInheritanceQuerySingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
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
