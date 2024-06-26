// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreGeometryCollectionMethodTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _item = typeof(GeometryCollection).GetRuntimeProperty("Item").GetMethod;
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreGeometryCollectionMethodTranslator(
            IRelationalTypeMappingSource typeMappingSource,
            SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (Equals(method, _item))
            {
                // Returns NULL for an empty geometry argument.
                return _sqlExpressionFactory.NullableFunction(
                    "ST_GeometryN",
                    new[]
                    {
                        instance,
                        _sqlExpressionFactory.Add(
                            arguments[0],
                            _sqlExpressionFactory.Constant(1))
                    },
                    method.ReturnType,
                    _typeMappingSource.FindMapping(typeof(Geometry), instance.TypeMapping.StoreType),
                    false);
            }

            return null;
        }
    }
}
