// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Query.Expressions.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal
{
    public abstract class SingleStoreJsonPocoTranslator : ISingleStoreJsonPocoTranslator
    {
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;
        private readonly RelationalTypeMapping _unquotedStringTypeMapping;
        private readonly RelationalTypeMapping _intTypeMapping;

        public SingleStoreJsonPocoTranslator(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _typeMappingSource = typeMappingSource;
            _sqlExpressionFactory = sqlExpressionFactory;
            _unquotedStringTypeMapping = ((SingleStoreStringTypeMapping)_typeMappingSource.FindMapping(typeof(string))).Clone(unquoted: true);
            _intTypeMapping = _typeMappingSource.FindMapping(typeof(int));
        }

        public virtual SqlExpression Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (instance?.TypeMapping is SingleStoreJsonTypeMapping ||
                instance is SingleStoreJsonTraversalExpression)
            {
                // Path locations need to be rendered without surrounding quotes, because the path itself already
                // has quotes.
                var sqlConstantExpression = _sqlExpressionFactory.ApplyDefaultTypeMapping(
                    _sqlExpressionFactory.Constant(
                        GetJsonPropertyName(member) ?? member.Name,
                        _unquotedStringTypeMapping));

                return TranslateMemberAccess(
                    instance,
                    sqlConstantExpression,
                    returnType);
            }

            return null;
        }

        public abstract string GetJsonPropertyName(MemberInfo member);

        public virtual SqlExpression TranslateMemberAccess(
            [NotNull] SqlExpression instance, [NotNull] SqlExpression member, [NotNull] Type returnType)
        {
            // The first time we see a JSON traversal it's on a column - create a JsonTraversalExpression.
            // Traversals on top of that get appended into the same expression.

            if (instance is ColumnExpression columnExpression &&
                columnExpression.TypeMapping is SingleStoreJsonTypeMapping)
            {
                return ConvertFromJsonExtract(
                    _sqlExpressionFactory.JsonTraversal(
                            columnExpression,
                            returnsText: SingleStoreJsonTraversalExpression.TypeReturnsText(returnType),
                            returnType)
                        .Append(
                            member,
                            returnType,
                            FindPocoTypeMapping(returnType)),
                    returnType);
            }

            if (instance is SingleStoreJsonTraversalExpression prevPathTraversal)
            {
                return prevPathTraversal.Append(
                    member,
                    returnType,
                    FindPocoTypeMapping(returnType));
            }

            return null;
        }

        public virtual SqlExpression TranslateArrayLength([NotNull] SqlExpression expression)
            => expression is SingleStoreJsonTraversalExpression ||
               expression is ColumnExpression columnExpression && columnExpression.TypeMapping is SingleStoreJsonTypeMapping
                ? _sqlExpressionFactory.NullableFunction(
                    "JSON_LENGTH",
                    new[] {expression},
                    typeof(int),
                    _intTypeMapping,
                    false)
                : null;

        protected virtual SqlExpression ConvertFromJsonExtract(SqlExpression expression, Type returnType)
        {
            var unwrappedReturnType = returnType.UnwrapNullableType();
            var typeMapping = FindPocoTypeMapping(returnType);

            switch (Type.GetTypeCode(unwrappedReturnType))
            {
                case TypeCode.Boolean:
                    return _sqlExpressionFactory.NonOptimizedEqual(
                        expression,
                        _sqlExpressionFactory.Constant(true, typeMapping));

                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return _sqlExpressionFactory.Convert(
                        expression,
                        returnType,
                        typeMapping);
            }

            if (unwrappedReturnType == typeof(Guid)
                || unwrappedReturnType == typeof(DateTimeOffset)
                || unwrappedReturnType == typeof(DateOnly)
                || unwrappedReturnType == typeof(TimeOnly))
            {
                return _sqlExpressionFactory.Convert(
                    expression,
                    returnType,
                    typeMapping);
            }

            return expression;
        }

        protected virtual RelationalTypeMapping FindPocoTypeMapping(Type type)
            => GetJsonSpecificTypeMapping(_typeMappingSource.FindMapping(type) ??
                                   _typeMappingSource.FindMapping(type, "json"));

        protected virtual RelationalTypeMapping GetJsonSpecificTypeMapping(RelationalTypeMapping typeMapping)
            => typeMapping is IJsonSpecificTypeMapping jsonSpecificTypeMapping
                ? jsonSpecificTypeMapping.CloneAsJsonCompatible()
                : typeMapping;
    }
}
