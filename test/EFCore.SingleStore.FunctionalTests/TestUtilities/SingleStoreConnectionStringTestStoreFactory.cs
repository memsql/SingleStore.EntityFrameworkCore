using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public class SingleStoreConnectionStringTestStoreFactory : RelationalTestStoreFactory
    {
        public static SingleStoreConnectionStringTestStoreFactory Instance { get; } = new SingleStoreConnectionStringTestStoreFactory();

        protected SingleStoreConnectionStringTestStoreFactory()
        {
        }

        public override TestStore Create(string storeName)
            => SingleStoreTestStore.Create(storeName, useConnectionString: true);

        public override TestStore GetOrCreate(string storeName)
            => SingleStoreTestStore.GetOrCreate(storeName, useConnectionString: true);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkSingleStore();
    }
}
