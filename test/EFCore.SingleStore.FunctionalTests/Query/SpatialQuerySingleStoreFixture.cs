using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public class SpatialQuerySingleStoreFixture : SpatialQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => SingleStoreTestStoreFactory.Instance;

        protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            => base.AddServices(serviceCollection)
                .AddEntityFrameworkSingleStoreNetTopologySuite();

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new SingleStoreDbContextOptionsBuilder(optionsBuilder)
                .UseNetTopologySuite();

            return optionsBuilder;
        }
    }
}
