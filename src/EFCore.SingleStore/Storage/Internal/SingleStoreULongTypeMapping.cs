// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreULongTypeMapping : ULongTypeMapping
{
    public static new SingleStoreULongTypeMapping Default { get; } = new("bigint unsigned");

    public SingleStoreULongTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.UInt64)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreULongTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreULongTypeMapping(parameters);
}
