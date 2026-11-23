using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.BasicTypesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class BasicTypesQuerySingleStoreFixture : BasicTypesQueryFixtureBase, ITestSqlLoggerFactory
{
    private BasicTypesData _expectedData;

    protected override ITestStoreFactory TestStoreFactory
        => SingleStoreTestStoreFactory.Instance;

    protected override Task SeedAsync(BasicTypesContext context)
    {
        _expectedData ??= LoadAndTweakData();
        context.AddRange(_expectedData.BasicTypesEntities);
        context.AddRange(_expectedData.NullableBasicTypesEntities);
        return context.SaveChangesAsync();
    }

    public override ISetSource GetExpectedData()
        => _expectedData ??= LoadAndTweakData();

    private BasicTypesData LoadAndTweakData()
    {
        var data = (BasicTypesData)base.GetExpectedData();

        foreach (var item in data.BasicTypesEntities)
        {
            // Strip sub-microsecond precision, since SingleStore's DATETIME(6) and TIME(6) types store at most microseconds,
            // and move unset (default) temporal values off their minimum value.

            if (item.DateTime == default)
            {
                item.DateTime += TimeSpan.FromSeconds(1);
            }

            // Seed DateTime values as DateTimeKind.Utc, which several DateTime test overrides rely on.
            item.DateTime = DateTime.SpecifyKind(new DateTime(StripSubMicrosecond(item.DateTime.Ticks)), DateTimeKind.Utc);

            if (item.DateOnly == default)
            {
                item.DateOnly = item.DateOnly.AddDays(1);
            }

            item.TimeOnly = new TimeOnly(StripSubMicrosecond(item.TimeOnly.Ticks));
            item.TimeSpan = new TimeSpan(StripSubMicrosecond(item.TimeSpan.Ticks));

            if (item.DateTimeOffset == default)
            {
                item.DateTimeOffset += TimeSpan.FromSeconds(1);
            }

            // Seed DateTimeOffset values with a zero offset (UTC) and without sub-microsecond precision.
            item.DateTimeOffset = new DateTimeOffset(StripSubMicrosecond(item.DateTimeOffset.Ticks), TimeSpan.Zero);
        }

        // Do the same for the nullable counterparts
        foreach (var item in data.NullableBasicTypesEntities)
        {
            if (item.DateTime.HasValue)
            {
                item.DateTime = DateTime.SpecifyKind(new DateTime(StripSubMicrosecond(item.DateTime.Value.Ticks)), DateTimeKind.Utc);
            }

            if (item.TimeOnly.HasValue)
            {
                item.TimeOnly = new TimeOnly(StripSubMicrosecond(item.TimeOnly.Value.Ticks));
            }

            if (item.TimeSpan.HasValue)
            {
                item.TimeSpan = new TimeSpan(StripSubMicrosecond(item.TimeSpan.Value.Ticks));
            }

            if (item.DateTimeOffset.HasValue)
            {
                item.DateTimeOffset = new DateTimeOffset(StripSubMicrosecond(item.DateTimeOffset.Value.Ticks), TimeSpan.Zero);
            }
        }

        return data;

        static long StripSubMicrosecond(long ticks) => ticks - (ticks % (TimeSpan.TicksPerMillisecond / 1000));
    }

    public TestSqlLoggerFactory TestSqlLoggerFactory
        => (TestSqlLoggerFactory)ListLoggerFactory;
}
