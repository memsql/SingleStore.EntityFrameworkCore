using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using EntityFrameworkCore.SingleStore.Metadata.Conventions;
using EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Conventions;

namespace EntityFrameworkCore.SingleStore.FunctionalTests.TestUtilities.Conventions
{
    public sealed class SingleStoreTestConventionSetBuilder : IConventionSetBuilder
    {
        private readonly ProviderConventionSetBuilderDependencies _dependencies;
        private readonly RelationalConventionSetBuilderDependencies _relationalDependencies;

        public SingleStoreTestConventionSetBuilder(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
        {
            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
        }

        public ConventionSet CreateConventionSet()
        {
            var conventionSet = new SingleStoreConventionSetBuilder(_dependencies, _relationalDependencies)
                .CreateConventionSet();

            // Run at model finalization so it affects DDL generation.
            conventionSet.ModelFinalizingConventions.Add(new SingleStoreTestBigIntIdentityConvention());

            return conventionSet;
        }
    }
}
