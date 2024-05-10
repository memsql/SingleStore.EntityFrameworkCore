// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NetTopologySuite.Geometries;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreGeometryCollectionMemberTranslator : IMemberTranslator
    {
        private static readonly MemberInfo _count = typeof(GeometryCollection).GetRuntimeProperty(nameof(GeometryCollection.Count));
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreGeometryCollectionMemberTranslator(SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (Equals(member, _count))
            {
                // Returns NULL for an empty geometry argument.
                return _sqlExpressionFactory.NullableFunction(
                    "ST_NumGeometries",
                    new [] {instance},
                    returnType,
                    false);
            }

            return null;
        }
    }
}
