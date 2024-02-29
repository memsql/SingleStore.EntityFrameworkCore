// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Query.Expressions.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Utilities;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreSqlExpressionFactory : SqlExpressionFactory
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly RelationalTypeMapping _boolTypeMapping;

        public SingleStoreSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies)
            : base(dependencies)
        {
            _typeMappingSource = dependencies.TypeMappingSource;
            _boolTypeMapping = _typeMappingSource.FindMapping(typeof(bool));
        }

        public virtual RelationalTypeMapping FindMapping(
            [NotNull] Type type,
            [CanBeNull] string storeTypeName,
            bool keyOrIndex = false,
            bool? unicode = null,
            int? size = null,
            bool? rowVersion = null,
            bool? fixedLength = null,
            int? precision = null,
            int? scale = null)
            => _typeMappingSource.FindMapping(
                type,
                storeTypeName,
                keyOrIndex,
                unicode,
                size,
                rowVersion,
                fixedLength,
                precision,
                scale);

        #region Expression factory methods

        /// <summary>
        /// Use for any function that could return `NULL` for *any* reason.
        /// </summary>
        /// <param name="name">The SQL name of the function.</param>
        /// <param name="arguments">The arguments of the function.</param>
        /// <param name="returnType">The CLR return type of the function.</param>
        /// <param name="onlyNullWhenAnyNullPropagatingArgumentIsNull">
        /// Set to `false` if the function can return `NULL` even if all of the arguments are not `NULL`. This will disable null-related
        /// optimizations by EF Core.
        /// </param>
        /// <remarks>See https://github.com/dotnet/efcore/issues/23042</remarks>
        /// <returns>The function expression.</returns>
        public virtual SqlFunctionExpression NullableFunction(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type returnType,
            bool onlyNullWhenAnyNullPropagatingArgumentIsNull)
            => NullableFunction(name, arguments, returnType, null, onlyNullWhenAnyNullPropagatingArgumentIsNull);

        /// <summary>
        /// Use for any function that could return `NULL` for *any* reason.
        /// </summary>
        /// <param name="name">The SQL name of the function.</param>
        /// <param name="arguments">The arguments of the function.</param>
        /// <param name="returnType">The CLR return type of the function.</param>
        /// <param name="typeMapping">The optional type mapping of the function.</param>
        /// <param name="onlyNullWhenAnyNullPropagatingArgumentIsNull">
        ///     Set to `false` if the function can return `NULL` even if all of the arguments are not `NULL`. This will disable null-related
        ///     optimizations by EF Core.
        /// </param>
        /// <param name="argumentsPropagateNullability">
        ///     The optional nullability array of the function.
        ///     If omited and <paramref name="onlyNullWhenAnyNullPropagatingArgumentIsNull"/> is
        ///     `true` (the default), all parameters will propagate nullability (meaning if any parameter is `NULL`, the function will
        ///     automatically return `NULL` as well).
        ///     If <paramref name="onlyNullWhenAnyNullPropagatingArgumentIsNull"/> is explicitly set to `false`, the
        ///     null propagating capabilities of the arguments don't matter at all anymore, because the function will never be optimized by
        ///     EF Core in the first place.
        /// </param>
        /// <remarks>See https://github.com/dotnet/efcore/issues/23042</remarks>
        /// <returns>The function expression.</returns>
        public virtual SqlFunctionExpression NullableFunction(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type returnType,
            RelationalTypeMapping typeMapping = null,
            bool onlyNullWhenAnyNullPropagatingArgumentIsNull = true,
            IEnumerable<bool> argumentsPropagateNullability = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(returnType, nameof(returnType));

            var typeMappedArguments = new List<SqlExpression>();

            foreach (var argument in arguments)
            {
                typeMappedArguments.Add(ApplyDefaultTypeMapping(argument));
            }

            return new SqlFunctionExpression(
                name,
                typeMappedArguments,
                true,
                onlyNullWhenAnyNullPropagatingArgumentIsNull
                    ? (argumentsPropagateNullability ?? Statics.GetTrueValues(typeMappedArguments.Count))
                    : Statics.GetFalseValues(typeMappedArguments.Count),
                returnType,
                typeMapping);
        }

        /// <summary>
        /// Use for any function that will never return `NULL`.
        /// </summary>
        /// <param name="name">The SQL name of the function.</param>
        /// <param name="arguments">The arguments of the function.</param>
        /// <param name="returnType">The CLR return type of the function.</param>
        /// <param name="typeMapping">The optional type mapping of the function.</param>
        /// <remarks>See https://github.com/dotnet/efcore/issues/23042</remarks>
        /// <returns>The function expression.</returns>
        public virtual SqlFunctionExpression NonNullableFunction(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type returnType,
            RelationalTypeMapping typeMapping = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(returnType, nameof(returnType));

            var typeMappedArguments = new List<SqlExpression>();

            foreach (var argument in arguments)
            {
                typeMappedArguments.Add(ApplyDefaultTypeMapping(argument));
            }

            return new SqlFunctionExpression(
                name,
                typeMappedArguments,
                false,
                Statics.GetFalseValues(typeMappedArguments.Count),
                returnType,
                typeMapping);
        }

        public virtual SingleStoreComplexFunctionArgumentExpression ComplexFunctionArgument(
            IEnumerable<SqlExpression> argumentParts,
            string delimiter,
            Type argumentType,
            RelationalTypeMapping typeMapping = null)
        {
            var typeMappedArgumentParts = new List<SqlExpression>();

            foreach (var argument in argumentParts)
            {
                typeMappedArgumentParts.Add(ApplyDefaultTypeMapping(argument));
            }

            return (SingleStoreComplexFunctionArgumentExpression)ApplyTypeMapping(
                new SingleStoreComplexFunctionArgumentExpression(
                    typeMappedArgumentParts,
                    delimiter,
                    argumentType,
                    typeMapping),
                typeMapping);
        }

        public virtual SingleStoreCollateExpression Collate(
            SqlExpression valueExpression,
            string charset,
            string collation)
            => (SingleStoreCollateExpression)ApplyDefaultTypeMapping(
                new SingleStoreCollateExpression(
                    valueExpression,
                    charset,
                    collation,
                    null));

        public virtual SingleStoreRegexpExpression Regexp(
            SqlExpression match,
            SqlExpression pattern)
            => (SingleStoreRegexpExpression)ApplyDefaultTypeMapping(
                new SingleStoreRegexpExpression(
                    match,
                    pattern,
                    null));

        public virtual SingleStoreBinaryExpression SingleStoreIntegerDivide(
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping = null)
            => MakeBinary(
                SingleStoreBinaryExpressionOperatorType.IntegerDivision,
                left,
                right,
                typeMapping);

        public virtual SingleStoreBinaryExpression NonOptimizedEqual(
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping = null)
            => MakeBinary(
                SingleStoreBinaryExpressionOperatorType.NonOptimizedEqual,
                left,
                right,
                typeMapping);

        public virtual SingleStoreColumnAliasReferenceExpression ColumnAliasReference(
            string alias,
            SqlExpression expression,
            Type type,
            RelationalTypeMapping typeMapping = null)
            => new SingleStoreColumnAliasReferenceExpression(alias, expression, type, typeMapping);

        #endregion Expression factory methods

        public virtual SingleStoreBinaryExpression MakeBinary(
            SingleStoreBinaryExpressionOperatorType operatorType,
            SqlExpression left,
            SqlExpression right,
            RelationalTypeMapping typeMapping)
        {
            var returnType = left.Type;

            return (SingleStoreBinaryExpression)ApplyTypeMapping(
                new SingleStoreBinaryExpression(
                    operatorType,
                    left,
                    right,
                    returnType,
                    null),
                typeMapping);
        }

        public virtual SingleStoreMatchExpression MakeMatch(
            SqlExpression match,
            SqlExpression against)
        {
            return (SingleStoreMatchExpression)ApplyDefaultTypeMapping(
                new SingleStoreMatchExpression(
                    match,
                    against,
                    null));
        }

        public virtual SingleStoreJsonTraversalExpression JsonTraversal(
            [NotNull] SqlExpression expression,
            bool returnsText,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping = null)
            => new SingleStoreJsonTraversalExpression(
                ApplyDefaultTypeMapping(expression),
                returnsText,
                type,
                typeMapping);

        public virtual SingleStoreJsonArrayIndexExpression JsonArrayIndex(
            [NotNull] SqlExpression expression)
            => JsonArrayIndex(expression, typeof(int));

        public virtual SingleStoreJsonArrayIndexExpression JsonArrayIndex(
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping = null)
            => (SingleStoreJsonArrayIndexExpression)ApplyDefaultTypeMapping(
                new SingleStoreJsonArrayIndexExpression(
                    ApplyDefaultTypeMapping(expression),
                    type,
                    typeMapping));

        public override SqlExpression ApplyTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
            => sqlExpression is not { TypeMapping: null }
                ? sqlExpression
                : ApplyNewTypeMapping(sqlExpression, typeMapping);

        private SqlExpression ApplyNewTypeMapping(SqlExpression sqlExpression, RelationalTypeMapping typeMapping)
            => sqlExpression switch
            {
                // Customize handling for binary expressions.
                SqlBinaryExpression e => ApplyTypeMappingOnSqlBinary(e, typeMapping),

                // MySQL specific expression types:
                SingleStoreComplexFunctionArgumentExpression e => ApplyTypeMappingOnComplexFunctionArgument(e),
                SingleStoreCollateExpression e => ApplyTypeMappingOnCollate(e),
                SingleStoreRegexpExpression e => ApplyTypeMappingOnRegexp(e),
                SingleStoreBinaryExpression e => ApplyTypeMappingOnSingleStoreBinary(e, typeMapping),
                SingleStoreMatchExpression e => ApplyTypeMappingOnMatch(e),
                SingleStoreJsonArrayIndexExpression e => e.ApplyTypeMapping(typeMapping),

                _ => base.ApplyTypeMapping(sqlExpression, typeMapping)
            };

        private SqlBinaryExpression ApplyTypeMappingOnSqlBinary(SqlBinaryExpression sqlBinaryExpression, RelationalTypeMapping typeMapping)
        {
            // The default SqlExpressionFactory behavior is to assume that the two operands have the same type, and so to infer one side's
            // mapping from the other if needed. Here we take care of some heterogeneous operand cases where this doesn't work.

            var left = sqlBinaryExpression.Left;
            var right = sqlBinaryExpression.Right;

            var newSqlBinaryExpression = (SqlBinaryExpression)base.ApplyTypeMapping(sqlBinaryExpression, typeMapping);

            // Handle the special case, that a JSON value is compared to a string (e.g. when used together with
            // JSON_EXTRACT()).
            // The string argument should not be interpreted as a JSON value, which it normally would due to inference
            // if its type mapping hasn't been explicitly set before, but just as a string.
            if (newSqlBinaryExpression.Left.TypeMapping is SingleStoreJsonTypeMapping newLeftTypeMapping &&
                newLeftTypeMapping.ClrType == typeof(string) &&
                right.TypeMapping is null &&
                right.Type == typeof(string))
            {
                newSqlBinaryExpression = new SqlBinaryExpression(
                    sqlBinaryExpression.OperatorType,
                    ApplyTypeMapping(left, newLeftTypeMapping),
                    ApplyTypeMapping(right, _typeMappingSource.FindMapping(right.Type)),
                    newSqlBinaryExpression.Type,
                    newSqlBinaryExpression.TypeMapping);
            }
            else if (newSqlBinaryExpression.Right.TypeMapping is SingleStoreJsonTypeMapping newRightTypeMapping &&
                     newRightTypeMapping.ClrType == typeof(string) &&
                     left.TypeMapping is null &&
                     left.Type == typeof(string))
            {
                newSqlBinaryExpression = new SqlBinaryExpression(
                    sqlBinaryExpression.OperatorType,
                    ApplyTypeMapping(left, _typeMappingSource.FindMapping(left.Type)),
                    ApplyTypeMapping(right, newRightTypeMapping),
                    newSqlBinaryExpression.Type,
                    newSqlBinaryExpression.TypeMapping);
            }

            return newSqlBinaryExpression;
        }

        private SingleStoreComplexFunctionArgumentExpression ApplyTypeMappingOnComplexFunctionArgument(SingleStoreComplexFunctionArgumentExpression complexFunctionArgumentExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(complexFunctionArgumentExpression.ArgumentParts.ToArray())
                                      ?? (complexFunctionArgumentExpression.Type.IsArray
                                          ? _typeMappingSource.FindMapping(
                                              complexFunctionArgumentExpression.Type.GetElementType() ??
                                              complexFunctionArgumentExpression.Type)
                                          : _typeMappingSource.FindMapping(complexFunctionArgumentExpression.Type));

            return new SingleStoreComplexFunctionArgumentExpression(
                complexFunctionArgumentExpression.ArgumentParts,
                complexFunctionArgumentExpression.Delimiter,
                complexFunctionArgumentExpression.Type,
                inferredTypeMapping ?? complexFunctionArgumentExpression.TypeMapping);
        }

        private SingleStoreCollateExpression ApplyTypeMappingOnCollate(SingleStoreCollateExpression collateExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(collateExpression.ValueExpression)
                                      ?? _typeMappingSource.FindMapping(collateExpression.ValueExpression.Type);

            return new SingleStoreCollateExpression(
                ApplyTypeMapping(collateExpression.ValueExpression, inferredTypeMapping),
                collateExpression.Charset,
                collateExpression.Collation,
                inferredTypeMapping ?? collateExpression.TypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnMatch(SingleStoreMatchExpression matchExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(matchExpression.Match) ??
                                      _typeMappingSource.FindMapping(matchExpression.Match.Type);

            return new SingleStoreMatchExpression(
                ApplyTypeMapping(matchExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(matchExpression.Against, inferredTypeMapping),
                _boolTypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnRegexp(SingleStoreRegexpExpression regexpExpression)
        {
            var inferredTypeMapping = ExpressionExtensions.InferTypeMapping(regexpExpression.Match)
                                      ?? _typeMappingSource.FindMapping(regexpExpression.Match.Type);

            return new SingleStoreRegexpExpression(
                ApplyTypeMapping(regexpExpression.Match, inferredTypeMapping),
                ApplyTypeMapping(regexpExpression.Pattern, inferredTypeMapping),
                _boolTypeMapping);
        }

        private SqlExpression ApplyTypeMappingOnSingleStoreBinary(
            SingleStoreBinaryExpression sqlBinaryExpression,
            RelationalTypeMapping typeMapping)
        {
            var left = sqlBinaryExpression.Left;
            var right = sqlBinaryExpression.Right;

            Type resultType;
            RelationalTypeMapping resultTypeMapping;
            RelationalTypeMapping inferredTypeMapping;

            switch (sqlBinaryExpression.OperatorType)
            {
                case SingleStoreBinaryExpressionOperatorType.NonOptimizedEqual:
                    inferredTypeMapping = ExpressionExtensions.InferTypeMapping(left, right)
                                          // We avoid object here since the result does not get typeMapping from outside.
                                          ?? (left.Type != typeof(object)
                                              ? _typeMappingSource.FindMapping(left.Type)
                                              : _typeMappingSource.FindMapping(right.Type));
                    resultType = typeof(bool);
                    resultTypeMapping = _boolTypeMapping;
                    break;

                case SingleStoreBinaryExpressionOperatorType.IntegerDivision:
                    inferredTypeMapping = typeMapping ?? ExpressionExtensions.InferTypeMapping(left, right);
                    resultType = inferredTypeMapping?.ClrType ?? left.Type;
                    resultTypeMapping = inferredTypeMapping;
                    break;

                default:
                    throw new InvalidOperationException($"Incorrect {nameof(SingleStoreBinaryExpression.OperatorType)} for {nameof(SingleStoreBinaryExpression)}");
            }

            return new SingleStoreBinaryExpression(
                sqlBinaryExpression.OperatorType,
                ApplyTypeMapping(left, inferredTypeMapping),
                ApplyTypeMapping(right, inferredTypeMapping),
                resultType,
                resultTypeMapping);
        }
    }
}
