// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreUShortTypeMapping : UShortTypeMapping
{
    public static new SingleStoreUShortTypeMapping Default { get; } = new("smallint unsigned");

    public SingleStoreUShortTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.UInt16)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreUShortTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreUShortTypeMapping(parameters);
}
