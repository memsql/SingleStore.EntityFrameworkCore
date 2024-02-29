using System;
using Microsoft.EntityFrameworkCore;
using SingleStoreConnector;
using Xunit;

namespace EntityFrameworkCore.SingleStore
{
    public class RawSqlTestWithFixture<TFixture> : TestWithFixture<TFixture>
        where TFixture : SingleStoreTestFixtureBase
    {
        protected DbContext Context { get; }
        protected SingleStoreConnection Connection { get; }

        protected RawSqlTestWithFixture(TFixture fixture)
            : base(fixture)
        {
            Context = Fixture.CreateDefaultDbContext();
            Context.Database.OpenConnection();

            Connection = (SingleStoreConnection)Context.Database.GetDbConnection();
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }
    }

    public class TestWithFixture<TFixture>
        : IClassFixture<TFixture>, IDisposable
        where TFixture : SingleStoreTestFixtureBase
    {
        protected TestWithFixture(TFixture fixture)
        {
            Fixture = fixture;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TestWithFixture()
        {
            Dispose(false);
        }

        protected TFixture Fixture { get; }
    }
}
