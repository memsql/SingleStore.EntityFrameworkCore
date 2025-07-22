// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreSByteTypeMapping : SByteTypeMapping
{
    public static new SingleStoreSByteTypeMapping Default { get; } = new("tinyint");

    public SingleStoreSByteTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.SByte)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreSByteTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreSByteTypeMapping(parameters);
}
