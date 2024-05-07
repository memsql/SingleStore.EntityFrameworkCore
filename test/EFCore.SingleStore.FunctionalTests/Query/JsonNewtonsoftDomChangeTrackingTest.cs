using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Extensions.Internal;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonNewtonsoftDomChangeTrackingTest : IClassFixture<JsonNewtonsoftDomChangeTrackingTest.JsonMicrosoftDomChangeTrackingFixture>
    {
        public JsonNewtonsoftDomChangeTrackingTest(JsonMicrosoftDomChangeTrackingFixture fixture)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected JsonMicrosoftDomChangeTrackingFixture Fixture { get; }

        [Fact]
        public void Root_property_changed_CustomerJObject()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerJObject = JsonMicrosoftDomChangeTrackingContext.CreateCustomer2();
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.CustomerJObject).IsModified);
        }

        [Fact]
        public void Root_property_changed_CustomerJToken()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerJToken = JsonMicrosoftDomChangeTrackingContext.CreateCustomer2();
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.CustomerJToken).IsModified);
        }

        [Fact]
        public void Root_property_unchanged_CustomerJObject()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJObject).IsModified);
        }

        [Fact]
        public void Root_property_unchanged_CustomerJToken()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.None);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJToken).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareRootPropertyOnly_CustomerJObject()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomerJObject = x.CustomerJObject;
            var newCustomerJObject = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1();
            x.CustomerJObject = newCustomerJObject;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomerJObject, newCustomerJObject));
            Assert.True(ctx.Entry(x).Property(e => e.CustomerJObject).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareRootPropertyOnly_CustomerJToken()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.CompareRootPropertyOnly);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerJToken = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1();
            ctx.ChangeTracker.DetectChanges();

            Assert.True(ctx.Entry(x).Property(e => e.CustomerJToken).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareDomSemantically_CustomerJObject()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.CompareDomSemantically);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            var oldCustomerJObject = x.CustomerJObject;
            var newCustomerJObject = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1();
            x.CustomerJObject = newCustomerJObject;
            ctx.ChangeTracker.DetectChanges();

            Assert.True(!ReferenceEquals(oldCustomerJObject, newCustomerJObject));
            Assert.False(ctx.Entry(x).Property(e => e.CustomerJObject).IsModified);
        }

        [Fact]
        public void Semantically_equal_with_CompareDomSemantically_CustomerJToken()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.CompareDomSemantically);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            x.CustomerJToken = JsonMicrosoftDomChangeTrackingContext.CreateCustomer1();
            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJToken).IsModified);
        }

        [Fact]
        public void HashDomSemantially_CustomerJObject()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.HashDomSemantially);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJObject).IsModified);
        }

        [Fact]
        public void HashDomSemantially_CustomerJToken()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.HashDomSemantially);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJToken).IsModified);
        }

        [Fact]
        public void HashDomSemantiallyOptimized_CustomerJObject()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.HashDomSemantiallyOptimized);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJObject).IsModified);
        }

        [Fact]
        public void HashDomSemantiallyOptimized_CustomerJToken()
        {
            using var ctx = CreateContext(SingleStoreJsonChangeTrackingOptions.HashDomSemantiallyOptimized);
            var x = ctx.JsonEntities.Single(e => e.Id == 1);

            ctx.ChangeTracker.DetectChanges();

            Assert.False(ctx.Entry(x).Property(e => e.CustomerJToken).IsModified);
        }

        #region Support

        protected JsonMicrosoftDomChangeTrackingContext CreateContext(
            SingleStoreJsonChangeTrackingOptions? changeTrackingOptions = null,
            bool isGlobal = false)
            => Fixture.CreateContext(
                serviceCollection => serviceCollection.Configure<JsonMicrosoftDomChangeTrackingContext.JsonPocoChangeTrackingContextOptions>(
                    options =>
                    {
                        options.ChangeTrackingOptions = changeTrackingOptions;
                        options.AreChangeTrackingOptionsGlobal = isGlobal;
                    })
            );

        public class JsonMicrosoftDomChangeTrackingContext : PoolableDbContext
        {
            private readonly IOptions<JsonPocoChangeTrackingContextOptions> _changeTrackingOptions;
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonMicrosoftDomChangeTrackingContext(
                DbContextOptions<JsonMicrosoftDomChangeTrackingContext> options,
                IOptions<JsonPocoChangeTrackingContextOptions> changeTrackingOptions = null)
                : base(options)
            {
                _changeTrackingOptions = changeTrackingOptions;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (_changeTrackingOptions?.Value.ChangeTrackingOptions != null &&
                    _changeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension((SingleStoreJsonNewtonsoftOptionsExtension)
                        (optionsBuilder.Options.FindExtension<SingleStoreJsonNewtonsoftOptionsExtension>() ??
                         new SingleStoreJsonNewtonsoftOptionsExtension())
                        .WithJsonChangeTrackingOptions(_changeTrackingOptions.Value.ChangeTrackingOptions.Value));
                }
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                if (_changeTrackingOptions != null &&
                    !_changeTrackingOptions.Value.AreChangeTrackingOptionsGlobal)
                {
                    modelBuilder.Entity<JsonEntity>(
                        entity =>
                        {
                            entity.Property(e => e.CustomerJObject)
                                .UseJsonChangeTrackingOptions(_changeTrackingOptions.Value.ChangeTrackingOptions);
                            entity.Property(e => e.CustomerJToken)
                                .UseJsonChangeTrackingOptions(_changeTrackingOptions.Value.ChangeTrackingOptions);
                        });
                }
            }

            public void Seed()
            {
                var (customer1, customer2, customer3) = (CreateCustomer1(), CreateCustomer2(), CreateCustomer3());

                JsonEntities.AddRange(
                    new JsonEntity { Id = 1, CustomerJObject = customer1, CustomerJToken = customer1},
                    new JsonEntity { Id = 2, CustomerJObject = customer2, CustomerJToken = customer2},
                    new JsonEntity { Id = 3, CustomerJObject = null, CustomerJToken = customer3});
                SaveChanges();
            }

            public static JObject CreateCustomer1()
                => JObject.Parse(
                    @"
                {
                    ""Name"": ""Joe"",
                    ""Age"": 25,
                    ""ID"": ""00000000-0000-0000-0000-000000000000"",
                    ""IsVip"": false,
                    ""Statistics"":
                    {
                        ""Visits"": 4,
                        ""Purchases"": 3,
                        ""Nested"":
                        {
                            ""SomeProperty"": 10,
                            ""SomeNullableInt"": 20,
                            ""SomeNullableGuid"": ""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",
                            ""IntArray"": [3, 4]
                        }
                    },
                    ""Orders"":
                    [
                        {
                            ""Price"": 99.5,
                            ""ShippingAddress"": ""Some address 1"",
                            ""ShippingDate"": ""2019-10-01""
                        },
                        {
                            ""Price"": 23.1,
                            ""ShippingAddress"": ""Some address 2"",
                            ""ShippingDate"": ""2019-10-10""
                        }
                    ]
                }");

            public static JObject CreateCustomer2()
                => JObject.Parse(
                    @"
                {
                    ""Name"": ""Moe"",
                    ""Age"": 35,
                    ""ID"": ""3272b593-bfe2-4ecf-81ae-4242b0632465"",
                    ""IsVip"": true,
                    ""Statistics"":
                    {
                        ""Visits"": 20,
                        ""Purchases"": 25,
                        ""Nested"":
                        {
                            ""SomeProperty"": 20,
                            ""SomeNullableInt"": null,
                            ""SomeNullableGuid"": null,
                            ""IntArray"": [5, 6]
                        }
                    },
                    ""Orders"":
                    [
                        {
                            ""Price"": 5,
                            ""ShippingAddress"": ""Moe's address"",
                            ""ShippingDate"": ""2019-11-03""
                        }
                    ]
                }");

            public static JToken CreateCustomer3()
                => JToken.Parse(@"""foo""");

            public class JsonPocoChangeTrackingContextOptions
            {
                public SingleStoreJsonChangeTrackingOptions? ChangeTrackingOptions { get; set; }
                public bool AreChangeTrackingOptionsGlobal { get; set; }
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            public JObject CustomerJObject { get; set; }
            public JToken CustomerJToken { get; set; }
        }

        public class JsonMicrosoftDomChangeTrackingFixture : ServiceProviderPerContextFixtureBase<JsonMicrosoftDomChangeTrackingContext>
        {
            protected override string StoreName => "JsonNewtonsoftDomChangeTrackingTest";
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;
            protected override void Seed(JsonMicrosoftDomChangeTrackingContext context) => context.Seed();

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkSingleStoreJsonNewtonsoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var options = base.AddOptions(builder);
                new SingleStoreDbContextOptionsBuilder(options)
                    .UseNewtonsoftJson();

                return options;
            }
        }

        #endregion
    }
}
