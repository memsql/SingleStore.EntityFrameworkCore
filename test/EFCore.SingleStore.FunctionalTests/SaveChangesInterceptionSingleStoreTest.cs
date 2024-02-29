using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public abstract class SaveChangesInterceptionSingleStoreTestBase : SaveChangesInterceptionTestBase
    {
        protected SaveChangesInterceptionSingleStoreTestBase(InterceptionSingleStoreFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionSingleStoreFixtureBase : InterceptionFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkSingleStore(), injectedInterceptors);
        }

        public class SaveChangesInterceptionSingleStoreTest
            : SaveChangesInterceptionSingleStoreTestBase, IClassFixture<SaveChangesInterceptionSingleStoreTest.InterceptionSingleStoreFixture>
        {
            public SaveChangesInterceptionSingleStoreTest(InterceptionSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
            {
                protected override string StoreName
                    => "SaveChangesInterception";

                protected override bool ShouldSubscribeToDiagnosticListener
                    => false;

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                {
                    new SingleStoreDbContextOptionsBuilder(base.AddOptions(builder))
                        .ExecutionStrategy(d => new SingleStoreExecutionStrategy(d));
                    return builder;
                }
            }
        }

        public class SaveChangesInterceptionWithDiagnosticsSingleStoreTest
            : SaveChangesInterceptionSingleStoreTestBase,
                IClassFixture<SaveChangesInterceptionWithDiagnosticsSingleStoreTest.InterceptionSingleStoreFixture>
        {
            public SaveChangesInterceptionWithDiagnosticsSingleStoreTest(InterceptionSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
            {
                protected override string StoreName => "SaveChangesInterceptionWithDiagnostics";

                protected override bool ShouldSubscribeToDiagnosticListener
                    => true;

                public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                {
                    new SingleStoreDbContextOptionsBuilder(base.AddOptions(builder))
                        .ExecutionStrategy(d => new SingleStoreExecutionStrategy(d));
                    return builder;
                }
            }
        }
    }
}
