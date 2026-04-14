using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests.
    internal abstract class ProxyGraphUpdatesSingleStoreTest
    {
        public abstract class ProxyGraphUpdatesSingleStoreTestBase<TFixture> : ProxyGraphUpdatesTestBase<TFixture>
            where TFixture : ProxyGraphUpdatesSingleStoreTestBase<TFixture>.ProxyGraphUpdatesSingleStoreFixtureBase, new()
        {
            protected ProxyGraphUpdatesSingleStoreTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class ProxyGraphUpdatesSingleStoreFixtureBase : ProxyGraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory
                    => (TestSqlLoggerFactory)ListLoggerFactory;

                protected override ITestStoreFactory TestStoreFactory
                    => SingleStoreTestStoreFactory.Instance;
            }
        }

        public class LazyLoading : ProxyGraphUpdatesSingleStoreTestBase<LazyLoading.ProxyGraphUpdatesWithLazyLoadingSingleStoreFixture>
        {
            public LazyLoading(ProxyGraphUpdatesWithLazyLoadingSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            protected override bool DoesLazyLoading
                => true;

            protected override bool DoesChangeTracking
                => false;

            public class ProxyGraphUpdatesWithLazyLoadingSingleStoreFixture : ProxyGraphUpdatesSingleStoreFixtureBase
            {
                protected override string StoreName { get; } = "ProxyGraphLazyLoadingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseLazyLoadingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());
            }
        }

        public class ChangeTracking : ProxyGraphUpdatesSingleStoreTestBase<ChangeTracking.ProxyGraphUpdatesWithChangeTrackingSingleStoreFixture>
        {
            public ChangeTracking(ProxyGraphUpdatesWithChangeTrackingSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            // Needs lazy loading
            public override Task Save_two_entity_cycle_with_lazy_loading()
                => Task.CompletedTask;

            protected override bool DoesLazyLoading
                => false;

            protected override bool DoesChangeTracking
                => true;

            public class ProxyGraphUpdatesWithChangeTrackingSingleStoreFixture : ProxyGraphUpdatesSingleStoreFixtureBase
            {
                protected override string StoreName { get; } = "ProxyGraphChangeTrackingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseChangeTrackingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());
            }
        }

        public class ChangeTrackingAndLazyLoading : ProxyGraphUpdatesSingleStoreTestBase<
            ChangeTrackingAndLazyLoading.ProxyGraphUpdatesWithChangeTrackingAndLazyLoadingSingleStoreFixture>
        {
            public ChangeTrackingAndLazyLoading(ProxyGraphUpdatesWithChangeTrackingAndLazyLoadingSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            protected override bool DoesLazyLoading
                => true;

            protected override bool DoesChangeTracking
                => true;

            public class ProxyGraphUpdatesWithChangeTrackingAndLazyLoadingSingleStoreFixture : ProxyGraphUpdatesSingleStoreFixtureBase
            {
                protected override string StoreName { get; } = "ProxyGraphChangeTrackingAndLazyLoadingUpdatesTest";

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                    => base.AddOptions(builder.UseLazyLoadingProxies().UseChangeTrackingProxies());

                protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                    => base.AddServices(serviceCollection.AddEntityFrameworkProxies());
            }
        }
    }
}
