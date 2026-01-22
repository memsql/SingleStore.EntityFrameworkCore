using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Conventions
{
    /// <summary>
    /// Test-only convention:
    /// SingleStore distributed tables require AUTO_INCREMENT on BIGINT,
    /// so force BIGINT for int identity PKs in tests.
    /// </summary>
    public sealed class SingleStoreTestBigIntIdentityConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                // Only the actual entity PK(s) matter here; complex/owned types won't have keys.
                var pk = entityType.FindPrimaryKey();
                if (pk is null)
                    continue;

                foreach (var property in pk.Properties)
                {
                    // Target the classic pattern: int PK generated on add.
                    if (property.ClrType != typeof(int) && property.ClrType != typeof(int?))
                        continue;

                    if (property.ValueGenerated != ValueGenerated.OnAdd)
                        continue;

                    // Force store type to BIGINT.
                    property.Builder.HasColumnType("bigint");
                }
            }
        }
    }
}
