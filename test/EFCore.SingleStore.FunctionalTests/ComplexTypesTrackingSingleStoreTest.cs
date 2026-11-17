using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class ComplexTypesTrackingSingleStoreTest : ComplexTypesTrackingTestBase<ComplexTypesTrackingSingleStoreTest.SingleStoreFixture>
{
    public ComplexTypesTrackingSingleStoreTest(SingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    #region Complex collections mapped to JSON

    // The entity types with complex collections (`Activities`), that EF Core requires to be mapped to JSON columns, are being ignored
    // in the fixture, so the following tests that work with them are being skipped.

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override Task Can_track_entity_with_complex_type_collections(EntityState state, bool async)
        => base.Can_track_entity_with_complex_type_collections(state, async);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_mark_complex_type_collection_properties_modified(bool trackFromQuery)
        => base.Can_mark_complex_type_collection_properties_modified(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_read_original_values_for_properties_of_complex_type_collections(bool trackFromQuery)
        => base.Can_read_original_values_for_properties_of_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_write_original_values_for_properties_of_complex_type_collections(bool trackFromQuery)
        => base.Can_write_original_values_for_properties_of_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override Task Can_track_entity_with_complex_record_collections(EntityState state, bool async)
        => base.Can_track_entity_with_complex_record_collections(state, async);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_mark_complex_record_collection_properties_modified(bool trackFromQuery)
        => base.Can_mark_complex_record_collection_properties_modified(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_read_original_values_for_properties_of_complex_record_collections(bool trackFromQuery)
        => base.Can_read_original_values_for_properties_of_complex_record_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_write_original_values_for_properties_of_complex_record_collections(bool trackFromQuery)
        => base.Can_write_original_values_for_properties_of_complex_record_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override Task Can_track_entity_with_complex_field_collections(EntityState state, bool async)
        => base.Can_track_entity_with_complex_field_collections(state, async);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_mark_complex_field_collection_properties_modified(bool trackFromQuery)
        => base.Can_mark_complex_field_collection_properties_modified(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_read_original_values_for_properties_of_complex_field_collections(bool trackFromQuery)
        => base.Can_read_original_values_for_properties_of_complex_field_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_write_original_values_for_properties_of_complex_field_collections(bool trackFromQuery)
        => base.Can_write_original_values_for_properties_of_complex_field_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override Task Can_track_entity_with_complex_record_collections_with_fields(EntityState state, bool async)
        => base.Can_track_entity_with_complex_record_collections_with_fields(state, async);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_mark_complex_record_collections_with_fields_properties_modified(bool trackFromQuery)
        => base.Can_mark_complex_record_collections_with_fields_properties_modified(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_read_original_values_for_properties_of_complex_record_collections_with_fields(bool trackFromQuery)
        => base.Can_read_original_values_for_properties_of_complex_record_collections_with_fields(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_write_original_values_for_properties_of_complex_record_collections_with_fields(bool trackFromQuery)
        => base.Can_write_original_values_for_properties_of_complex_record_collections_with_fields(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override Task Can_track_entity_with_complex_property_bag_collections(EntityState state, bool async)
        => base.Can_track_entity_with_complex_property_bag_collections(state, async);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_mark_complex_property_bag_collection_properties_modified(bool trackFromQuery)
        => base.Can_mark_complex_property_bag_collection_properties_modified(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_read_original_values_for_properties_of_complex_property_bag_collections(bool trackFromQuery)
        => base.Can_read_original_values_for_properties_of_complex_property_bag_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_write_original_values_for_properties_of_complex_property_bag_collections(bool trackFromQuery)
        => base.Can_write_original_values_for_properties_of_complex_property_bag_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_reordered_elements_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_reordered_elements_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_added_elements_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_added_elements_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_removed_elements_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_removed_elements_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_replaced_elements_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_replaced_elements_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_duplicates_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_duplicates_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_handle_null_elements_in_complex_type_collections(bool trackFromQuery)
        => base.Can_handle_null_elements_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_swapped_complex_objects_in_collections(bool trackFromQuery)
        => base.Can_detect_swapped_complex_objects_in_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_handle_collection_with_mixed_null_and_duplicate_elements(bool trackFromQuery)
        => base.Can_handle_collection_with_mixed_null_and_duplicate_elements(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_changes_to_record_collection_elements(bool trackFromQuery)
        => base.Can_detect_changes_to_record_collection_elements(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_nested_collection_changes_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_nested_collection_changes_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_changes_to_nested_teams_members_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_changes_to_nested_teams_members_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_detect_changes_to_record_teams_in_complex_type_collections(bool trackFromQuery)
        => base.Can_detect_changes_to_record_teams_in_complex_type_collections(trackFromQuery);

    [ConditionalTheory(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Can_handle_empty_nested_teams_in_complex_type_collections(bool trackFromQuery)
        => base.Can_handle_empty_nested_teams_in_complex_type_collections(trackFromQuery);

    [ConditionalFact(Skip = "TODO: PLAT-6409 (requires JSON column support)")]
    public override void Throws_when_accessing_complex_entries_using_incorrect_cardinality()
        => base.Throws_when_accessing_complex_entries_using_incorrect_cardinality();

    #endregion Complex collections mapped to JSON

    public class SingleStoreFixture : FixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // The following entity types have complex collections (`Activities`), that EF Core requires to be mapped to JSON columns
            // (`ToJson()`), which is not supported by the SingleStore provider yet.
            modelBuilder.Ignore<PubWithCollections>();
            modelBuilder.Ignore<PubWithRecordCollections>();
            modelBuilder.Ignore<PubWithArrayCollections>();
            modelBuilder.Ignore<PubWithRecordArrayCollections>();
            modelBuilder.Ignore<PubWithPropertyBagCollections>();
            modelBuilder.Ignore<FieldPubWithCollections>();
            modelBuilder.Ignore<FieldPubWithRecordCollections>();

            // modelBuilder.Entity<Pub>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<PubWithStructs>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<PubWithReadonlyStructs>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<PubWithRecords>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<FieldPub>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<FieldPubWithStructs>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // // TODO: Allow binding of complex properties to constructors
            // // modelBuilder.Entity<FieldPubWithReadonlyStructs>(
            // //     b =>
            // //     {
            // //         b.ComplexProperty(
            // //             e => e.LunchtimeActivity, b =>
            // //             {
            // //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            // //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            // //                 b.Ignore(e => e!.Notes);
            // //             });
            // //         b.ComplexProperty(
            // //             e => e.EveningActivity, b =>
            // //             {
            // //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            // //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            // //                 b.Ignore(e => e.Notes);
            // //             });
            // //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            // //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            // //     });
            //
            // modelBuilder.Entity<FieldPubWithRecords>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });

            // // TODO: We currently don't support primitive collections yet.
            // var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            // var unprocessedComplexProperties = new Queue<IMutableComplexProperty>(entityTypes.SelectMany(e => e.GetComplexProperties()));
            // var processedComplexProperties = new HashSet<IMutableComplexProperty>();
            //
            // while (unprocessedComplexProperties.TryDequeue(out var complexProperty))
            // {
            //     processedComplexProperties.Add(complexProperty);
            //
            //     Debug.WriteLine(complexProperty.ClrType.Name);
            //
            //     var unmappedProperties = complexProperty.ComplexType.ClrType
            //         .GetProperties()
            //         .Except(
            //             complexProperty.ComplexType
            //                 .GetProperties()
            //                 .Where(p => p.PropertyInfo is not null)
            //                 .Select(p => p.PropertyInfo));
            //
            //     foreach (var property in unmappedProperties)
            //     {
            //
            //         // if (propertyInfo.IsGenericType &&
            //         //     propertyInfo.GetGenericTypeDefinition() == typeof(List<>))
            //         // {
            //         //     Debug.WriteLine($"{propertyInfo.Name} {complexProperty.ClrType.Name}.{property.Name}");
            //         // }
            //     }
            //
            //     foreach (var innerComplexProperty in complexProperty.ComplexType.GetComplexProperties())
            //     {
            //         if (!processedComplexProperties.Contains(innerComplexProperty) &&
            //             !unprocessedComplexProperties.Contains(innerComplexProperty))
            //         {
            //             unprocessedComplexProperties.Enqueue(innerComplexProperty);
            //         }
            //     }
            // }
        }
    }
}
