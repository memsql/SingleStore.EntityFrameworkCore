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

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreNewGuidTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo = typeof(Guid).GetRuntimeMethod(nameof(Guid.NewGuid), Array.Empty<Type>());
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreNewGuidTranslator(SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            return _methodInfo.Equals(method)
                ? _sqlExpressionFactory.NonNullableFunction(
                    "UUID",
                    Array.Empty<SqlExpression>(),
                    method.ReturnType)
                : null;
        }
    }
}
