using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.ManyToManyFieldsModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    // Made internal to skip all tests.
    internal class ManyToManyFieldsLoadSingleStoreTest : ManyToManyFieldsLoadTestBase<
        ManyToManyFieldsLoadSingleStoreTest.ManyToManyFieldsLoadSingleStoreFixture>
    {
        public ManyToManyFieldsLoadSingleStoreTest(ManyToManyFieldsLoadSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        public class ManyToManyFieldsLoadSingleStoreFixture : ManyToManyFieldsLoadFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            protected override ITestStoreFactory TestStoreFactory
                => SingleStoreTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder
                    .Entity<JoinOneSelfPayload>()
                    .Property(e => e.Payload)
                    .ValueGeneratedOnAdd();

                modelBuilder
                    .SharedTypeEntity<Dictionary<string, object>>("JoinOneToThreePayloadFullShared")
                    .IndexerProperty<string>("Payload")
                        .HasMaxLength(255)
                    .HasDefaultValue("Generated");

                modelBuilder
                    .Entity<JoinOneToThreePayloadFull>()
                    .Property(e => e.Payload)
                        .HasMaxLength(255)
                    .HasDefaultValue("Generated");
            }
        }
    }
}
