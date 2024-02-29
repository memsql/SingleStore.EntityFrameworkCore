// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities
{
    public static class SingleStoreDbContextOptionsBuilderExtensions
    {
        public static SingleStoreDbContextOptionsBuilder ApplyConfiguration(this SingleStoreDbContextOptionsBuilder optionsBuilder)
        {
            var maxBatch = TestEnvironment.GetInt(nameof(SingleStoreDbContextOptionsBuilder.MaxBatchSize));
            if (maxBatch.HasValue)
            {
                optionsBuilder.MaxBatchSize(maxBatch.Value);
            }

            optionsBuilder.ExecutionStrategy(d => new TestSingleStoreRetryingExecutionStrategy(d));

            optionsBuilder.CommandTimeout(SingleStoreTestStore.DefaultCommandTimeout);

            return optionsBuilder;
        }
    }
}
