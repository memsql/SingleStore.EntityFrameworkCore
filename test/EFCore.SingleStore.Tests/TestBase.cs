using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore
{
    public class TestBase<TContext> : IDisposable
        where TContext : ContextBase, new()
    {
        public TestBase()
        {
            TestStore = SingleStoreTestStore.CreateInitialized(StoreName);
        }
        public virtual void Dispose() => TestStore.Dispose();

        public virtual string StoreName => GetType().Name;
        public virtual SingleStoreTestStore TestStore { get; }
        public virtual List<string> SqlCommands { get; } = new List<string>();
        public virtual string Sql => string.Join("\n\n", SqlCommands);

        public virtual TContext CreateContext(
            Action<SingleStoreDbContextOptionsBuilder> jetOptions = null,
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

            TestStore.Clean(context);

            return context;
        }
    }
}
