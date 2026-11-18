using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class PropertyValuesSingleStoreTest : PropertyValuesTestBase<PropertyValuesSingleStoreTest.PropertyValuesSingleStoreFixture>
    {
        public PropertyValuesSingleStoreTest(PropertyValuesSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public override async Task Values_can_be_reloaded_from_database_for_entity_in_any_state_with_inheritance(EntityState state,
            bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            await base.Values_can_be_reloaded_from_database_for_entity_in_any_state_with_inheritance(state, async);
        }

        #region Complex collections mapped to JSON

        // The `School` entity has a complex collection (`Departments`), that EF Core requires to be mapped to a JSON column.
        // We ignore the entity in the fixture, so the following tests that work with it directly are being skipped.

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Using_complex_property_value_not_list_throws()
            => base.Using_complex_property_value_not_list_throws();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override Task Complex_collection_current_values_can_be_accessed_as_a_property_dictionary()
            => base.Complex_collection_current_values_can_be_accessed_as_a_property_dictionary();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override Task Complex_collection_original_values_can_be_accessed_as_a_property_dictionary()
            => base.Complex_collection_original_values_can_be_accessed_as_a_property_dictionary();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_values_from_object_works()
            => base.Setting_complex_collection_values_from_object_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_current_values_from_object_with_nulls_works()
            => base.Setting_complex_collection_current_values_from_object_with_nulls_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_original_values_from_object_with_nulls_works()
            => base.Setting_complex_collection_original_values_from_object_with_nulls_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_current_values_from_dictionary_works()
            => base.Setting_complex_collection_current_values_from_dictionary_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_values_from_DTO_with_nulls_works()
            => base.Setting_complex_collection_values_from_DTO_with_nulls_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_current_values_from_dictionary_with_nulls_works()
            => base.Setting_complex_collection_current_values_from_dictionary_with_nulls_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_original_values_from_dictionary_with_nulls_works()
            => base.Setting_complex_collection_original_values_from_dictionary_with_nulls_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void Setting_complex_collection_current_values_from_DTO_with_complex_metadata_access_works()
            => base.Setting_complex_collection_current_values_from_DTO_with_complex_metadata_access_works();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void SetValues_throws_for_complex_collection_with_non_list_value()
            => base.SetValues_throws_for_complex_collection_with_non_list_value();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void SetValues_throws_for_complex_collection_with_non_dictionary_item()
            => base.SetValues_throws_for_complex_collection_with_non_dictionary_item();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void SetValues_throws_for_nested_complex_collection_with_non_list_value()
            => base.SetValues_throws_for_nested_complex_collection_with_non_list_value();

        [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
        public override void SetValues_throws_for_nested_complex_collection_with_non_dictionary_item()
            => base.SetValues_throws_for_nested_complex_collection_with_non_dictionary_item();

        #endregion Complex collections mapped to JSON

        public class PropertyValuesSingleStoreFixture : PropertyValuesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            // `School` is mapped by the base fixture and then ignored in `OnModelCreating` below.
            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base.AddOptions(builder)
                    .ConfigureWarnings(w => w.Ignore(CoreEventId.MappedEntityTypeIgnoredWarning));

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<VirtualTeam>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Contact33307>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");

                base.OnModelCreating(modelBuilder, context);

                // `School` has a complex collection (`Departments`), that EF Core requires to be mapped to a JSON column (`ToJson()`),
                // which is not supported by the SingleStore provider yet. Tests that only optionally use `School` check whether it
                // exists in the model.
                modelBuilder.Ignore<School>();
            }
        }
    }
}
