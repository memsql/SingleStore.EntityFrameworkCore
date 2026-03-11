using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class MaterializationInterceptionSingleStoreTest : MaterializationInterceptionTestBase<MaterializationInterceptionSingleStoreTest.SingleStoreLibraryContext>
{
    private int _id = 1;

    [ConditionalTheory]
    public override async Task Intercept_query_materialization_with_owned_types_projecting_collection(bool async, bool usePooling)
    {
        // We're skipping this test when we're running tests on Managed Service due to the specifics of
        // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
        if (AppConfig.ManagedService)
        {
            return;
        }

        var creatingInstanceCounts = new Dictionary<Type, int>();
        var createdInstanceCounts = new Dictionary<Type, int>();
        var initializingInstanceCounts = new Dictionary<Type, int>();
        var initializedInstanceCounts = new Dictionary<Type, int>();
        LibraryContext context = null;

        var interceptors = new[]
        {
            new ValidatingMaterializationInterceptor(
                (data, instance, method) =>
                {
                    Assert.Same(context, data.Context);
                    Assert.Equal(QueryTrackingBehavior.NoTracking, data.QueryTrackingBehavior);

                    int count;
                    var clrType = data.EntityType.ClrType;
                    switch (method)
                    {
                        case nameof(IMaterializationInterceptor.CreatingInstance):
                            count = creatingInstanceCounts.GetOrAddNew(clrType);
                            creatingInstanceCounts[clrType] = count + 1;
                            Assert.Null(instance);
                            break;
                        case nameof(IMaterializationInterceptor.CreatedInstance):
                            count = createdInstanceCounts.GetOrAddNew(clrType);
                            createdInstanceCounts[clrType] = count + 1;
                            Assert.Same(clrType, instance!.GetType());
                            break;
                        case nameof(IMaterializationInterceptor.InitializingInstance):
                            count = initializingInstanceCounts.GetOrAddNew(clrType);
                            initializingInstanceCounts[clrType] = count + 1;
                            Assert.Same(clrType, instance!.GetType());
                            break;
                        case nameof(IMaterializationInterceptor.InitializedInstance):
                            count = initializedInstanceCounts.GetOrAddNew(clrType);
                            initializedInstanceCounts[clrType] = count + 1;
                            Assert.Same(clrType, instance!.GetType());
                            break;
                    }
                })
        };

        using (context = await CreateContext(interceptors, inject: true, usePooling))
        {
            context.Add(
                new TestEntity30244
                {
                    Id = _id++,
                    Title = "TestIssue",
                    Settings = { new KeyValueSetting30244("Value1", "1"), new KeyValueSetting30244("Value2", "9") }
                });

            _ = async
                ? await context.SaveChangesAsync()
                : context.SaveChanges();

            context.ChangeTracker.Clear();

            var query = context.Set<TestEntity30244>()
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Select(x => x.Settings
                    .Where(s => s.Key != "Foo")
                    .OrderBy(s => s.Key)
                    .ToList());

            var collection = async
                ? await query.FirstOrDefaultAsync()
                : query.FirstOrDefault();

            Assert.NotNull(collection);
            Assert.Equal("Value1", collection[0].Key);
            Assert.Equal("1", collection[0].Value);
            Assert.Contains(("Value2", "9"), collection.Select(x => (x.Key, x.Value)));

            Assert.Equal(1, creatingInstanceCounts.Count);
            Assert.Equal(2, creatingInstanceCounts[typeof(KeyValueSetting30244)]);

            Assert.Equal(1, createdInstanceCounts.Count);
            Assert.Equal(2, createdInstanceCounts[typeof(KeyValueSetting30244)]);

            Assert.Equal(1, initializingInstanceCounts.Count);
            Assert.Equal(2, initializingInstanceCounts[typeof(KeyValueSetting30244)]);

            Assert.Equal(1, initializedInstanceCounts.Count);
            Assert.Equal(2, initializedInstanceCounts[typeof(KeyValueSetting30244)]);
        }
    }

    public class SingleStoreLibraryContext : LibraryContext
    {
        public SingleStoreLibraryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
            // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
            modelBuilder.Entity<TestEntity30244>()
                .Property(e => e.Id)
                .HasColumnType("bigint");

            modelBuilder.Entity<TestEntity30244>().OwnsMany(
                e => e.Settings, b =>
                {
                    b.Property<long>("Id").HasColumnType("bigint");
                });

            modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings);

            // TODO: https://github.com/npgsql/efcore.pg/issues/2548
            // modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings, b => b.ToJson());
        }
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}

internal static class DictionaryExtensions
{
    public static TValue GetOrAddNew<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
        where TKey : notnull
        where TValue : new()
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = new TValue();
            dictionary[key] = value;
        }

        return value;
    }
}
