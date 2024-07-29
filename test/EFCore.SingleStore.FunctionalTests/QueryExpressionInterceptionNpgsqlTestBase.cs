using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public abstract class QueryExpressionInterceptionSingleStoreTestBase : QueryExpressionInterceptionTestBase
{
    protected QueryExpressionInterceptionSingleStoreTestBase(InterceptionSingleStoreFixtureBase fixture)
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

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            new SingleStoreDbContextOptionsBuilder(base.AddOptions(builder))
                .ExecutionStrategy(d => new SingleStoreExecutionStrategy(d));
            return builder;
        }
    }

    public class QueryExpressionInterceptionSingleStoreTest
        : QueryExpressionInterceptionSingleStoreTestBase, IClassFixture<QueryExpressionInterceptionSingleStoreTest.InterceptionSingleStoreFixture>
    {
        public QueryExpressionInterceptionSingleStoreTest(InterceptionSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
        {
            protected override string StoreName
                => "QueryExpressionInterception";

            protected override bool ShouldSubscribeToDiagnosticListener
                => false;
        }
    }

    public class QueryExpressionInterceptionWithDiagnosticsSingleStoreTest
        : QueryExpressionInterceptionSingleStoreTestBase,
            IClassFixture<QueryExpressionInterceptionWithDiagnosticsSingleStoreTest.InterceptionSingleStoreFixture>
    {
        public QueryExpressionInterceptionWithDiagnosticsSingleStoreTest(InterceptionSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
        {
            protected override string StoreName
                => "QueryExpressionInterceptionWithDiagnostics";

            protected override bool ShouldSubscribeToDiagnosticListener
                => true;
        }
    }
}
