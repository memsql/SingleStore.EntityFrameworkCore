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

        [ConditionalTheory]
        public override Task Byte_enum_value_constant_used_in_projection(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Byte_enum_value_constant_used_in_projection(async);
        }

        [ConditionalTheory]
        public override Task Can_filter_all_animals(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_filter_all_animals(async);
        }

        [ConditionalTheory]
        public override Task Can_include_animals(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_include_animals(async);
        }

        [ConditionalTheory]
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

        [ConditionalTheory]
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

        [ConditionalTheory]
        public override Task Can_query_all_animals(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_query_all_animals(async);
        }

        [ConditionalTheory]
        public override Task Can_query_all_birds(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_query_all_birds(async);
        }

        [ConditionalTheory]
        public override Task Can_query_just_kiwis(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_query_just_kiwis(async);
        }

        [ConditionalTheory]
        public override Task Can_query_when_shared_column(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_query_when_shared_column(async);
        }

        [ConditionalTheory]
        public override Task Can_use_backwards_is_animal(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_backwards_is_animal(async);
        }

        [ConditionalTheory]
        public override Task Can_use_backwards_of_type_animal(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_backwards_of_type_animal(async);
        }

        [ConditionalTheory]
        public override Task Can_use_is_kiwi_with_cast(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_is_kiwi_with_cast(async);
        }

        [ConditionalTheory]
        public override Task Can_use_of_type_kiwi_where_south_on_derived_property(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Can_use_of_type_kiwi_where_south_on_derived_property(async);
        }

        [ConditionalTheory]
        public override Task GetType_in_hierarchy_in_leaf_type_with_sibling(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.GetType_in_hierarchy_in_leaf_type_with_sibling(async);
        }

        [ConditionalTheory]
        public override Task GetType_in_hierarchy_in_leaf_type_with_sibling2(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.GetType_in_hierarchy_in_leaf_type_with_sibling2(async);
        }

        [ConditionalTheory]
        public override Task GetType_in_hierarchy_in_leaf_type_with_sibling2_not_equal(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.GetType_in_hierarchy_in_leaf_type_with_sibling2_not_equal(async);
        }

        [ConditionalTheory]
        public override Task GetType_in_hierarchy_in_leaf_type_with_sibling2_reverse(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.GetType_in_hierarchy_in_leaf_type_with_sibling2_reverse(async);
        }

        [ConditionalTheory]
        public override Task Is_operator_on_result_of_FirstOrDefault(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Is_operator_on_result_of_FirstOrDefault(async);
        }

        [ConditionalFact]
        public override void Member_access_on_intermediate_type_works()
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            base.Member_access_on_intermediate_type_works();
        }

        [ConditionalTheory]
        public override Task Selecting_only_base_properties_on_derived_type(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Selecting_only_base_properties_on_derived_type(async);
        }

        [ConditionalTheory]
        public override Task Subquery_OfType(bool async)
        {
            // Skipping this test when running on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return Task.CompletedTask;
            }

            return base.Subquery_OfType(async);
        }
    }
}
