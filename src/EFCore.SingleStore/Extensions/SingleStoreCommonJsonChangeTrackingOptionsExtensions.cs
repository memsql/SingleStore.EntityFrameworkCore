// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using EntityFrameworkCore.SingleStore.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class SingleStoreCommonJsonChangeTrackingOptionsExtensions
    {
        public static SingleStoreJsonChangeTrackingOptions ToJsonChangeTrackingOptions(this SingleStoreCommonJsonChangeTrackingOptions options)
            => options switch
            {
                SingleStoreCommonJsonChangeTrackingOptions.RootPropertyOnly => SingleStoreJsonChangeTrackingOptions.CompareRootPropertyOnly,
                SingleStoreCommonJsonChangeTrackingOptions.FullHierarchyOptimizedFast => SingleStoreJsonChangeTrackingOptions.CompareStringRootPropertyByEquals |
                                                                                   SingleStoreJsonChangeTrackingOptions.CompareDomRootPropertyByEquals |
                                                                                   SingleStoreJsonChangeTrackingOptions.SnapshotCallsDeepClone |
                                                                                   SingleStoreJsonChangeTrackingOptions.SnapshotCallsClone,
                SingleStoreCommonJsonChangeTrackingOptions.FullHierarchyOptimizedSemantically => SingleStoreJsonChangeTrackingOptions.CompareStringRootPropertyByEquals |
                                                                                           SingleStoreJsonChangeTrackingOptions.CompareDomSemantically |
                                                                                           SingleStoreJsonChangeTrackingOptions.HashDomSemantiallyOptimized |
                                                                                           SingleStoreJsonChangeTrackingOptions.SnapshotCallsDeepClone |
                                                                                           SingleStoreJsonChangeTrackingOptions.SnapshotCallsClone,
                SingleStoreCommonJsonChangeTrackingOptions.FullHierarchySemantically => SingleStoreJsonChangeTrackingOptions.None,
                _ => throw new ArgumentOutOfRangeException(nameof(options)),
            };
    }
}
