// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.ValueComparison.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Storage.ValueComparison.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Extensions.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class SingleStoreInternalJsonMicrosoftPropertyExtensions
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static void SetJsonChangeTrackingOptions([NotNull] this IMutableProperty property, SingleStoreJsonChangeTrackingOptions? options)
        {
            Check.NotNull(property, nameof(property));

            if (options == null)
            {
                // Use globally configured options for this context.
                // This can always be used to get back to the default implementation and options.
                property.SetValueComparer((ValueComparer)null);
                return;
            }

            var valueComparer = property.GetValueComparer() ?? property.FindTypeMapping()?.Comparer;
            property.SetValueComparer(
                valueComparer is ISingleStoreJsonValueComparer mySqlJsonValueComparer
                    ? mySqlJsonValueComparer.Clone(options.Value)
                    : SingleStoreJsonMicrosoftValueComparer.Create(property.ClrType, options.Value));
        }
    }
}
