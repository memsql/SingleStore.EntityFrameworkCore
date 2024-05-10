using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System.Threading.Tasks;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.TestModels.MusicStore;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class MusicStoreSingleStoreTest : MusicStoreTestBase<MusicStoreSingleStoreTest.MusicStoreSingleStoreFixture>
    {
        public MusicStoreSingleStoreTest(MusicStoreSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact(Skip = "SingleStore doesn't support correlated subselect in ORDER BY")]
        public override async Task Index_GetsSixTopAlbums()
        {
            await base.Index_GetsSixTopAlbums();
        }

        public override async Task AddressAndPayment_RedirectToCompleteWhenSuccessful()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.AddressAndPayment_RedirectToCompleteWhenSuccessful();
        }

        public override async Task AddressAndPayment_ReturnsOrderIfInvalidPromoCode()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.AddressAndPayment_ReturnsOrderIfInvalidPromoCode();
        }

        public override async Task Can_add_items_to_cart()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.Can_add_items_to_cart();
        }

        public override async Task Cart_has_items_once_they_have_been_added()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.Cart_has_items_once_they_have_been_added();
        }

        public override async Task CartSummaryComponent_returns_items()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.CartSummaryComponent_returns_items();
        }

        public override async Task Complete_ReturnsOrderIdIfValid()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.Complete_ReturnsOrderIdIfValid();
        }

        public override async Task Details_ReturnsAlbumDetail()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.Details_ReturnsAlbumDetail();
        }

        public override async Task Browse_ReturnsViewWithGenre()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.Browse_ReturnsViewWithGenre();
        }

        public override void Music_store_project_to_mapped_entity()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            base.Music_store_project_to_mapped_entity();
        }

        public override async Task RemoveFromCart_removes_items_from_cart()
        {
            // We're skipping this test when we're running tests on Managed Service due to the specifics of
            // how AUTO_INCREMENT works (https://docs.singlestore.com/cloud/reference/sql-reference/data-definition-language-ddl/create-table/#auto-increment-behavior)
            if (AppConfig.ManagedService)
            {
                return;
            }
            await base.RemoveFromCart_removes_items_from_cart();
        }

    public class MusicStoreSingleStoreFixture : MusicStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
                // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
                modelBuilder.Entity<Artist>()
                    .Property(e => e.ArtistId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Genre>()
                    .Property(e => e.GenreId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Order>()
                    .Property(e => e.OrderId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<Album>()
                    .Property(e => e.AlbumId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<CartItem>()
                    .Property(e => e.CartItemId)
                    .HasColumnType("bigint");
                modelBuilder.Entity<OrderDetail>()
                    .Property(e => e.OrderDetailId)
                    .HasColumnType("bigint");

                SingleStoreTestHelpers.Instance.EnsureSufficientKeySpace(modelBuilder.Model, TestStore);
            }
        }
    }
}
