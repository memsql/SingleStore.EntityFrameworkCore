using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Tests;
using Xunit;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class BadDataJsonDeserializationSingleStoreTest : BadDataJsonDeserializationTestBase
{
    [ConditionalTheory(Skip = "Geospatial types aren't supported by this provider yet.")]
    public override void Throws_for_bad_point_as_GeoJson(string json)
    {
        base.Throws_for_bad_point_as_GeoJson(json);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => base.OnConfiguring(optionsBuilder.UseSingleStore(b => b.UseNetTopologySuite()));
}
