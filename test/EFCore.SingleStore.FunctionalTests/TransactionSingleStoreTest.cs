using System;
using System.Threading.Tasks;
using System.Transactions;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Tests;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests
{
    public class TransactionSingleStoreTest : TransactionTestBase<TransactionSingleStoreTest.TransactionSingleStoreFixture>
    {
        public TransactionSingleStoreTest(TransactionSingleStoreFixture fixture)
            : base(fixture)
        {
        }

        protected override bool SnapshotSupported => false;
        protected override bool AmbientTransactionsSupported => true;
        protected override bool DirtyReadsOccur => false;

        protected override bool SavepointsSupported => false;

        [ConditionalTheory(Skip = "Feature 'SAVEPOINT' is not supported by SingleStore.")]
        public override async Task Savepoint_can_be_released(bool async)
        {
            await base.Savepoint_can_be_released(async);
        }

        [ConditionalTheory(Skip = "Feature 'SAVEPOINT' is not supported by SingleStore.")]
        public override async Task Savepoint_can_be_rolled_back(bool async)
        {
            await base.Savepoint_can_be_rolled_back(async);
        }

        [ConditionalTheory(Skip = "Feature 'SAVEPOINT' is not supported by SingleStore.")]
        public override async Task Savepoint_name_is_quoted(bool async)
        {
            await base.Savepoint_name_is_quoted(async);
        }

        [ConditionalFact(Skip = "Feature 'XaTransactions' is not supported by SingleStore.")]
        public override void BeginTransaction_throws_if_ambient_transaction_started()
        {
            base.BeginTransaction_throws_if_ambient_transaction_started();
        }

        [ConditionalFact(Skip = "Feature 'XaTransactions' is not supported by SingleStore.")]
        public override void EnlistTransaction_throws_if_ambient_transaction_started()
        {
            base.EnlistTransaction_throws_if_ambient_transaction_started();
        }

        protected override DbContext CreateContextWithConnectionString()
        {
            var options = Fixture.AddOptions(
                    new DbContextOptionsBuilder()
                        .UseSingleStore(
                            TestStore.ConnectionString,
                            b => SingleStoreTestStore.AddOptions(b).ExecutionStrategy(c => new SingleStoreExecutionStrategy(c))))
                .UseInternalServiceProvider(Fixture.ServiceProvider);

            return new DbContext(options.Options);
        }

        public class TransactionSingleStoreFixture : TransactionFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => SingleStoreTestStoreFactory.Instance;

            public override void Reseed()
            {
                using var context = CreateContext();
                context.Set<TransactionCustomer>().RemoveRange(context.Set<TransactionCustomer>());
                context.Set<TransactionOrder>().RemoveRange(context.Set<TransactionOrder>());
                context.SaveChanges();

                base.Seed(context);
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new SingleStoreDbContextOptionsBuilder(
                        base.AddOptions(builder))
                    .ExecutionStrategy(c => new SingleStoreExecutionStrategy(c));
                return builder;
            }

            // public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            // {
            //     new SingleStoreDbContextOptionsBuilder(base.AddOptions(builder))
            //         .MaxBatchSize(1);
            //     return builder;
            // }
        }
    }
}
