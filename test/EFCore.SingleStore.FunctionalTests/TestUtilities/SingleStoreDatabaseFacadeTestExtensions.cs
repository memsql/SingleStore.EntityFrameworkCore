using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public static class SingleStoreDatabaseFacadeTestExtensions
    {
        public static void EnsureClean(this DatabaseFacade databaseFacade)
            => new SingleStoreDatabaseCleaner(
                    databaseFacade.GetService<ISingleStoreOptions>(),
                    databaseFacade.GetService<IRelationalTypeMappingSource>())
                .Clean(databaseFacade);
    }
}
