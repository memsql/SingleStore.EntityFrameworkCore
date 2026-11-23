using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore
{
    public abstract class SingleStoreTestFixtureBase : IAsyncDisposable
    {
        public abstract void SetupDatabase();
        public abstract DbContext CreateDefaultDbContext();

        protected virtual ValueTask DisposeAsync(bool disposing)
        {
            return ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }
    }

    public class SingleStoreTestFixtureBase<TContext>
        : SingleStoreTestFixtureBase, IAsyncLifetime
    where TContext : ContextBase, new()
    {
        private readonly bool _initializeEmpty;
        private const string FixtureSuffix = "Fixture";

        public SingleStoreTestFixtureBase(bool initializeEmpty = false)
        {
            _initializeEmpty = initializeEmpty;
        }

        public async Task InitializeAsync()
        {
            // We branch here, because CreateDefaultDbContext depends on TestStore.Name by default, which would not be available yet in
            // the SingleStoreTestStore.RecreateInitialized(StoreName) call.
            if (_initializeEmpty)
            {
                TestStore = await SingleStoreTestStore.RecreateInitializedAsync(StoreName);
            }
            else
            {
                TestStore = SingleStoreTestStore.Create(StoreName);

                await TestStore.InitializeSingleStoreAsync(null, CreateDefaultDbContext, null, async c =>
                {
                    await c.Database.EnsureDeletedAsync();
                    await c.Database.EnsureCreatedAsync();
                });
            }

            SetupDatabase();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await DisposeAsync(true);
        }

        protected override async ValueTask DisposeAsync(bool disposing)
        {
            await TestStore.DisposeAsync();
            await base.DisposeAsync(disposing);
        }

        protected virtual string StoreName
        {
            get
            {
                var typeName = GetType().Name;
                return typeName.EndsWith(FixtureSuffix)
                    ? typeName.Substring(0, typeName.Length - FixtureSuffix.Length)
                    : typeName;
            }
        }

        protected virtual SingleStoreTestStore TestStore { get; private set; }
        protected virtual string SetupDatabaseScript { get; }
        protected virtual List<string> SqlCommands { get; } = new List<string>();
        protected virtual string Sql => string.Join("\n\n", SqlCommands);

        public virtual TContext CreateContext(
            Action<SingleStoreDbContextOptionsBuilder> mySqlOptions = null,
            Action<IServiceProvider, DbContextOptionsBuilder> options = null,
            Action<ModelBuilder> model = null,
            Action<IServiceCollection> serviceCollection = null,
            string databaseName = null)
        {
            var context = new TContext();

            var collection = new ServiceCollection()
                .AddEntityFrameworkSingleStore();

            serviceCollection?.Invoke(collection);

            context.Initialize(
                databaseName ?? TestStore.Name,
                command => SqlCommands.Add(command.CommandText),
                model,
                options,
                collection,
                mySqlOptions);

            return context;
        }

        public override DbContext CreateDefaultDbContext()
            => CreateContext();

        public override void SetupDatabase()
        {
            if (!string.IsNullOrEmpty(SetupDatabaseScript))
            {
                TestStore.ExecuteScript(SetupDatabaseScript);
            }
        }
    }
}
