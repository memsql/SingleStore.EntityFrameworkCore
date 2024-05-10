// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.ChangeTracking;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Storage.ValueComparison.Internal
{
    public interface ISingleStoreJsonValueComparer
    {
        ValueComparer Clone(SingleStoreJsonChangeTrackingOptions options);
    }
}
