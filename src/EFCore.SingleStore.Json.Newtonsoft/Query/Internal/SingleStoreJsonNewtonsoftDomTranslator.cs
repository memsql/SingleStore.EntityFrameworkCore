using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.SingleStore.Query.Expressions.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Query.Internal
{
    public class SingleStoreJsonNewtonsoftDomTranslator : IMemberTranslator, IMethodCallTranslator
    {
        private static readonly MethodInfo _enumerableAnyWithoutPredicate = typeof(Enumerable).GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(mi => mi.Name == nameof(Enumerable.Any) && mi.GetParameters().Length == 1);

        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;
        private readonly SingleStoreJsonPocoTranslator _jsonPocoTranslator;

        public SingleStoreJsonNewtonsoftDomTranslator(
            [NotNull] SingleStoreSqlExpressionFactory sqlExpressionFactory,
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] SingleStoreJsonPocoTranslator jsonPocoTranslator)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _typeMappingSource = typeMappingSource;
            _jsonPocoTranslator = jsonPocoTranslator;
        }

        public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (instance?.Type.IsGenericList() == true &&
                member.Name == nameof(List<object>.Count) &&
                instance.TypeMapping is null)
            {
                return _jsonPocoTranslator.TranslateArrayLength(instance);
            }

            if (!typeof(JToken).IsAssignableFrom(member.DeclaringType))
            {
                return null;
            }

            if (member.Name == nameof(JToken.Root) &&
                instance is ColumnExpression column &&
                column.TypeMapping is SingleStoreJsonTypeMapping)
            {
                // Simply get rid of the RootElement member access
                return column;
            }

            var traversal = GetTraversalExpression(instance);
            if (traversal == null)
            {
                return null;
            }

            // Support for JContainer.Count property (e.g. for JArray):
            if (typeof(JContainer).IsAssignableFrom(member.DeclaringType) &&
                member.Name == nameof(JContainer.Count))
            {
                return _jsonPocoTranslator.TranslateArrayLength(traversal);
            }

            return null;
        }

        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            var traversal = GetTraversalExpression(instance, arguments);
            if (traversal == null)
            {
                return null;
            }

            if (typeof(JToken).IsAssignableFrom(method.DeclaringType) &&
                method.Name == "get_Item" &&
                arguments.Count == 1)
            {
                var indexExpression = _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[0]);

                if (method.DeclaringType == typeof(JArray) ||
                    indexExpression.Type == typeof(int))
                {
                    // Try translating indexing inside json column.
                    return _jsonPocoTranslator.TranslateMemberAccess(
                        traversal,
                        _sqlExpressionFactory.JsonArrayIndex(indexExpression),
                        method.ReturnType);
                }

                return traversal.Append(
                        ApplyPathLocationTypeMapping(arguments[0]),
                        method.DeclaringType,
                        _typeMappingSource.FindMapping(method.DeclaringType));
            }

            // Support for .Value<T>() and .Value<U, T>():
            if (instance == null &&
                method.Name == nameof(global::Newtonsoft.Json.Linq.Extensions.Value) &&
                method.DeclaringType == typeof(global::Newtonsoft.Json.Linq.Extensions) &&
                method.IsGenericMethod &&
                method.GetParameters().Length == 1 &&
                arguments.Count == 1)
            {
                return ConvertFromJsonExtract(
                    traversal.Clone(
                        method.ReturnType == typeof(string),
                        method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType)
                    ),
                    method.ReturnType);
            }

            // Support for Count()
            if (instance == null &&
                method.Name == nameof(Enumerable.Count) &&
                method.DeclaringType == typeof(Enumerable) &&
                method.IsGenericMethod &&
                method.GetParameters().Length == 1 &&
                arguments.Count == 1)
            {
                return _jsonPocoTranslator.TranslateArrayLength(traversal);
            }

            // Predicate-less Any - translate to a simple length check.
            if (method.IsClosedFormOf(_enumerableAnyWithoutPredicate) &&
                arguments.Count == 1 &&
                arguments[0].Type.TryGetElementType(out _) &&
                arguments[0].TypeMapping is SingleStoreJsonTypeMapping)
            {
                return _sqlExpressionFactory.GreaterThan(
                    _jsonPocoTranslator.TranslateArrayLength(arguments[0]),
                    _sqlExpressionFactory.Constant(0));
            }

            return null;
        }

        // The traversal expression can be the instance of the method that is being called, or the first
        // argument, if the method called is an extension method.
        private SingleStoreJsonTraversalExpression GetTraversalExpression(SqlExpression instance, IReadOnlyList<SqlExpression> arguments = null)
            => instance == null
                ? arguments?.Count >= 1
                    ? GetJsonRootExpression(arguments[0])
                    : null
                : GetJsonRootExpression(instance);

        // The root of the JSON expression is a ColumnExpression. We wrap that with an empty traversal
        // expression (col->'$'); subsequent traversals will gradually append the path into that.
        // Note that it's possible to call methods such as GetString() directly on the root, and the
        // empty traversal is necessary to properly convert it to a text.
        private SingleStoreJsonTraversalExpression GetJsonRootExpression(SqlExpression instance)
            => instance is ColumnExpression columnExpression &&
               instance.TypeMapping is SingleStoreJsonTypeMapping mapping
                ? _sqlExpressionFactory.JsonTraversal(
                    columnExpression, returnsText: false, typeof(string), mapping)
                : instance as SingleStoreJsonTraversalExpression;

        private SqlExpression ConvertFromJsonExtract(SqlExpression expression, Type returnType)
            => returnType == typeof(bool)
                ? _sqlExpressionFactory.NonOptimizedEqual(
                    expression,
                    _sqlExpressionFactory.Constant(true, _typeMappingSource.FindMapping(typeof(bool))))
                : expression;

        private SqlExpression ApplyPathLocationTypeMapping(SqlExpression expression)
        {
            var pathLocation = _sqlExpressionFactory.ApplyDefaultTypeMapping(expression);

            // Path locations are usually made of strings. And they should be rendered without surrounding quotes.
            if (pathLocation is SqlConstantExpression sqlConstantExpression &&
                sqlConstantExpression.TypeMapping is SingleStoreStringTypeMapping stringTypeMapping &&
                !stringTypeMapping.IsUnquoted)
            {
                pathLocation = sqlConstantExpression.ApplyTypeMapping(stringTypeMapping.Clone(true));
            }

            return pathLocation;
        }
    }
}
