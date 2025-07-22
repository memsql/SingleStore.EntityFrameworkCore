// #define FIXED_TEST_ORDER

using Xunit;

//
// Optional: Control the test execution order.
//           This can be helpful for diffing etc.
//

#if FIXED_TEST_ORDER

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true, MaxParallelThreads = 1)]
[assembly: TestCollectionOrderer("EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit.SingleStoreTestCollectionOrderer", "EntityFrameworkCore.SingleStore.FunctionalTests")]
[assembly: TestCaseOrderer("EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit.SingleStoreTestCaseOrderer", "EntityFrameworkCore.SingleStore.FunctionalTests")]

#endif

// Our custom SingleStoreXunitTestFrameworkDiscoverer class allows filtering whole classes like SupportedServerVersionConditionAttribute, instead
// of just the test cases. This is necessary, if a fixture is database server version dependent.
[assembly: TestFramework("EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit.SingleStoreXunitTestFramework", "EntityFrameworkCore.SingleStore.FunctionalTests")]
