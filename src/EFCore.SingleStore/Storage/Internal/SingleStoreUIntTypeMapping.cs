// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreUIntTypeMapping : UIntTypeMapping
{
    public static new SingleStoreUIntTypeMapping Default { get; } = new("int unsigned");

    public SingleStoreUIntTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.UInt32)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreUIntTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreUIntTypeMapping(parameters);
}
