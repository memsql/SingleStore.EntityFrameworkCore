using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Tests;

namespace EntityFrameworkCore.SingleStore.FunctionalTests;

public class BadDataJsonDeserializationSingleStoreTest : BadDataJsonDeserializationTestBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => base.OnConfiguring(optionsBuilder.UseSingleStore(b => b.UseNetTopologySuite()));
}
