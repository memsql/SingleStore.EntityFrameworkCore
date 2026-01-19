// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Query.Internal;
using EntityFrameworkCore.SingleStore.Utilities;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SingleStoreDbFunctionsExtensionsMethodTranslator : IMethodCallTranslator
    {
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

                private static readonly HashSet<MethodInfo> _convertTimeZoneMethodInfos =
        [
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateTime), typeof(string), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateOnly), typeof(string), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateTime?), typeof(string), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(string), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateTime), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateTimeOffset), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateOnly), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateTime?), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateTimeOffset?), typeof(string) }),
            typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.ConvertTimeZone),
                new[] { typeof(DbFunctions), typeof(DateOnly?), typeof(string) }),
        ];

        private static readonly Type[] _supportedLikeTypes = {
            typeof(int),
            typeof(long),
            typeof(DateTime),
            typeof(Guid),
            typeof(bool),
            typeof(byte),
            typeof(byte[]),
            typeof(double),
            typeof(DateTimeOffset),
            typeof(char),
            typeof(short),
            typeof(float),
            typeof(decimal),
            typeof(TimeSpan),
            typeof(uint),
            typeof(ushort),
            typeof(ulong),
            typeof(sbyte),
            typeof(DateOnly),
            typeof(TimeOnly),
            typeof(int?),
            typeof(long?),
            typeof(DateTime?),
            typeof(Guid?),
            typeof(bool?),
            typeof(byte?),
            typeof(double?),
            typeof(DateTimeOffset?),
            typeof(char?),
            typeof(short?),
            typeof(float?),
            typeof(decimal?),
            typeof(TimeSpan?),
            typeof(uint?),
            typeof(ushort?),
            typeof(ulong?),
            typeof(sbyte?),
            typeof(DateOnly?),
            typeof(TimeOnly?),
        };

        private static readonly MethodInfo[] _likeMethodInfos
            = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethods()
                .Where(method => method.Name == nameof(SingleStoreDbFunctionsExtensions.Like)
                                 && method.IsGenericMethod
                                 && method.GetParameters().Length is >= 3 and <= 4)
                .SelectMany(method => _supportedLikeTypes.Select(type => method.MakeGenericMethod(type))).ToArray();

        private static readonly MethodInfo _isMatchMethodInfo
            = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.IsMatch),
                new[] {typeof(DbFunctions), typeof(string), typeof(string)});

        private static readonly MethodInfo _isMatchWithMultiplePropertiesMethodInfo
            = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.IsMatch),
                new[] {typeof(DbFunctions), typeof(string[]), typeof(string)});

        private static readonly MethodInfo _matchMethodInfo
            = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.Match),
                new[] {typeof(DbFunctions), typeof(string), typeof(string)});

        private static readonly MethodInfo _matchWithMultiplePropertiesMethodInfo
            = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(
                nameof(SingleStoreDbFunctionsExtensions.Match),
                new[] {typeof(DbFunctions), typeof(string[]), typeof(string)});

        private static readonly Type[] _supportedHexTypes = {
            typeof(string),
            typeof(byte[]),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(int?),
            typeof(long?),
            typeof(short?),
            typeof(sbyte?),
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(byte),
            typeof(uint?),
            typeof(ulong?),
            typeof(ushort?),
            typeof(byte?),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(decimal?),
            typeof(double?),
            typeof(float?),
        };

        private static readonly MethodInfo[] _hexMethodInfos
            = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethods()
                .Where(method => method.Name == nameof(SingleStoreDbFunctionsExtensions.Hex) &&
                                 method.IsGenericMethod)
                .SelectMany(method => _supportedHexTypes.Select(type => method.MakeGenericMethod(type)))
                .ToArray();

        private static readonly MethodInfo _unhexMethodInfo = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.Unhex), new[] {typeof(DbFunctions), typeof(string)});

        private static readonly MethodInfo _degreesDoubleMethodInfo = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.Degrees), new[] { typeof(DbFunctions), typeof(double) });
        private static readonly MethodInfo _degreesFloatMethodInfo = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.Degrees), new[] { typeof(DbFunctions), typeof(float) });

        private static readonly MethodInfo _radiansDoubleMethodInfo = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.Radians), new[] { typeof(DbFunctions), typeof(double) });
        private static readonly MethodInfo _radiansFloatMethodInfo = typeof(SingleStoreDbFunctionsExtensions).GetRuntimeMethod(nameof(SingleStoreDbFunctionsExtensions.Radians), new[] { typeof(DbFunctions), typeof(float) });

        public SingleStoreDbFunctionsExtensionsMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (_convertTimeZoneMethodInfos.TryGetValue(method, out _))
            {
                // Will not just return `NULL` if any of its parameters is `NULL`, but also if `fromTimeZone` or `toTimeZone` is incorrect.
                // Will do no conversion at all if `dateTime` is outside the supported range.
                return _sqlExpressionFactory.NullableFunction(
                    "CONVERT_TZ",
                    arguments.Count == 3
                        ?
                        [
                            arguments[1],
                            // The implicit fromTimeZone is UTC for DateTimeOffset values and the current session time zone otherwise.
                            method.GetParameters()[1].ParameterType.UnwrapNullableType() == typeof(DateTimeOffset)
                                ? _sqlExpressionFactory.Constant("+00:00")
                                : _sqlExpressionFactory.Fragment("@@session.time_zone"),
                            arguments[2]
                        ]
                        : new[] { arguments[1], arguments[2], arguments[3] },
                    method.ReturnType.UnwrapNullableType(),
                    null,
                    false,
                    Statics.GetTrueValues(arguments.Count));
            }

            if (_likeMethodInfos.Any(m => Equals(method, m)))
            {
                var match = _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]);

                var pattern = InferStringTypeMappingOrApplyDefault(
                    arguments[2],
                    match.TypeMapping);

                var excapeChar = arguments.Count == 4
                    ? InferStringTypeMappingOrApplyDefault(
                        arguments[3],
                        match.TypeMapping)
                    : null;

                return _sqlExpressionFactory.Like(
                    match,
                    pattern,
                    excapeChar);
            }

            if (Equals(method, _isMatchMethodInfo) ||
                Equals(method, _isMatchWithMultiplePropertiesMethodInfo))
            {
                return _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.MakeMatch(arguments[1], arguments[2]),
                    _sqlExpressionFactory.Constant(0));
            }

            if (Equals(method, _matchMethodInfo) ||
                Equals(method, _matchWithMultiplePropertiesMethodInfo))
            {
                return _sqlExpressionFactory.MakeMatch(arguments[1], arguments[2]);
            }

            if (_hexMethodInfos.Any(m => Equals(method, m)))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "HEX",
                    new[] {arguments[1]},
                    typeof(string));
            }

            if (Equals(method, _unhexMethodInfo))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "UNHEX",
                    new[] {arguments[1]},
                    typeof(string),
                    false);
            }

            if (Equals(method, _degreesDoubleMethodInfo) ||
                Equals(method, _degreesFloatMethodInfo))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "DEGREES",
                    new[] { arguments[1] },
                    method.ReturnType);
            }

            if (Equals(method, _radiansDoubleMethodInfo) ||
                Equals(method, _radiansFloatMethodInfo))
            {
                return _sqlExpressionFactory.NullableFunction(
                    "RADIANS",
                    new[] { arguments[1] },
                    method.ReturnType);
            }

            return null;
        }

        private SqlExpression InferStringTypeMappingOrApplyDefault(SqlExpression expression, RelationalTypeMapping inferenceSourceTypeMapping)
        {
            if (expression == null)
            {
                return null;
            }

            if (expression.TypeMapping != null)
            {
                return expression;
            }

            if (expression.Type == typeof(string) && inferenceSourceTypeMapping?.ClrType == typeof(string))
            {
                return _sqlExpressionFactory.ApplyTypeMapping(expression, inferenceSourceTypeMapping);
            }

            return _sqlExpressionFactory.ApplyDefaultTypeMapping(expression);
        }
    }
}
