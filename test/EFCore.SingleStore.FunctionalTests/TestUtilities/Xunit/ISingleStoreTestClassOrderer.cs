using System.Collections.Generic;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit;

public interface ISingleStoreTestClassOrderer
{
    IEnumerable<ITestClass> OrderTestClasses(IEnumerable<ITestClass> testClasses);
}
