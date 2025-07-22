// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreIntTypeMapping : IntTypeMapping
{
    public static new SingleStoreIntTypeMapping Default { get; } = new("int");

    public SingleStoreIntTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Int32)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreIntTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreIntTypeMapping(parameters);
}
