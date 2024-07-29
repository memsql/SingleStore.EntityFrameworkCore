using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;

// ReSharper disable InconsistentNaming
namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class SimpleQuerySingleStoreTest : SimpleQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

        public override async Task Multiple_nested_reference_navigations(bool async)
        {
            await base.Multiple_nested_reference_navigations(async);

            AssertSql(
                @"@__p_0='3'

SELECT `s`.`Id`, `s`.`Email`, `s`.`Logon`, `s`.`ManagerId`, `s`.`Name`, `s`.`SecondaryManagerId`
FROM `Staff` AS `s`
WHERE `s`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__id_0='1'

SELECT `a`.`Id`, `a`.`Complete`, `a`.`Deleted`, `a`.`PeriodEnd`, `a`.`PeriodStart`, `a`.`StaffId`, `s`.`Id`, `s`.`Email`, `s`.`Logon`, `s`.`ManagerId`, `s`.`Name`, `s`.`SecondaryManagerId`, `s0`.`Id`, `s0`.`Email`, `s0`.`Logon`, `s0`.`ManagerId`, `s0`.`Name`, `s0`.`SecondaryManagerId`, `s1`.`Id`, `s1`.`Email`, `s1`.`Logon`, `s1`.`ManagerId`, `s1`.`Name`, `s1`.`SecondaryManagerId`
FROM `Appraisals` AS `a`
INNER JOIN `Staff` AS `s` ON `a`.`StaffId` = `s`.`Id`
LEFT JOIN `Staff` AS `s0` ON `s`.`ManagerId` = `s0`.`Id`
LEFT JOIN `Staff` AS `s1` ON `s`.`SecondaryManagerId` = `s1`.`Id`
WHERE `a`.`Id` = @__id_0
LIMIT 2");
        }

        public override async Task Multiple_different_entity_type_from_different_namespaces(bool async)
        {
            var contextFactory = await InitializeAsync<Context23981>();
            using var context = contextFactory.CreateContext();
            var bad = context.Set<NameSpace1.TestQuery>().FromSqlRaw(@"SELECT cast(null as signed) AS MyValue").ToList(); // <-- MySQL uses `signed` instead of `int` in CAST() expressions
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Group_by_multiple_aggregate_joining_different_tables(bool async)
        {
            return base.Group_by_multiple_aggregate_joining_different_tables(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Group_by_multiple_aggregate_joining_different_tables_with_query_filter(bool async)
        {
            return base.Group_by_multiple_aggregate_joining_different_tables_with_query_filter(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Aggregate_over_subquery_in_group_by_projection(bool async)
        {
            return base.Aggregate_over_subquery_in_group_by_projection(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Aggregate_over_subquery_in_group_by_projection_2(bool async)
        {
            return base.Aggregate_over_subquery_in_group_by_projection_2(async);
        }

        [ConditionalTheory(Skip = "SingleStore does not support this type of query: scalar subselect references field belonging to outer select that is more than one level up")]
        public override Task Group_by_aggregate_in_subquery_projection_after_group_by(bool async)
        {
            return base.Group_by_aggregate_in_subquery_projection_after_group_by(async);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Bool_discriminator_column_works(bool async)
        {
            var contextFactory = await InitializeAsync<Context24657>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Blog>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Author>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });

            using var context = contextFactory.CreateContext();

            var query = context.Authors.Include(e => e.Blog);

            var authors = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Equal(2, authors.Count);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task GroupBy_Aggregate_over_navigations_repeated(bool async)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }

            var contextFactory = await InitializeAsync<Context27083>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Customer>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Order>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Project>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<TimeSheet>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            var query = context
                .Set<TimeSheet>()
                .Where(x => x.OrderId != null)
                .GroupBy(x => x.OrderId)
                .Select(x => new
                {
                    HourlyRate = x.Min(f => f.Order.HourlyRate),
                    CustomerId = x.Min(f => f.Project.Customer.Id),
                    CustomerName = x.Min(f => f.Project.Customer.Name),
                });

            var timeSheets = async
                ? await query.ToListAsync()
                : query.ToList();

            Assert.Equal(2, timeSheets.Count);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Unwrap_convert_node_over_projection_when_translating_contains_over_subquery(bool async)
        {
            var contextFactory = await InitializeAsync<Context26593>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Group>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Membership>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            var currentUserId = 1;

            var currentUserGroupIds = context.Memberships
                .Where(m => m.UserId == currentUserId)
                .Select(m => m.GroupId);

            var hasMembership = context.Memberships
                .Where(m => currentUserGroupIds.Contains(m.GroupId))
                .Select(m => m.User);

            var query = context.Users
                .Select(u => new
                {
                    HasAccess = hasMembership.Contains(u)
                });

            var users = async
                ? await query.ToListAsync()
                : query.ToList();
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Unwrap_convert_node_over_projection_when_translating_contains_over_subquery_2(bool async)
        {
            var contextFactory = await InitializeAsync<Context26593>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Group>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Membership>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            var currentUserId = 1;

            var currentUserGroupIds = context.Memberships
                .Where(m => m.UserId == currentUserId)
                .Select(m => m.Group);

            var hasMembership = context.Memberships
                .Where(m => currentUserGroupIds.Contains(m.Group))
                .Select(m => m.User);

            var query = context.Users
                .Select(u => new
                {
                    HasAccess = hasMembership.Contains(u)
                });

            var users = async
                ? await query.ToListAsync()
                : query.ToList();
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task Unwrap_convert_node_over_projection_when_translating_contains_over_subquery_3(bool async)
        {
            var contextFactory = await InitializeAsync<Context26593>(seed: c => c.Seed(), onModelCreating: modelBuilder =>
            {
                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Group>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Membership>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
            });
            using var context = contextFactory.CreateContext();

            var currentUserId = 1;

            var currentUserGroupIds = context.Memberships
                .Where(m => m.UserId == currentUserId)
                .Select(m => m.GroupId);

            var hasMembership = context.Memberships
                .Where(m => currentUserGroupIds.Contains(m.GroupId))
                .Select(m => m.User);

            var query = context.Users
                .Select(u => new
                {
                    HasAccess = hasMembership.Any(e => e == u)
                });

            var users = async
                ? await query.ToListAsync()
                : query.ToList();
        }
    }
}
