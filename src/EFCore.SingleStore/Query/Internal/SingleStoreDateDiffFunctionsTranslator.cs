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

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "QUARTER" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "QUARTER" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "QUARTER" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "QUARTER" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "QUARTER" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffQuarter), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "QUARTER" },

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MONTH" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMonth), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MONTH" },

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "WEEK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "WEEK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "WEEK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "WEEK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "WEEK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffWeek), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "WEEK" },

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

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MILLISECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MILLISECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MILLISECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MILLISECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MILLISECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMillisecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MILLISECOND" },

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "MICROSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffMicrosecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "MICROSECOND" },

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "TICK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "TICK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "TICK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "TICK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "TICK" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffTick), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "TICK" },

                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTime), typeof(DateTime) }), "NANOSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTime?), typeof(DateTime?) }), "NANOSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(DateTimeOffset) }), "NANOSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(DateTimeOffset?) }), "NANOSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateOnly), typeof(DateOnly) }), "NANOSECOND" },
                { typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.DateDiffNanosecond), new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(DateOnly?) }), "NANOSECOND" },
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

                var actualDatePart = datePart is "MILLISECOND"
                    or "TICK"
                    or "NANOSECOND"
                    ? "MICROSECOND"
                    : datePart;

                var timeStampDiffExpression = _sqlExpressionFactory.NullableFunction(
                    "TIMESTAMPDIFF",
                    new[]
                    {
                        _sqlExpressionFactory.Fragment(actualDatePart),
                        startDate,
                        endDate
                    },
                    typeof(int),
                    typeMapping: null,
                    onlyNullWhenAnyNullPropagatingArgumentIsNull: true,
                    argumentsPropagateNullability: new[] { false, true, true });

                return datePart switch
                {
                    "MILLISECOND" => _sqlExpressionFactory.SingleStoreIntegerDivide(timeStampDiffExpression, _sqlExpressionFactory.Constant(1_000)),
                    "TICK" => _sqlExpressionFactory.Multiply(timeStampDiffExpression, _sqlExpressionFactory.Constant(10)),
                    "NANOSECOND" => _sqlExpressionFactory.Multiply(timeStampDiffExpression, _sqlExpressionFactory.Constant(1_000)),
                    _ => timeStampDiffExpression
                };
            }

            return null;
        }
    }
}
