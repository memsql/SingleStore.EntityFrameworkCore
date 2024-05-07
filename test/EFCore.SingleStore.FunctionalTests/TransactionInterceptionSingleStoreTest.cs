using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public abstract class TransactionInterceptionSingleStoreTestBase : TransactionInterceptionTestBase
    {
        protected TransactionInterceptionSingleStoreTestBase(InterceptionSingleStoreFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionSingleStoreFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "TransactionInterception";
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkSingleStore(), injectedInterceptors);
        }

        // Made internal to skip all tests.
        internal class TransactionInterceptionSingleStoreTest
            : TransactionInterceptionSingleStoreTestBase, IClassFixture<TransactionInterceptionSingleStoreTest.InterceptionSingleStoreFixture>
        {
            public TransactionInterceptionSingleStoreTest(InterceptionSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;
            }
        }

        // Made internal to skip all tests.
        internal class TransactionInterceptionWithDiagnosticsSingleStoreTest
            : TransactionInterceptionSingleStoreTestBase, IClassFixture<TransactionInterceptionWithDiagnosticsSingleStoreTest.InterceptionSingleStoreFixture>
        {
            public TransactionInterceptionWithDiagnosticsSingleStoreTest(InterceptionSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => true;
            }
        }
    }
}
