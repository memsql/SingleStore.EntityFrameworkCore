using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit
{
    public class SingleStoreXunitTestCaseRunner : XunitTestCaseRunner
    {
        public SingleStoreXunitTestCaseRunner(
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                testMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override XunitTestRunner CreateTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            => new SingleStoreXunitTestRunner(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                beforeAfterAttributes,
                new ExceptionAggregator(aggregator),
                cancellationTokenSource);

        /// <remarks>
        /// `TestRunner&lt;TTestCase&gt;.RunAsync()` is not virtual, so we need to override this method here to call our own
        /// `SingleStoreXunitTestRunner.RunAsync()` implementation.
        /// </remarks>>
        protected override Task<RunSummary> RunTestAsync()
            => ((SingleStoreXunitTestRunner)CreateTestRunner(
                    CreateTest(TestCase, DisplayName),
                    MessageBus,
                    TestClass,
                    ConstructorArguments,
                    TestMethod,
                    TestMethodArguments,
                    SkipReason,
                    BeforeAfterAttributes,
                    Aggregator,
                    CancellationTokenSource))
                .RunAsync();
    }
}
