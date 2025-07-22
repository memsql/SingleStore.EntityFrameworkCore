// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreByteTypeMapping : ByteTypeMapping
{
    public static new SingleStoreByteTypeMapping Default { get; } = new("tinyint unsigned");

    public SingleStoreByteTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Byte)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreByteTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreByteTypeMapping(parameters);
}
