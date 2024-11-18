using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.Query;

public class TPCGearsOfWarQuerySingleStoreFixture : TPCGearsOfWarQueryRelationalFixture, IQueryFixtureBase
{
    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;

    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    {
        var optionsBuilder = base.AddOptions(builder);

        new SingleStoreDbContextOptionsBuilder(optionsBuilder)
            .EnableIndexOptimizedBooleanColumns(true);

        return optionsBuilder;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        modelBuilder.Entity<Weapon>().HasIndex(e => e.IsAutomatic);
    }

    public new ISetSource GetExpectedData()
    {
        var data = (GearsOfWarData)base.GetExpectedData();

        foreach (var mission in data.Missions)
        {
            mission.Timeline = SingleStoreTestHelpers.GetExpectedValue(mission.Timeline);
        }

        return data;
    }
}
