using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore
{
    public class TestBase<TContext> : IDisposable, IAsyncLifetime
        where TContext : ContextBase, new()
    {
        public async Task InitializeAsync()
        {
            TestStore = await SingleStoreTestStore.CreateInitializedAsync(StoreName);
        }

        public Task DisposeAsync()
            => Task.CompletedTask;

        public virtual void Dispose() => TestStore.Dispose();

        public virtual string StoreName => GetType().Name;
        public virtual SingleStoreTestStore TestStore { get; private set; }
        public virtual List<string> SqlCommands { get; } = new List<string>();
        public virtual string Sql => string.Join("\n\n", SqlCommands);

        public virtual async Task<TContext> CreateContext(Action<SingleStoreDbContextOptionsBuilder> jetOptions = null,
            Action<IServiceProvider, DbContextOptionsBuilder> options = null,
            Action<ModelBuilder> model = null)
        {
            var context = new TContext();

            context.Initialize(
                TestStore.Name,
                command => SqlCommands.Add(command.CommandText),
                model: model,
                options: options,
                mySqlOptions: jetOptions);

            await TestStore.CleanAsync(context);

            return context;
        }
    }
}
