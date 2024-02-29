using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit
{
    public class SingleStoreTestCollectionOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
            => testCollections.OrderBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase);
    }
}
