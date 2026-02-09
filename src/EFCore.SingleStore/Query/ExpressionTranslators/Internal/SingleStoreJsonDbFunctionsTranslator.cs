// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Query.Expressions.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal
{
    public class SingleStoreJsonDbFunctionsTranslator : IMethodCallTranslator
    {
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreJsonDbFunctionsTranslator([NotNull] SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (method.DeclaringType != typeof(SingleStoreJsonDbFunctionsExtensions))
            {
                return null;
            }

            var args = arguments
                // Skip useless DbFunctions instance
                .Skip(1)
                // JSON extensions accept object parameters for JSON, since they must be able to handle POCOs, strings or DOM types.
                // This means they come wrapped in a convert node, which we need to remove.
                // Convert nodes may also come from wrapping JsonTraversalExpressions generated through POCO traversal.
                .Select(RemoveConvert)
                // CHECK: Either just not doing this at all is fine, or not applying it to JsonQuote and JsonUnquote
                // (as already implemented below) is needed. An alternative would be to move the check into the local
                // json() function.
                //
                // If a function is invoked over a JSON traversal expression, that expression may come with
                // returnText: true (i.e. operator ->> and not ->). Since the functions below require a json object and
                // not text, we transform it.
                // .Select(
                //     a => a is SingleStoreJsonTraversalExpression traversal &&
                //          method.Name != nameof(SingleStoreJsonDbFunctionsExtensions.JsonQuote) &&
                //          method.Name != nameof(SingleStoreJsonDbFunctionsExtensions.JsonUnquote)
                //         ? withReturnsText(traversal, false)
                //         : a)
                .ToArray();

            var result = method.Name switch
            {
                nameof(SingleStoreJsonDbFunctionsExtensions.AsJson)
                    => _sqlExpressionFactory.ApplyTypeMapping(
                        args[0],
                        _sqlExpressionFactory.FindMapping(method.ReturnType, "json")),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonType)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_TYPE",
                        new[] {Json(args[0])},
                        typeof(string)),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonQuote)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_QUOTE",
                        new[] {args[0]},
                        method.ReturnType),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonUnquote)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_UNQUOTE",
                        new[] {args[0]},
                        method.ReturnType),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonExtract)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_EXTRACT",
                        Array.Empty<SqlExpression>()
                            .Append(Json(args[0]))
                            .Concat(DeconstructParamsArray(args[1])),
                        method.ReturnType,
                        _sqlExpressionFactory.FindMapping(method.ReturnType, "json"),
                        false),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonOverlaps)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_OVERLAPS",
                        new[] { Json(args[0]), args[1] },
                        typeof(bool)),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonContains)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_CONTAINS",
                        args.Length >= 3
                            ? new[] {Json(args[0]), args[1], args[2]}
                            : new[] {Json(args[0]), args[1]},
                        typeof(bool)),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonContainsPath)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_CONTAINS_PATH",
                        new[] {Json(args[0]), _sqlExpressionFactory.Constant("one"), args[1]},
                        typeof(bool)),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonContainsPathAny)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_CONTAINS_PATH",
                        Array.Empty<SqlExpression>()
                            .Append(Json(args[0]))
                            .Append(_sqlExpressionFactory.Constant("one"))
                            .Concat(DeconstructParamsArray(args[1])),
                        typeof(bool)),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonContainsPathAll)
                    => _sqlExpressionFactory.NullableFunction(
                        "JSON_CONTAINS_PATH",
                        Array.Empty<SqlExpression>()
                            .Append(Json(args[0]))
                            .Append(_sqlExpressionFactory.Constant("all"))
                            .Concat(DeconstructParamsArray(args[1])),
                        typeof(bool)),
                nameof(SingleStoreJsonDbFunctionsExtensions.JsonSearchAny)
                    => _sqlExpressionFactory.IsNotNull(
                        _sqlExpressionFactory.NullableFunction(
                            "JSON_SEARCH",
                            Array.Empty<SqlExpression>()
                                .Append(Json(args[0]))
                                .Append(_sqlExpressionFactory.Constant("one"))
                                .Append(args[1])
                                .AppendIfTrue(
                                    args.Length >= 3,
                                    () => args.Length >= 4
                                        ? args[3]
                                        : _sqlExpressionFactory.Constant(null, typeof(string)))
                                .AppendIfTrue(args.Length >= 3, () => args[2]),
                            typeof(bool),
                            null,
                            false)), // JSON_SEARCH can return null even if all arguments are not null
                _ => null
            };

            return result;

            SqlExpression Json(SqlExpression e) => _sqlExpressionFactory.ApplyTypeMapping(EnsureJson(e), _sqlExpressionFactory.FindMapping(e.Type, "json"));

            static SqlExpression EnsureJson(SqlExpression e)
                => e.TypeMapping is SingleStoreJsonTypeMapping ||
                   e is SingleStoreJsonTraversalExpression
                    ? e
                    : throw new InvalidOperationException("The JSON method requires a JSON parameter but none was found.");

            static SqlExpression RemoveConvert(SqlExpression e)
            {
                while (e is SqlUnaryExpression unary &&
                       (unary.OperatorType == ExpressionType.Convert || unary.OperatorType == ExpressionType.ConvertChecked))
                {
                    e = unary.Operand;
                }

                return e;
            }

            // SingleStoreJsonTraversalExpression withReturnsText(SingleStoreJsonTraversalExpression traversal, bool returnsText)
            //     => traversal.ReturnsText == returnsText
            //         ? traversal
            //         : returnsText
            //             ? traversal.Clone(returnsText, typeof(string), _stringTypeMapping)
            //             : traversal.Clone(returnsText, traversal.Type, traversal.Expression.TypeMapping);

            IEnumerable<SqlExpression> DeconstructParamsArray(SqlExpression paramsArray)
            {
                if (paramsArray is SqlConstantExpression constant)
                {
                    if (constant.Value is object[] array)
                    {
                        foreach (var value in array)
                        {
                            yield return _sqlExpressionFactory.Constant(value);
                        }
                    }
                }
                else
                {
                    yield return paramsArray;
                }
            }
        }
    }
}
