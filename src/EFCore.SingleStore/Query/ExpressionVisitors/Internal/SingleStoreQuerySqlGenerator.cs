// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.Expressions.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SingleStoreQuerySqlGenerator : QuerySqlGenerator
    {
        // The order in which the types are specified matters, because types get matched by using StartsWith.
        private static readonly Dictionary<string, string[]> _castMappings = new Dictionary<string, string[]>
        {
            { "signed", new []{ "tinyint", "smallint", "mediumint", "int", "bigint", "bit" }},
            { "decimal(65,30)", new []{ "decimal" } },
            { "double", new []{ "double" } },
            { "float", new []{ "float" } },
            { "binary", new []{ "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob" } },
            { "datetime(6)", new []{ "datetime(6)" } },
            { "datetime", new []{ "datetime" } },
            { "date", new []{ "date" } },
            { "timestamp(6)", new []{ "timestamp(6)" } },
            { "timestamp", new []{ "timestamp" } },
            { "time(6)", new []{ "time(6)" } },
            { "time", new []{ "time" } },
            { "json", new []{ "json" } },
            { "char", new []{ "char", "varchar", "text", "tinytext", "mediumtext", "longtext" } },
            { "nchar", new []{ "nchar", "nvarchar" } },
        };

        private const ulong LimitUpperBound = 18446744073709551610;

        private readonly ISingleStoreOptions _options;
        private string _removeTableAliasOld;
        private string _removeTableAliasNew;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SingleStoreQuerySqlGenerator(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [CanBeNull] ISingleStoreOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SingleStoreJsonTraversalExpression jsonTraversalExpression => VisitJsonPathTraversal(jsonTraversalExpression),
                SingleStoreColumnAliasReferenceExpression columnAliasReferenceExpression => VisitColumnAliasReference(columnAliasReferenceExpression),
                _ => base.VisitExtension(extensionExpression)
            };

        private Expression VisitColumnAliasReference(SingleStoreColumnAliasReferenceExpression columnAliasReferenceExpression)
        {
            Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(columnAliasReferenceExpression.Alias));

            return columnAliasReferenceExpression;
        }

        protected virtual Expression VisitJsonPathTraversal(SingleStoreJsonTraversalExpression expression)
        {
            // If the path contains parameters, then the -> and ->> aliases are not supported by MySQL, because
            // we need to concatenate the path and the parameters.
            // We will use JSON_EXTRACT (and JSON_UNQUOTE if needed) only in this case, because the aliases
            // are much more readable.
            var isSimplePath = expression.Path.All(
                l => l is SqlConstantExpression ||
                     l is SingleStoreJsonArrayIndexExpression e && e.Expression is SqlConstantExpression);

            if (expression.ReturnsText)
            {
                Sql.Append("JSON_UNQUOTE(");
            }

            if (expression.Path.Count > 0)
            {
                Sql.Append("JSON_EXTRACT(");
            }

            Visit(expression.Expression);

            if (expression.Path.Count > 0)
            {
                Sql.Append(", ");

                if (!isSimplePath)
                {
                    Sql.Append("CONCAT(");
                }

                Sql.Append("'$");

                foreach (var location in expression.Path)
                {
                    if (location is SingleStoreJsonArrayIndexExpression arrayIndexExpression)
                    {
                        var isConstantExpression = arrayIndexExpression.Expression is SqlConstantExpression;

                        Sql.Append("[");

                        if (!isConstantExpression)
                        {
                            Sql.Append("', ");
                        }

                        Visit(arrayIndexExpression.Expression);

                        if (!isConstantExpression)
                        {
                            Sql.Append(", '");
                        }

                        Sql.Append("]");
                    }
                    else
                    {
                        Sql.Append(".");
                        Visit(location);
                    }
                }

                Sql.Append("'");

                if (!isSimplePath)
                {
                    Sql.Append(")");
                }

                Sql.Append(")");
            }

            if (expression.ReturnsText)
            {
                Sql.Append(")");
            }

            return expression;
        }

        protected override Expression VisitColumn(ColumnExpression columnExpression)
        {
            if (_removeTableAliasOld is not null &&
                columnExpression.TableAlias == _removeTableAliasOld)
            {
                if (_removeTableAliasNew is not null)
                {
                    Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(_removeTableAliasNew))
                        .Append(".");
                }

                Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(columnExpression.Name));

                return columnExpression;
            }

            return base.VisitColumn(columnExpression);
        }

        protected override Expression VisitTable(TableExpression tableExpression)
        {
            if (_removeTableAliasOld is not null &&
                tableExpression.Alias == _removeTableAliasOld)
            {
                if (_removeTableAliasNew is not null)
                {
                    Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(_removeTableAliasNew))
                        .Append(AliasSeparator);
                }

                Sql.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(tableExpression.Name));

                return tableExpression;
            }

            return base.VisitTable(tableExpression);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("LIMIT ");
                Visit(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                if (selectExpression.Limit == null)
                {
                    // if we want to use Skip() without Take() we have to define the upper limit of LIMIT
                    Sql.AppendLine().Append($"LIMIT {LimitUpperBound}");
                }

                Sql.Append(" OFFSET ");
                Visit(selectExpression.Offset);
            }
        }

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            if (sqlFunctionExpression.Name.StartsWith("@@", StringComparison.Ordinal))
            {
                Sql.Append(sqlFunctionExpression.Name);

                return sqlFunctionExpression;
            }

            return base.VisitSqlFunction(sqlFunctionExpression);
        }

        protected override Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
        {
            Sql.Append("JOIN ");

            if (crossApplyExpression.Table is not TableExpression)
            {
                Sql.Append("LATERAL ");
            }

            Visit(crossApplyExpression.Table);

            Sql.Append(" ON TRUE");

            return crossApplyExpression;
        }

        protected override Expression VisitOuterApply(OuterApplyExpression outerApplyExpression)
        {
            Sql.Append("LEFT JOIN ");

            if (outerApplyExpression.Table is not TableExpression)
            {
                Sql.Append("LATERAL ");
            }

            Visit(outerApplyExpression.Table);

            Sql.Append(" ON TRUE");

            return outerApplyExpression;
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            Check.NotNull(sqlBinaryExpression, nameof(sqlBinaryExpression));

            if ((sqlBinaryExpression.OperatorType == ExpressionType.Add || sqlBinaryExpression.OperatorType == ExpressionType.Subtract) &&
                (sqlBinaryExpression.Type == typeof(System.TimeOnly) || sqlBinaryExpression.Type == typeof(System.TimeSpan)))
            {
                Sql.Append("ADDTIME(");
                Visit(sqlBinaryExpression.Left);
                Sql.Append(", ");
                Visit(sqlBinaryExpression.Right);
                Sql.Append(")");

                return sqlBinaryExpression;
            }

            if (sqlBinaryExpression.OperatorType == ExpressionType.Add &&
                sqlBinaryExpression.Type == typeof(string) &&
                sqlBinaryExpression.Left.TypeMapping?.ClrType == typeof(string) &&
                sqlBinaryExpression.Right.TypeMapping?.ClrType == typeof(string))
            {
                Sql.Append("CONCAT(");
                Visit(sqlBinaryExpression.Left);
                Sql.Append(", ");
                Visit(sqlBinaryExpression.Right);
                Sql.Append(")");
                return sqlBinaryExpression;
            }

            var requiresBrackets = RequiresBrackets(sqlBinaryExpression.Left);
            var requiresCast = (sqlBinaryExpression.OperatorType == ExpressionType.Equal &&
                                (sqlBinaryExpression.Right.Type == typeof(System.TimeOnly) || sqlBinaryExpression.Right.Type == typeof(System.TimeSpan)) &&
                                sqlBinaryExpression.Right.TypeMapping.Converter is null);

            if (requiresBrackets)
            {
                Sql.Append("(");
            }

            Visit(sqlBinaryExpression.Left);

            if (requiresBrackets)
            {
                Sql.Append(")");
            }

            Sql.Append(GetOperator(sqlBinaryExpression));

            // EF uses unary Equal and NotEqual to represent is-null checking.
            // These need to be surrounded with parenthesis in various cases (e.g. where TRUE = x IS NOT NULL).
            // See https://github.com/PomeloFoundation/EntityFrameworkCore.SingleStore/issues/1309
            requiresBrackets = RequiresBrackets(sqlBinaryExpression.Right) ||
                               !requiresBrackets &&
                               sqlBinaryExpression.Right is SqlUnaryExpression sqlUnaryExpression &&
                               (sqlUnaryExpression.OperatorType == ExpressionType.Equal || sqlUnaryExpression.OperatorType == ExpressionType.NotEqual);

            if (requiresBrackets)
            {
                Sql.Append("(");
            }

            Visit(sqlBinaryExpression.Right);

            if (requiresCast)
            {
                Sql.Append(":>time(6)");
            }

            if (requiresBrackets)
            {
                Sql.Append(")");
            }

            return sqlBinaryExpression;
        }

        protected override Expression VisitDelete(DeleteExpression deleteExpression)
        {
            var selectExpression = deleteExpression.SelectExpression;

            if (selectExpression.Offset == null
                && selectExpression.Having == null
                && selectExpression.GroupBy.Count == 0
                && selectExpression.Projection.Count == 0
                && (selectExpression.Tables.Count == 1 || selectExpression.Orderings.Count == 0 && selectExpression.Limit is null))
            {
                var removeSingleTableAlias = selectExpression.Tables.Count == 1 &&
                                             selectExpression.Orderings.Count > 0 || selectExpression.Limit is not null;

                Sql.Append($"DELETE");

                if (!removeSingleTableAlias)
                {
                    Sql.Append($" {Dependencies.SqlGenerationHelper.DelimitIdentifier(deleteExpression.Table.Alias)}");
                }

                Sql.AppendLine().Append("FROM ");

                if (removeSingleTableAlias)
                {
                    _removeTableAliasOld = selectExpression.Tables[0].Alias;
                    _removeTableAliasNew = null;
                }

                GenerateList(selectExpression.Tables, e => Visit(e), sql => sql.AppendLine());

                if (selectExpression.Predicate != null)
                {
                    Sql.AppendLine().Append("WHERE ");

                    Visit(selectExpression.Predicate);
                }

                GenerateOrderings(selectExpression);
                GenerateLimitOffset(selectExpression);

                if (removeSingleTableAlias)
                {
                    _removeTableAliasOld = null;
                }

                return deleteExpression;
            }

            throw new InvalidOperationException(
                RelationalStrings.ExecuteOperationWithUnsupportedOperatorInSqlGeneration(nameof(RelationalQueryableExtensions.ExecuteDelete)));
        }

        protected override Expression VisitUpdate(UpdateExpression updateExpression)
        {
            var selectExpression = updateExpression.SelectExpression;

            if (selectExpression.Offset == null
                && selectExpression.Having == null
                && selectExpression.Orderings.Count == 0
                && selectExpression.GroupBy.Count == 0
                && selectExpression.Projection.Count == 0)
            {
                Sql.Append("UPDATE ");
                GenerateList(selectExpression.Tables, e => Visit(e), sql => sql.AppendLine());

                Sql.AppendLine().Append("SET ");
                Visit(updateExpression.ColumnValueSetters[0].Column);
                Sql.Append(" = ");
                Visit(updateExpression.ColumnValueSetters[0].Value);

                using (Sql.Indent())
                {
                    foreach (var columnValueSetter in updateExpression.ColumnValueSetters.Skip(1))
                    {
                        Sql.AppendLine(",");
                        Visit(columnValueSetter.Column);
                        Sql.Append(" = ");
                        Visit(columnValueSetter.Value);
                    }
                }

                if (selectExpression.Predicate != null)
                {
                    Sql.AppendLine().Append("WHERE ");
                    Visit(selectExpression.Predicate);
                }

                GenerateLimitOffset(selectExpression);

                return updateExpression;
            }

            throw new InvalidOperationException(
                RelationalStrings.ExecuteOperationWithUnsupportedOperatorInSqlGeneration(nameof(RelationalQueryableExtensions.ExecuteUpdate)));
        }

        protected virtual void GenerateList<T>(
            IReadOnlyList<T> items,
            Action<T> generationAction,
            Action<IRelationalCommandBuilder> joinAction = null)
        {
            joinAction ??= (isb => isb.Append(", "));

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    joinAction(Sql);
                }

                generationAction(items[i]);
            }
        }

        private static bool RequiresBrackets(SqlExpression expression)
            => expression is SqlBinaryExpression
               || expression is LikeExpression
               || (expression is SqlUnaryExpression unary
                   && unary.Operand.Type == typeof(bool)
                   && (unary.OperatorType == ExpressionType.Equal
                       || unary.OperatorType == ExpressionType.NotEqual));

        public virtual Expression VisitSingleStoreRegexp(SingleStoreRegexpExpression mySqlRegexpExpression)
        {
            Check.NotNull(mySqlRegexpExpression, nameof(mySqlRegexpExpression));

            Visit(mySqlRegexpExpression.Match);
            Sql.Append(" REGEXP ");
            Visit(mySqlRegexpExpression.Pattern);

            return mySqlRegexpExpression;
        }

        public virtual Expression VisitSingleStoreMatch(SingleStoreMatchExpression mySqlMatchExpression)
        {
            Check.NotNull(mySqlMatchExpression, nameof(mySqlMatchExpression));

            Sql.Append("MATCH ");
            Sql.Append("(");
            Visit(mySqlMatchExpression.Match);
            Sql.Append(")");
            Sql.Append(" AGAINST ");
            Sql.Append("(");
            Visit(mySqlMatchExpression.Against);
            Sql.Append(")");

            return mySqlMatchExpression;
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
            => sqlUnaryExpression.OperatorType == ExpressionType.Convert
                ? VisitConvert(sqlUnaryExpression)
                : base.VisitSqlUnary(sqlUnaryExpression);

        private SqlUnaryExpression VisitConvert(SqlUnaryExpression sqlUnaryExpression)
        {
            var castMapping = GetCastStoreType(sqlUnaryExpression.TypeMapping);

            if (castMapping == "binary")
            {
                Sql.Append("UNHEX(HEX(");
                Visit(sqlUnaryExpression.Operand);
                Sql.Append("))");
                return sqlUnaryExpression;
            }

            // There needs to be no CAST() applied between the exact same store type. This could happen, e.g. if
            // `System.DateTime` and `System.DateTimeOffset` are used in conjunction, because both use different type
            // mappings, but map to the same store type (e.g. `datetime(6)`).
            //
            // There also is no need for a double CAST() to the same type. Due to only rudimentary CAST() support in
            // MySQL, the final store type of a CAST() operation might be different than the store type of the type
            // mapping of the expression (e.g. "float" will be cast to "double"). So we optimize these cases too.
            //
            // An exception is the JSON data type, when used in conjunction with a parameter (like `JsonDocument`).
            // JSON parameters like that will be serialized to string and supplied as a string parameter to MySQL
            // (at least this seems to be the case currently with SingleStoreConnector). To make assignments and comparisons
            // between JSON columns and JSON parameters (supplied as string) work, the string needs to be explicitly
            // converted to JSON.

            var sameInnerCastStoreType = sqlUnaryExpression.Operand is SqlUnaryExpression operandUnary &&
                                         operandUnary.OperatorType == ExpressionType.Convert &&
                                         castMapping.Equals(GetCastStoreType(operandUnary.TypeMapping), StringComparison.OrdinalIgnoreCase);

            if (castMapping == "json" && !_options.ServerVersion.Supports.JsonDataTypeEmulation ||
                !castMapping.Equals(sqlUnaryExpression.Operand.TypeMapping.StoreType, StringComparison.OrdinalIgnoreCase) &&
                !sameInnerCastStoreType)
            {
                var useDecimalToDoubleWorkaround = false;

                if (castMapping.StartsWith("double") &&
                    !_options.ServerVersion.Supports.DoubleCast)
                {
                    useDecimalToDoubleWorkaround = true;
                    castMapping = "decimal(65,30)";
                }

                if (useDecimalToDoubleWorkaround)
                {
                    Sql.Append("(");
                }

                Sql.Append("CAST(");
                Visit(sqlUnaryExpression.Operand);
                Sql.Append(" AS ");
                Sql.Append(castMapping);
                Sql.Append(")");

                // FLOAT and DOUBLE are supported by CAST() as of MySQL 8.0.17.
                // For server versions before that, a workaround is applied, that casts to a DECIMAL,
                // that is then added to 0e0, which results in a DOUBLE.
                // REF: https://dev.mysql.com/doc/refman/8.0/en/number-literals.html
                if (useDecimalToDoubleWorkaround)
                {
                    Sql.Append(" + 0e0)");
                }
            }
            else
            {
                Visit(sqlUnaryExpression.Operand);
            }

            return sqlUnaryExpression;
        }

        private string GetCastStoreType(RelationalTypeMapping typeMapping)
        {
            var storeTypeLower = typeMapping.StoreType.ToLower();
            string castMapping = null;
            foreach (var kvp in _castMappings)
            {
                foreach (var storeType in kvp.Value)
                {
                    if (storeTypeLower.StartsWith(storeType))
                    {
                        castMapping = kvp.Key;
                        break;
                    }
                }

                if (castMapping != null)
                {
                    break;
                }
            }

            if (castMapping == null)
            {
                throw new InvalidOperationException($"Cannot cast from type '{typeMapping.StoreType}'");
            }

            if (castMapping == "signed" && storeTypeLower.Contains("unsigned"))
            {
                castMapping = "unsigned";
            }

            // As of MySQL 8.0.18, a FLOAT cast might unnecessarily drop decimal places and round,
            // so we just keep casting to double instead. SingleStoreConnector ensures, that a System.Single
            // will be returned if expected, even if we return a DOUBLE.
            if (castMapping.StartsWith("float") &&
                !_options.ServerVersion.Supports.FloatCast)
            {
                castMapping = "double";
            }

            return castMapping;
        }

        public virtual Expression VisitSingleStoreComplexFunctionArgumentExpression(SingleStoreComplexFunctionArgumentExpression mySqlComplexFunctionArgumentExpression)
        {
            Check.NotNull(mySqlComplexFunctionArgumentExpression, nameof(mySqlComplexFunctionArgumentExpression));

            var first = true;
            foreach (var argument in mySqlComplexFunctionArgumentExpression.ArgumentParts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Sql.Append(mySqlComplexFunctionArgumentExpression.Delimiter);
                }

                Visit(argument);
            }

            return mySqlComplexFunctionArgumentExpression;
        }

        public virtual Expression VisitSingleStoreCollateExpression(SingleStoreCollateExpression mySqlCollateExpression)
        {
            Check.NotNull(mySqlCollateExpression, nameof(mySqlCollateExpression));

            Sql.Append("CONVERT(");

            Visit(mySqlCollateExpression.ValueExpression);

            Sql.Append($" USING {mySqlCollateExpression.Charset}) COLLATE {mySqlCollateExpression.Collation}");

            return mySqlCollateExpression;
        }

        public virtual Expression VisitSingleStoreBinaryExpression(SingleStoreBinaryExpression mySqlBinaryExpression)
        {
            if (mySqlBinaryExpression.OperatorType == SingleStoreBinaryExpressionOperatorType.NonOptimizedEqual)
            {
                var equalExpression = new SqlBinaryExpression(
                    ExpressionType.Equal,
                    mySqlBinaryExpression.Left,
                    mySqlBinaryExpression.Right,
                    mySqlBinaryExpression.Type,
                    mySqlBinaryExpression.TypeMapping);

                Visit(equalExpression);
            }
            else
            {
                Sql.Append("(");
                Visit(mySqlBinaryExpression.Left);
                Sql.Append(")");

                switch (mySqlBinaryExpression.OperatorType)
                {
                    case SingleStoreBinaryExpressionOperatorType.IntegerDivision:
                        Sql.Append(" DIV ");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Sql.Append("(");
                Visit(mySqlBinaryExpression.Right);
                Sql.Append(")");
            }

            return mySqlBinaryExpression;
        }

        /// <inheritdoc />
        protected override void CheckComposableSql(string sql)
        {
            // MySQL supports CTE (WITH) expressions within subqueries, as well as others,
            // so we allow any raw SQL to be composed over.
        }
    }
}
