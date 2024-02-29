using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Xunit
{
    public class SingleStoreConditionalTheoryDiscoverer : ConditionalTheoryDiscoverer
    {
        public SingleStoreConditionalTheoryDiscoverer(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute)
        {
            yield return new SingleStoreConditionalTheoryTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod);
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute,
            object[] dataRow)
        {
            yield return new SingleStoreConditionalFactTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                dataRow);
        }
    }
}
