// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NetTopologySuite.Geometries;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    internal class SingleStorePointMemberTranslator : IMemberTranslator
    {
        private static readonly IDictionary<MemberInfo, string> _geometryMemberToFunctionName = new Dictionary<MemberInfo, string>
        {
            { typeof(Point).GetRuntimeProperty(nameof(Point.X)), "ST_X" },
            { typeof(Point).GetRuntimeProperty(nameof(Point.Y)), "ST_Y" },
        };

        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStorePointMemberTranslator(SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (typeof(Point).IsAssignableFrom(member.DeclaringType))
            {
                if (_geometryMemberToFunctionName.TryGetValue(member, out var functionName))
                {
                    return _sqlExpressionFactory.NullableFunction(
                        functionName,
                        new[] { instance },
                        returnType);
                }
            }

            return null;
        }
    }
}
