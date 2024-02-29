using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

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
        : SingleStoreTestFixtureBase
    where TContext : ContextBase, new()
    {
        private const string FixtureSuffix = "Fixture";

        public SingleStoreTestFixtureBase(bool initializeEmpty = false)
        {
            // We branch here, because CreateDefaultDbContext depends on TestStore.Name by default, which would not be available yet in
            // the SingleStoreTestStore.RecreateInitialized(StoreName) call.
            if (initializeEmpty)
            {
                TestStore = SingleStoreTestStore.RecreateInitialized(StoreName);
            }
            else
            {
                TestStore = SingleStoreTestStore.Create(StoreName);

                TestStore.InitializeSingleStore(null, CreateDefaultDbContext, null, c =>
                {
                    c.Database.EnsureDeleted();
                    c.Database.EnsureCreated();
                });
            }

            SetupDatabase();
        }

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

        protected virtual SingleStoreTestStore TestStore { get; }
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
