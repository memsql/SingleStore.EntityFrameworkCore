using System;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class EntitySplittingSingleStoreTest : EntitySplittingTestBase
{
    public EntitySplittingSingleStoreTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // We're changing the data type of the fields from INT to BIGINT, because in SingleStore
        // on a sharded (distributed) table, AUTO_INCREMENT can only be used on a BIGINT column
        modelBuilder.Entity<MeterReading>()
            .Property(e => e.Id)
            .HasColumnType("bigint");

        base.OnModelCreating(modelBuilder);
    }

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;
}
