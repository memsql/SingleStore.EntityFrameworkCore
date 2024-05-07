// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Extensions.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Extensions
{
    /// <summary>
    ///     Extension methods for <see cref="IProperty" /> for MySQL-specific metadata.
    /// </summary>
    public static class SingleStoreJsonMicrosoftPropertyExtensions
    {
        /// <summary>
        /// Sets specific change tracking options for this JSON property, that specify how inner properties or array
        /// elements will be tracked. Applies to simple strings, POCOs and DOM objects. Using `null` restores all
        /// defaults.
        /// </summary>
        /// <param name="property">The JSON property to set the change tracking options for.</param>
        /// <param name="options">The change tracking option to configure for the JSON property.</param>
        public static void SetJsonChangeTrackingOptions([NotNull] this IMutableProperty property, SingleStoreCommonJsonChangeTrackingOptions? options)
            => property.SetJsonChangeTrackingOptions(options?.ToJsonChangeTrackingOptions());
    }
}
