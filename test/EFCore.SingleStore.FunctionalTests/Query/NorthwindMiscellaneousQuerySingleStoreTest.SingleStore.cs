using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Storage;
using EntityFrameworkCore.SingleStore.Tests.TestUtilities.Attributes;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query
{
    public partial class NorthwindMiscellaneousQuerySingleStoreTest
    {
        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public virtual Task RowNumberOverPartitionBy_not_supported_throws(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_Joined_Take(async));
        }
    }
}
