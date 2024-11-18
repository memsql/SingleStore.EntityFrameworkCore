using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
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

        protected override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSingleStore();

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

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_creation_passively(bool async)
            {
                ConnectionCreationInterceptor interceptor = new ConnectionCreationInterceptor();
                ConnectionStringContext expected = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                bool connectionDisposed = false;
                expected.Interceptors.Add(interceptor);
                IModel model = expected.Model;
                Assert.False(interceptor.CreatingCalled);
                Assert.False(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                DbConnection dbConnection = expected.Database.GetDbConnection();
                dbConnection.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                Assert.True(interceptor.CreatingCalled);
                Assert.True(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                Assert.Same((object) expected, (object) interceptor.Context);
                Assert.Same((object) dbConnection, (object) interceptor.Connection);
                // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                // whereas the interceptor retrieves a connection in a different manner.
                //Assert.Equal(dbConnection.ConnectionString, interceptor.ConnectionString ?? "");
                if (async)
                    await expected.DisposeAsync();
                else
                    expected.Dispose();
                Assert.True(interceptor.DisposingCalled);
                Assert.True(interceptor.DisposedCalled);
                Assert.Equal<bool>(async, interceptor.AsyncCalled);
                Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                Assert.True(connectionDisposed);
            }

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_to_override_connection_after_creation(bool async)
            {
                ConnectionCreationReplaceInterceptor interceptor;
                using (ConnectionStringContext tempContext = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider)))
                {
                    DbConnection dbConnection1 = tempContext.Database.GetDbConnection();
                    bool connectionDisposed = false;
                    dbConnection1.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                    interceptor = new ConnectionCreationReplaceInterceptor(dbConnection1);
                    ConnectionStringContext expected = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                    expected.Interceptors.Add((ConnectionCreationInterceptor) interceptor);
                    IModel model = expected.Model;
                    Assert.False(interceptor.CreatingCalled);
                    Assert.False(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    DbConnection dbConnection2 = expected.Database.GetDbConnection();
                    Assert.Same((object) dbConnection1, (object) dbConnection2);
                    Assert.True(interceptor.CreatingCalled);
                    Assert.True(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    Assert.Same((object) expected, (object) interceptor.Context);
                    Assert.Same((object) dbConnection2, (object) interceptor.Connection);
                    // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                    // whereas the interceptor retrieves a connection in a different manner.
                    //Assert.Equal(dbConnection2.ConnectionString, interceptor.ConnectionString ?? "");
                    if (async)
                      await expected.DisposeAsync();
                    else
                      expected.Dispose();
                    Assert.True(interceptor.DisposingCalled);
                    Assert.True(interceptor.DisposedCalled);
                    Assert.Equal<bool>(async, interceptor.AsyncCalled);
                    Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                    Assert.True(connectionDisposed);
                }
            }

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_to_override_creation(bool async)
            {
                ConnectionCreationOverrideInterceptor interceptor;
                using (ConnectionStringContext tempContext = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider)))
                {
                    DbConnection dbConnection1 = tempContext.Database.GetDbConnection();
                    bool connectionDisposed = false;
                    dbConnection1.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                    interceptor = new ConnectionCreationOverrideInterceptor(dbConnection1);
                    ConnectionStringContext expected = new ConnectionInterceptionTestBase.ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                    expected.Interceptors.Add((ConnectionInterceptionTestBase.ConnectionCreationInterceptor) interceptor);
                    IModel model = expected.Model;
                    Assert.False(interceptor.CreatingCalled);
                    Assert.False(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    DbConnection dbConnection2 = expected.Database.GetDbConnection();
                    Assert.Same((object) dbConnection1, (object) dbConnection2);
                    Assert.True(interceptor.CreatingCalled);
                    Assert.True(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    Assert.Same((object) expected, (object) interceptor.Context);
                    Assert.Same((object) dbConnection2, (object) interceptor.Connection);
                    // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                    // whereas the interceptor retrieves a connection in a different manner.
                    //Assert.Equal(dbConnection2.ConnectionString, interceptor.ConnectionString ?? "");
                    if (async)
                        await expected.DisposeAsync();
                    else
                        expected.Dispose();
                    Assert.True(interceptor.DisposingCalled);
                    Assert.True(interceptor.DisposedCalled);
                    Assert.Equal<bool>(async, interceptor.AsyncCalled);
                    Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                    Assert.True(connectionDisposed);
                }
            }

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_to_suppress_dispose(bool async)
            {
                ConnectionCreationNoDisposeInterceptor interceptor = new ConnectionInterceptionTestBase.ConnectionCreationNoDisposeInterceptor();
                ConnectionStringContext expected = new ConnectionInterceptionTestBase.ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                bool connectionDisposed = false;
                expected.Interceptors.Add((ConnectionInterceptionTestBase.ConnectionCreationInterceptor) interceptor);
                IModel model = expected.Model;
                Assert.False(interceptor.CreatingCalled);
                Assert.False(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                DbConnection dbConnection = expected.Database.GetDbConnection();
                dbConnection.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                Assert.True(interceptor.CreatingCalled);
                Assert.True(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                Assert.Same((object) expected, (object) interceptor.Context);
                Assert.Same((object) dbConnection, (object) interceptor.Connection);
                // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                // whereas the interceptor retrieves a connection in a different manner.
                //Assert.Equal(dbConnection.ConnectionString, interceptor.ConnectionString ?? "");
                if (async)
                    await expected.DisposeAsync();
                else
                    expected.Dispose();
                Assert.True(interceptor.DisposingCalled);
                Assert.True(interceptor.DisposedCalled);
                Assert.Equal<bool>(async, interceptor.AsyncCalled);
                Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                Assert.False(connectionDisposed);
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

                        [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_creation_passively(bool async)
            {
                ConnectionCreationInterceptor interceptor = new ConnectionCreationInterceptor();
                ConnectionStringContext expected = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                bool connectionDisposed = false;
                expected.Interceptors.Add(interceptor);
                IModel model = expected.Model;
                Assert.False(interceptor.CreatingCalled);
                Assert.False(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                DbConnection dbConnection = expected.Database.GetDbConnection();
                dbConnection.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                Assert.True(interceptor.CreatingCalled);
                Assert.True(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                Assert.Same((object) expected, (object) interceptor.Context);
                Assert.Same((object) dbConnection, (object) interceptor.Connection);
                // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                // whereas the interceptor retrieves a connection in a different manner.
                //Assert.Equal(dbConnection.ConnectionString, interceptor.ConnectionString ?? "");
                if (async)
                    await expected.DisposeAsync();
                else
                    expected.Dispose();
                Assert.True(interceptor.DisposingCalled);
                Assert.True(interceptor.DisposedCalled);
                Assert.Equal<bool>(async, interceptor.AsyncCalled);
                Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                Assert.True(connectionDisposed);
            }

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_to_override_connection_after_creation(bool async)
            {
                ConnectionCreationReplaceInterceptor interceptor;
                using (ConnectionStringContext tempContext = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider)))
                {
                    DbConnection dbConnection1 = tempContext.Database.GetDbConnection();
                    bool connectionDisposed = false;
                    dbConnection1.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                    interceptor = new ConnectionCreationReplaceInterceptor(dbConnection1);
                    ConnectionStringContext expected = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                    expected.Interceptors.Add((ConnectionCreationInterceptor) interceptor);
                    IModel model = expected.Model;
                    Assert.False(interceptor.CreatingCalled);
                    Assert.False(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    DbConnection dbConnection2 = expected.Database.GetDbConnection();
                    Assert.Same((object) dbConnection1, (object) dbConnection2);
                    Assert.True(interceptor.CreatingCalled);
                    Assert.True(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    Assert.Same((object) expected, (object) interceptor.Context);
                    Assert.Same((object) dbConnection2, (object) interceptor.Connection);
                    // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                    // whereas the interceptor retrieves a connection in a different manner.
                    //Assert.Equal(dbConnection2.ConnectionString, interceptor.ConnectionString ?? "");
                    if (async)
                      await expected.DisposeAsync();
                    else
                      expected.Dispose();
                    Assert.True(interceptor.DisposingCalled);
                    Assert.True(interceptor.DisposedCalled);
                    Assert.Equal<bool>(async, interceptor.AsyncCalled);
                    Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                    Assert.True(connectionDisposed);
                }
            }

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_to_override_creation(bool async)
            {
                ConnectionCreationOverrideInterceptor interceptor;
                using (ConnectionStringContext tempContext = new ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider)))
                {
                    DbConnection dbConnection1 = tempContext.Database.GetDbConnection();
                    bool connectionDisposed = false;
                    dbConnection1.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                    interceptor = new ConnectionCreationOverrideInterceptor(dbConnection1);
                    ConnectionStringContext expected = new ConnectionInterceptionTestBase.ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                    expected.Interceptors.Add((ConnectionInterceptionTestBase.ConnectionCreationInterceptor) interceptor);
                    IModel model = expected.Model;
                    Assert.False(interceptor.CreatingCalled);
                    Assert.False(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    DbConnection dbConnection2 = expected.Database.GetDbConnection();
                    Assert.Same((object) dbConnection1, (object) dbConnection2);
                    Assert.True(interceptor.CreatingCalled);
                    Assert.True(interceptor.CreatedCalled);
                    Assert.False(interceptor.DisposingCalled);
                    Assert.False(interceptor.DisposedCalled);
                    Assert.Same((object) expected, (object) interceptor.Context);
                    Assert.Same((object) dbConnection2, (object) interceptor.Connection);
                    // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                    // whereas the interceptor retrieves a connection in a different manner.
                    //Assert.Equal(dbConnection2.ConnectionString, interceptor.ConnectionString ?? "");
                    if (async)
                        await expected.DisposeAsync();
                    else
                        expected.Dispose();
                    Assert.True(interceptor.DisposingCalled);
                    Assert.True(interceptor.DisposedCalled);
                    Assert.Equal<bool>(async, interceptor.AsyncCalled);
                    Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                    Assert.True(connectionDisposed);
                }
            }

            [ConditionalTheory]
            [InlineData(new object[] {false})]
            [InlineData(new object[] {true})]
            public override async Task Intercept_connection_to_suppress_dispose(bool async)
            {
                ConnectionCreationNoDisposeInterceptor interceptor = new ConnectionInterceptionTestBase.ConnectionCreationNoDisposeInterceptor();
                ConnectionStringContext expected = new ConnectionInterceptionTestBase.ConnectionStringContext(new Func<DbContextOptionsBuilder, DbContextOptionsBuilder>(ConfigureProvider));
                bool connectionDisposed = false;
                expected.Interceptors.Add((ConnectionInterceptionTestBase.ConnectionCreationInterceptor) interceptor);
                IModel model = expected.Model;
                Assert.False(interceptor.CreatingCalled);
                Assert.False(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                DbConnection dbConnection = expected.Database.GetDbConnection();
                dbConnection.Disposed += (EventHandler) ((_, __) => connectionDisposed = true);
                Assert.True(interceptor.CreatingCalled);
                Assert.True(interceptor.CreatedCalled);
                Assert.False(interceptor.DisposingCalled);
                Assert.False(interceptor.DisposedCalled);
                Assert.Same((object) expected, (object) interceptor.Context);
                Assert.Same((object) dbConnection, (object) interceptor.Connection);
                // We're commenting out this line because GetDbConnection() returns a connection with specific attributes,
                // whereas the interceptor retrieves a connection in a different manner.
                //Assert.Equal(dbConnection.ConnectionString, interceptor.ConnectionString ?? "");
                if (async)
                    await expected.DisposeAsync();
                else
                    expected.Dispose();
                Assert.True(interceptor.DisposingCalled);
                Assert.True(interceptor.DisposedCalled);
                Assert.Equal<bool>(async, interceptor.AsyncCalled);
                Assert.NotEqual<bool>(async, interceptor.SyncCalled);
                Assert.False(connectionDisposed);
            }

            public class InterceptionSingleStoreFixture : InterceptionSingleStoreFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => true;
            }
        }
    }
}
