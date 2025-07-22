// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreLongTypeMapping : LongTypeMapping
{
    public static new SingleStoreLongTypeMapping Default { get; } = new("bigint");

    public SingleStoreLongTypeMapping(
        string storeType,
        DbType? dbType = System.Data.DbType.Int64)
        : base(storeType, dbType)
    {
    }

    protected SingleStoreLongTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new SingleStoreLongTypeMapping(parameters);
}
