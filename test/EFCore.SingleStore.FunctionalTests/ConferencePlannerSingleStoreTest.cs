using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.ConferencePlanner;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class ConferencePlannerSingleStoreTest : ConferencePlannerTestBase<ConferencePlannerSingleStoreTest.ConferencePlannerSingleStoreFixture>
    {
        public ConferencePlannerSingleStoreTest(ConferencePlannerSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public override async Task AttendeesController_AddSession()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.AttendeesController_AddSession();
        }

        public override async Task AttendeesController_Get()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.AttendeesController_Get();
        }

        public override async Task AttendeesController_GetSessions()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.AttendeesController_GetSessions();
        }

        public override async Task AttendeesController_RemoveSession()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.AttendeesController_RemoveSession();
        }

        public override async Task SearchController_Search(
            string searchTerm,
            int totalCount,
            int sessionCount)
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SearchController_Search(searchTerm, totalCount, sessionCount);
        }

        public override async Task SessionsController_Delete()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SessionsController_Delete();
        }

        public override async Task SessionsController_Get()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SessionsController_Get();
        }

        public override async Task SessionsController_Get_with_ID()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SessionsController_Get_with_ID();
        }

        public override async Task SessionsController_Put()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SessionsController_Put();
        }

        public override async Task SpeakersController_GetSpeaker_with_ID()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SpeakersController_GetSpeaker_with_ID();
        }

        public override async Task SpeakersController_GetSpeakers()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.SpeakersController_GetSpeakers();
        }

        public class ConferencePlannerSingleStoreFixture : ConferencePlannerFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // In MySQL 5.6 and lower, unique indices have a smaller max. key size as in later version.
                // This can lead to the following exception being thrown:
                //     Specified key was too long; max key length is 767 bytes
                if (!AppConfig.ServerVersion.Supports.LargerKeyLength)
                {
                    modelBuilder.Entity<Attendee>(entity =>
                    {
                        entity.Property(e => e.UserName)
                            .HasMaxLength(AppConfig.ServerVersion.MaxKeyLength / 4);

                        entity.HasAlternateKey(e => e.UserName);
                    });
                }

                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Attendee>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Speaker>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Track>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Session>()
                    .Property(e => e.Id)
                    .HasColumnType("bigint");

                // Some of the data requires a UTF-8 character set.
                modelBuilder.Entity<Speaker>()
                    .Property(e => e.Name)
                    .HasCharSet(CharSet.Utf8Mb4);
                modelBuilder.Entity<Session>()
                    .Property(e => e.Title)
                    .HasCharSet(CharSet.Utf8Mb4);
            }
        }
    }
}
