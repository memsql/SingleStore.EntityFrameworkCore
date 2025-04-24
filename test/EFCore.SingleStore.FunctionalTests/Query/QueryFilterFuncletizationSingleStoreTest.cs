using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class QueryFilterFuncletizationSingleStoreTest
        : QueryFilterFuncletizationTestBase<QueryFilterFuncletizationSingleStoreTest.QueryFilterFuncletizationSingleStoreFixture>
    {
        public QueryFilterFuncletizationSingleStoreTest(
            QueryFilterFuncletizationSingleStoreFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void Using_DbSet_in_filter_works()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            base.Using_DbSet_in_filter_works();
        }

        public override void DbContext_list_is_parameterized()
        {
            base.DbContext_list_is_parameterized();

            if (SingleStoreTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
            {
                AssertSql(
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE(NULL, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""",
                    //
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""",
                    //
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[1]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""",
                    //
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[2,3]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""");
            }
            else
            {
                AssertSql(
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE FALSE
""",
                    //
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE FALSE
""",
                    //
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` = 1
""",
                    //
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (2, 3)
""");
            }
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class QueryFilterFuncletizationSingleStoreFixture : QueryFilterFuncletizationRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<ComplexFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<DbContextStaticMemberFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<EntityTypeConfigurationFieldFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<EntityTypeConfigurationMethodCallFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<EntityTypeConfigurationPropertyChainFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<EntityTypeConfigurationPropertyFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ExtensionBuilderFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ExtensionContextFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<FieldFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ListFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LocalMethodFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LocalMethodParamsFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LocalVariableErrorFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<LocalVariableFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<MethodCallChainFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<MethodCallFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<MultiContextFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ParameterFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<PrincipalSetFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<PropertyChainFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<PropertyFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<PropertyMethodCallFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<RemoteMethodParamsFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<ShortCircuitFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<StaticMemberFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<DependentSetFilter>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            }
        }
    }
}
