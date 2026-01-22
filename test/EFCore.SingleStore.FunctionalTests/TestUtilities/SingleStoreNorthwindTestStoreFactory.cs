using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public class SingleStoreNorthwindTestStoreFactory : SingleStoreTestStoreFactory
    {
        public const string DefaultNamePrefix = "Northwind";

        public static new SingleStoreNorthwindTestStoreFactory Instance => InstanceCi;
        public static new SingleStoreNorthwindTestStoreFactory InstanceCi { get; } = new SingleStoreNorthwindTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CiCollation);
        public static new SingleStoreNorthwindTestStoreFactory InstanceCs { get; } = new SingleStoreNorthwindTestStoreFactory(databaseCollation: AppConfig.ServerVersion.DefaultUtf8CsCollation);
        public static new SingleStoreNorthwindTestStoreFactory NoBackslashEscapesInstance { get; } = new SingleStoreNorthwindTestStoreFactory(true);

        protected SingleStoreNorthwindTestStoreFactory(bool noBackslashEscapes = false, string databaseCollation = null)
            : base(noBackslashEscapes, databaseCollation)
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => SingleStoreTestStore.GetOrCreate(storeName ?? $"{DefaultNamePrefix}__{DatabaseCollation}", "Northwind.sql", noBackslashEscapes: NoBackslashEscapes, databaseCollation: DatabaseCollation);
    }
}
