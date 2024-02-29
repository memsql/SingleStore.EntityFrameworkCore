using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public abstract class ConnectionInterceptionSingleStoreTestBase : ConnectionInterceptionTestBase
    {
        protected ConnectionInterceptionSingleStoreTestBase(InterceptionSingleStoreFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionSingleStoreFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "ConnectionInterception";
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkSingleStore(), injectedInterceptors);
        }

        protected override BadUniverseContext CreateBadUniverse(DbContextOptionsBuilder optionsBuilder)
            => new BadUniverseContext(optionsBuilder.UseSingleStore(new FakeDbConnection()).Options);

        public class FakeDbConnection : DbConnection
        {
            public override string ConnectionString { get; set; }
            public override string Database => "Database";
            public override string DataSource => "DataSource";
            public override string ServerVersion => throw new NotImplementedException();
            public override ConnectionState State => ConnectionState.Closed;
            public override void ChangeDatabase(string databaseName) => throw new NotImplementedException();
            public override void Close() => throw new NotImplementedException();
            public override void Open() => throw new NotImplementedException();
            protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel) => throw new NotImplementedException();
            protected override DbCommand CreateDbCommand() => throw new NotImplementedException();
        }

        public class ConnectionInterceptionSingleStoreTest
            : ConnectionInterceptionSingleStoreTestBase, IClassFixture<ConnectionInterceptionSingleStoreTest.InterceptionSingleStoreFixture>
        {
            public ConnectionInterceptionSingleStoreTest(InterceptionSingleStoreFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;
            }
        }

        public class ConnectionInterceptionWithDiagnosticsSingleStoreTest
            : ConnectionInterceptionSingleStoreTestBase, IClassFixture<ConnectionInterceptionWithDiagnosticsSingleStoreTest.InterceptionSingleStoreFixture>
        {
            public ConnectionInterceptionWithDiagnosticsSingleStoreTest(InterceptionSingleStoreFixture fixture)
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
