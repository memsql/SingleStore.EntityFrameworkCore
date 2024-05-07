using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.ManyToManyModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    // Made internal to skip all tests.
    internal class ManyToManyLoadSingleStoreTest : ManyToManyLoadTestBase<ManyToManyLoadSingleStoreTest.ManyToManyLoadSingleStoreFixture>
    {
        public ManyToManyLoadSingleStoreTest(ManyToManyLoadSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class ManyToManyLoadSingleStoreFixture : ManyToManyLoadFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder
                    .Entity<JoinOneSelfPayload>()
                    .Property(e => e.Payload)
                    .ValueGeneratedOnAdd(); // uses UTC in the original SQL Server implementation

                modelBuilder
                    .SharedTypeEntity<Dictionary<string, object>>("JoinOneToThreePayloadFullShared")
                    .IndexerProperty<string>("Payload")
                    .HasMaxLength(255) // longtext does not support default values
                    .HasDefaultValue("Generated");

                modelBuilder
                    .Entity<JoinOneToThreePayloadFull>()
                    .Property(e => e.Payload)
                    .HasMaxLength(255) // longtext does not support default values
                    .HasDefaultValue("Generated");
            }
        }
    }
}
