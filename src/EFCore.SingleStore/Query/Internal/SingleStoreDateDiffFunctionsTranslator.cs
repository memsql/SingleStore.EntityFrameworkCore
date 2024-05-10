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
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SingleStoreDateDiffFunctionsTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDateDiffMapping
            = new Dictionary<MethodInfo, string>
            {
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "YEAR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "YEAR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "YEAR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "YEAR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "YEAR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffYear), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "YEAR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "DAY" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "DAY" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "DAY" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "DAY" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "DAY" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffDay), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "DAY" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "HOUR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "HOUR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "HOUR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "HOUR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "HOUR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffHour), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "HOUR" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MINUTE" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MINUTE" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MINUTE" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MINUTE" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MINUTE" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMinute), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MINUTE" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "SECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "SECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "SECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "SECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "SECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffSecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "SECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MICROSECOND" },
            };

        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreDateDiffFunctionsTranslator(SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_methodInfoDateDiffMapping.TryGetValue(method, out var datePart))
            {
                var startDate = arguments[1];
                var endDate = arguments[2];
                var typeMapping = ExpressionExtensions.InferTypeMapping(startDate, endDate);

                startDate = _sqlExpressionFactory.ApplyTypeMapping(startDate, typeMapping);
                endDate = _sqlExpressionFactory.ApplyTypeMapping(endDate, typeMapping);

                return _sqlExpressionFactory.NullableFunction(
                    "TIMESTAMPDIFF",
                    new[]
                    {
                        _sqlExpressionFactory.Fragment(datePart),
                        startDate,
                        endDate
                    },
                    typeof(int),
                    typeMapping: null,
                    onlyNullWhenAnyNullPropagatingArgumentIsNull: true,
                    argumentsPropagateNullability: new []{false, true, true});
            }

            return null;
        }
    }
}
