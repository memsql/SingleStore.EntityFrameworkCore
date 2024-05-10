// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreDateTimeMethodTranslator : IMethodCallTranslator
    {
        private readonly Dictionary<MethodInfo, string> _methodInfoDatePartMapping = new Dictionary<MethodInfo, string>
        {
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddYears), new[] { typeof(int) }), "year" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMonths), new[] { typeof(int) }), "month" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddDays), new[] { typeof(double) }), "day" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddHours), new[] { typeof(double) }), "hour" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMinutes), new[] { typeof(double) }), "minute" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddSeconds), new[] { typeof(double) }), "second" },
            { typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMilliseconds), new[] { typeof(double) }), "microsecond" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddYears), new[] { typeof(int) }), "year" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMonths), new[] { typeof(int) }), "month" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddDays), new[] { typeof(double) }), "day" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddHours), new[] { typeof(double) }), "hour" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMinutes), new[] { typeof(double) }), "minute" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddSeconds), new[] { typeof(double) }), "second" },
            { typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMilliseconds), new[] { typeof(double) }), "microsecond" },
            { typeof(DateOnly).GetRuntimeMethod(nameof(DateOnly.AddYears), new[] { typeof(int) }), "year" },
            { typeof(DateOnly).GetRuntimeMethod(nameof(DateOnly.AddMonths), new[] { typeof(int) }), "month" },
            { typeof(DateOnly).GetRuntimeMethod(nameof(DateOnly.AddDays), new[] { typeof(int) }), "day" },
        };

        private static readonly MethodInfo _timeOnlyAddTimeSpanMethod = typeof(TimeOnly).GetRuntimeMethod(nameof(TimeOnly.Add), new[] { typeof(TimeSpan) })!;
        private static readonly MethodInfo _timeOnlyAddHoursMethod = typeof(TimeOnly).GetRuntimeMethod(nameof(TimeOnly.AddHours), new[] {typeof(double)})!;
        private static readonly MethodInfo _timeOnlyAddMinutesMethod = typeof(TimeOnly).GetRuntimeMethod(nameof(TimeOnly.AddMinutes), new[] {typeof(double)})!;
        private static readonly MethodInfo _timeOnlyIsBetweenMethod = typeof(TimeOnly).GetRuntimeMethod(nameof(TimeOnly.IsBetween), new[] { typeof(TimeOnly), typeof(TimeOnly) })!;

        private static readonly MethodInfo _dateOnlyFromDateTimeMethod = typeof(DateOnly).GetRuntimeMethod(nameof(DateOnly.FromDateTime), new[] { typeof(DateTime) })!;
        private static readonly MethodInfo _dateOnlyToDateTimeMethod = typeof(DateOnly).GetRuntimeMethod(nameof(DateOnly.ToDateTime), new[] { typeof(TimeOnly) })!;

        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreDateTimeMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_methodInfoDatePartMapping.TryGetValue(method, out var datePart))
            {
                return !datePart.Equals("year")
                       && !datePart.Equals("month")
                       && arguments[0] is SqlConstantExpression { Value: double and (>= int.MaxValue or <= int.MinValue) }
                    ? null
                    : _sqlExpressionFactory.NullableFunction(
                        "DATE_ADD",
                        new[]
                        {
                            instance,
                            _sqlExpressionFactory.ComplexFunctionArgument(
                                new SqlExpression[]
                                {
                                    _sqlExpressionFactory.Fragment("INTERVAL"),
                                    datePart.Equals("microsecond")
                                        ? _sqlExpressionFactory.Multiply(
                                            _sqlExpressionFactory.Constant(1000),
                                            _sqlExpressionFactory.Convert(arguments[0], typeof(int)))
                                        : _sqlExpressionFactory.Convert(arguments[0], typeof(int)),
                                    _sqlExpressionFactory.Fragment(datePart)
                                },
                                " ",
                                typeof(string))
                        },
                        instance.Type,
                        instance.TypeMapping,
                        true,
                        new[] {true, false});
            }

            if (method.DeclaringType == typeof(TimeOnly))
            {
                if (method == _timeOnlyAddTimeSpanMethod)
                {
                    return _sqlExpressionFactory.Add(instance, arguments[0]);
                }

                if (method == _timeOnlyAddHoursMethod || method == _timeOnlyAddMinutesMethod)
                {
                    if (arguments[0] is SqlConstantExpression sqlConstantExpression && sqlConstantExpression.Value is double value)
                    {
                        var timespan = method == _timeOnlyAddHoursMethod? TimeSpan.FromHours(value) : TimeSpan.FromMinutes(value);
                        return _sqlExpressionFactory.Add(instance, _sqlExpressionFactory.Fragment("\"" + timespan.ToString() + "\""));
                    }
                }

                if (method == _timeOnlyIsBetweenMethod)
                {
                    return _sqlExpressionFactory.And(
                        _sqlExpressionFactory.GreaterThanOrEqual(instance, arguments[0]),
                        _sqlExpressionFactory.LessThan(instance, arguments[1]));
                }
            }

            if (method.DeclaringType == typeof(DateOnly))
            {
                if (method == _dateOnlyFromDateTimeMethod)
                {
                    return _sqlExpressionFactory.NullableFunction(
                        "DATE",
                        new[] { arguments[0] },
                        method.ReturnType);
                }

                if (method == _dateOnlyToDateTimeMethod)
                {
                    var convertExpression = _sqlExpressionFactory.Convert(
                        instance,
                        method.ReturnType);

                    if (arguments[0] is SqlConstantExpression sqlConstantExpression &&
                        sqlConstantExpression.Value is TimeOnly timeOnly &&
                        timeOnly == default)
                    {
                        return convertExpression;
                    }

                    return _sqlExpressionFactory.NullableFunction(
                        "ADDTIME",
                        new[]
                        {
                            convertExpression,
                            arguments[0]
                        },
                        method.ReturnType);
                }
            }

            return null;
        }
    }
}
