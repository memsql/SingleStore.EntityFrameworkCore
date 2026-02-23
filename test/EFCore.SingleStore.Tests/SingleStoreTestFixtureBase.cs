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
    public abstract class SingleStoreTestFixtureBase : IDisposable
    {
        public abstract void SetupDatabase();
        public abstract DbContext CreateDefaultDbContext();

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
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

        public Task DisposeAsync()
            => Task.CompletedTask;

        protected override void Dispose(bool disposing)
        {
            TestStore.Dispose();
            base.Dispose(disposing);
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
