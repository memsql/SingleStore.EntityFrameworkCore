using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Extensions.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonMicrosoftStringChangeTrackingTest : JsonStringChangeTrackingTestBase<JsonMicrosoftStringChangeTrackingTest.JsonMicrosoftStringChangeTrackingFixture>
    {
        public JsonMicrosoftStringChangeTrackingTest(JsonMicrosoftStringChangeTrackingFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public class JsonMicrosoftStringChangeTrackingFixture : JsonStringChangeTrackingFixtureBase
        {
            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkSingleStoreJsonMicrosoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder, SingleStoreJsonChangeTrackingOptions? changeTrackingOptions)
            {
                builder = base.AddOptions(builder, changeTrackingOptions);

                if (changeTrackingOptions != null)
                {
                    new SingleStoreDbContextOptionsBuilder(builder)
                        .UseMicrosoftJson(changeTrackingOptions.Value);
                }

                return builder;
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                var currentContext = (JsonStringChangeTrackingContext)context;
                if (currentContext.ChangeTrackingOptions != null &&
                    !currentContext.ChangeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    modelBuilder.Entity<JsonEntity>(
                        entity =>
                        {
                            entity.Property(e => e.Customer)
                                .UseJsonChangeTrackingOptions(currentContext.ChangeTrackingOptions.Value.ChangeTrackingOptions);
                        });
                }
            }
        }
    }
}
