// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     NetTopologySuite specific extension methods for <see cref="SingleStoreDbContextOptionsBuilder" />.
    /// </summary>
    public static class SingleStoreNetTopologySuiteDbContextOptionsBuilderExtensions
    {
        /// <summary>
        ///     Use NetTopologySuite to access MySQL spatial data.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure MySQL. </param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static SingleStoreDbContextOptionsBuilder UseNetTopologySuite(
            [NotNull] this SingleStoreDbContextOptionsBuilder optionsBuilder)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)optionsBuilder).OptionsBuilder;

            var extension = coreOptionsBuilder.Options.FindExtension<SingleStoreNetTopologySuiteOptionsExtension>()
                ?? new SingleStoreNetTopologySuiteOptionsExtension();

            ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}
