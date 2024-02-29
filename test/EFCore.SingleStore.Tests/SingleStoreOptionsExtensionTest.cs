using System;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using Xunit;

namespace EntityFrameworkCore.SingleStore
{
    public class SingleStoreOptionsExtensionTest
    {
        [Fact]
        public void GetServiceProviderHashCode_returns_same_value()
        {
            Assert.Equal(
                new SingleStoreOptionsExtension().Info.GetServiceProviderHashCode(),
                new SingleStoreOptionsExtension().Info.GetServiceProviderHashCode());

            Assert.Equal(
                new SingleStoreOptionsExtension()
                    .WithServerVersion(new SingleStoreServerVersion(new Version(1, 2, 3, 4)))
                    .WithDisabledBackslashEscaping()
                    .Info
                    .GetServiceProviderHashCode(),
                new SingleStoreOptionsExtension()
                    .WithServerVersion(new SingleStoreServerVersion(new Version(1, 2, 3, 4)))
                    .WithDisabledBackslashEscaping()
                    .Info
                    .GetServiceProviderHashCode());
        }
    }
}
