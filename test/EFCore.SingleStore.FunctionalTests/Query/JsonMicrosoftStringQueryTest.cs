using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonMicrosoftStringQueryTest : JsonStringQueryTestBase<JsonMicrosoftStringQueryTest.JsonMicrosoftStringQueryFixture>
    {
        public JsonMicrosoftStringQueryTest(JsonMicrosoftStringQueryFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public class JsonMicrosoftStringQueryFixture : JsonStringQueryFixtureBase
        {
            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkSingleStoreJsonMicrosoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var options = base.AddOptions(builder);
                new SingleStoreDbContextOptionsBuilder(options)
                    .UseMicrosoftJson();

                return options;
            }
        }
    }
}
