using System.Reflection;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class TwoDatabasesSingleStoreTest : TwoDatabasesTestBase, IClassFixture<SingleStoreFixture>
    {
        public TwoDatabasesSingleStoreTest(SingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected new SingleStoreFixture Fixture
            => (SingleStoreFixture)base.Fixture;

        protected override DbContextOptionsBuilder CreateTestOptions(
            DbContextOptionsBuilder optionsBuilder,
            bool withConnectionString = false,
            bool withNullConnectionString = false)
            => withConnectionString
                ? withNullConnectionString
                    ? optionsBuilder.UseSingleStore((string)null)
                    : optionsBuilder.UseSingleStore(DummyConnectionString)
                : optionsBuilder.UseSingleStore();

        protected override TwoDatabasesWithDataContext CreateBackingContext(string databaseName)
            => new TwoDatabasesWithDataContext(Fixture.CreateOptions(SingleStoreTestStore.Create(databaseName)));

        protected static string program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        protected override string DummyConnectionString { get; } = $"Server=localhost;Database=DoesNotExist;Allow User Variables=True;Connection Attributes=\"_connector_name:SingleStore Entity Framework Core provider,_connector_version:{program_version}\";Use Affected Rows=False";
    }
}
