using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public class SingleStoreTestStoreFactory : RelationalTestStoreFactory
    {
        public static SingleStoreTestStoreFactory Instance => InstanceCi;
        public static SingleStoreTestStoreFactory InstanceCi { get; } = new SingleStoreTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CiCollation);
        public static SingleStoreTestStoreFactory InstanceCs { get; } = new SingleStoreTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CsCollation);

        public static SingleStoreTestStoreFactory NoBackslashEscapesInstance { get; } = new SingleStoreTestStoreFactory(noBackslashEscapes: true);
        public static SingleStoreTestStoreFactory GuidBinary16Instance { get; } = new SingleStoreTestStoreFactory(guidFormat: SingleStoreGuidFormat.Binary16);

        protected bool NoBackslashEscapes { get; }
        protected string DatabaseCollation { get; }
        protected SingleStoreGuidFormat GuidFormat { get; }

        protected SingleStoreTestStoreFactory(
            bool noBackslashEscapes = false,
            string databaseCollation = null,
            SingleStoreGuidFormat guidFormat = SingleStoreGuidFormat.Default)
        {
            NoBackslashEscapes = noBackslashEscapes;
            DatabaseCollation = databaseCollation;
            GuidFormat = guidFormat;
        }

        public override TestStore Create(string storeName)
            => SingleStoreTestStore.Create(storeName, noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation, guidFormat: GuidFormat);

        public override TestStore GetOrCreate(string storeName)
            => SingleStoreTestStore.GetOrCreate(storeName, noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation, guidFormat: GuidFormat);

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkSingleStore();
    }
}
