// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal;

public class SingleStoreQueryableMethodTranslatingExpressionVisitor : RelationalQueryableMethodTranslatingExpressionVisitor
{
    private readonly ISingleStoreOptions _options;
    private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly SqlAliasManager _sqlAliasManager;

    public SingleStoreQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        RelationalQueryCompilationContext relationalQueryCompilationContext,
        ISingleStoreOptions options)
        : base(dependencies, relationalDependencies, relationalQueryCompilationContext)
    {
        _sqlExpressionFactory = (SingleStoreSqlExpressionFactory)relationalDependencies.SqlExpressionFactory;
        _typeMappingSource = relationalDependencies.TypeMappingSource;
        _sqlAliasManager = relationalQueryCompilationContext.SqlAliasManager;
        _options = options;
    }

    protected SingleStoreQueryableMethodTranslatingExpressionVisitor(
        SingleStoreQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
        _sqlExpressionFactory = parentVisitor._sqlExpressionFactory;
        _typeMappingSource = parentVisitor._typeMappingSource;
        _sqlAliasManager = parentVisitor._sqlAliasManager;
        _options = parentVisitor._options;
    }

    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new SingleStoreQueryableMethodTranslatingExpressionVisitor(this);

    #region Prevent LIMIT LEAST(...)

    // EF Core 10 combines a new row limit with an already existing one into a single `LIMIT LEAST(oldLimit, newLimit)`
    // (e.g. for `Take(x).Take(y)` or `Take(x).First()`). SingleStore (like MySQL) only accepts non-negative integer constants
    // (or parameters, which SingleStoreConnector substitutes client-side) in the LIMIT clause, not expressions.
    // So whenever EF Core would generate LEAST() for the limit, we push the existing limit down into a subquery instead, which
    // is what EF Core did up to version 9.
    //
    // EF Core applies the limit in a private ApplyLimit() method, that is called by TranslateTake(), TranslateFirstOrDefault(),
    // TranslateSingleOrDefault(), TranslateLastOrDefault() and TranslateElementAtOrDefault(). The last two always push down an
    // existing limit themselves before calling ApplyLimit() (via ReverseOrderings() and ApplyOffset()), so only the first three
    // need to be handled here.

    protected override ShapedQueryExpression TranslateTake(ShapedQueryExpression source, Expression count)
    {
        var selectExpression = (SelectExpression)source.QueryExpression;

        if (selectExpression.Limit is not null)
        {
            var newLimit = TranslateExpression(count);
            if (newLimit is null)
            {
                return null;
            }

            PushdownLimitIfLeastWouldBeGenerated(selectExpression, newLimit);
        }

        return base.TranslateTake(source, count);
    }

    protected override ShapedQueryExpression TranslateFirstOrDefault(
        ShapedQueryExpression source,
        LambdaExpression predicate,
        Type returnType,
        bool returnDefault)
    {
        // The base implementation translates the predicate before applying the limit (which pushes down an existing limit, unless
        // the predicate is constant TRUE). We do the same here, so we can inspect the final state before the limit gets applied.
        if (predicate is not null)
        {
            source = TranslateWhere(source, predicate);
            if (source is null)
            {
                return null;
            }
        }

        // FirstOrDefault() always applies a limit of 1.
        PushdownLimitIfLeastWouldBeGenerated(
            (SelectExpression)source.QueryExpression,
            TranslateExpression(Expression.Constant(1)));

        return base.TranslateFirstOrDefault(source, predicate: null, returnType, returnDefault);
    }

    protected override ShapedQueryExpression TranslateSingleOrDefault(
        ShapedQueryExpression source,
        LambdaExpression predicate,
        Type returnType,
        bool returnDefault)
    {
        if (predicate is not null)
        {
            source = TranslateWhere(source, predicate);
            if (source is null)
            {
                return null;
            }
        }

        // SingleOrDefault() applies a constant limit of 2 (or 1 in subqueries). Since only the fact that the new limit is a constant
        // matters for the decision, we can use 2 here.
        PushdownLimitIfLeastWouldBeGenerated(
            (SelectExpression)source.QueryExpression,
            TranslateExpression(Expression.Constant(2)));

        return base.TranslateSingleOrDefault(source, predicate: null, returnType, returnDefault);
    }

    /// <summary>
    ///     Mirrors the decision of EF Core's private <c>RelationalQueryableMethodTranslatingExpressionVisitor.ApplyLimit()</c>:
    ///     If there is an existing limit that EF Core would combine with the new one using LEAST(), push it down into a subquery
    ///     first, so EF Core can just set the new limit on the outer query.
    /// </summary>
    protected virtual void PushdownLimitIfLeastWouldBeGenerated(SelectExpression selectExpression, SqlExpression newLimit)
    {
        if (selectExpression.Limit is not { } oldLimit)
        {
            return;
        }

        // EF Core does not use LEAST() if both limits are constants (it picks the smaller one), or if both limits are identical.
        var bothConstants = oldLimit is SqlConstantExpression { Value: int } &&
                            newLimit is SqlConstantExpression { Value: int };

        if (!bothConstants && !oldLimit.Equals(newLimit))
        {
            selectExpression.PushdownIntoSubquery();
        }
    }

    #endregion

    protected override bool IsNaturallyOrdered(SelectExpression selectExpression)
    {
        return selectExpression is
               {
                   Tables: [var mainTable, ..],
                   Orderings:
                   [
                       {
                           Expression: ColumnExpression { Name: "key", TableAlias: var orderingTable } orderingColumn,
                           IsAscending: true
                       }
                   ]
               }
               && orderingTable == mainTable.Alias
               && IsJsonEachKeyColumn(selectExpression, orderingColumn);

        bool IsJsonEachKeyColumn(SelectExpression selectExpression, ColumnExpression orderingColumn)
            => selectExpression.Tables.FirstOrDefault(t => t.Alias == orderingColumn.TableAlias)?.UnwrapJoin() is TableExpressionBase table
               && (table is SingleStoreJsonTableExpression
                   || (table is SelectExpression subquery
                       && subquery.Projection.FirstOrDefault(p => p.Alias == "key")?.Expression is ColumnExpression projectedColumn
                       && IsJsonEachKeyColumn(subquery, projectedColumn)));
    }

    #region ExecuteDelete

    // The shaper of the entity being deleted by the ExecuteDelete() currently being translated. EF Core's
    // IsValidSelectExpressionForExecuteDelete() does not get told which table is the delete target, so we remember the shaper here
    // to find it ourselves.
    private StructuralTypeShaperExpression _executeDeleteShaper;

    protected override DeleteExpression TranslateExecuteDelete(ShapedQueryExpression source)
    {
        // EF Core calls this method recursively for its subquery fallback, so restore the outer value afterwards.
        var previousShaper = _executeDeleteShaper;
        _executeDeleteShaper = UnwrapIncludes(source.ShaperExpression) as StructuralTypeShaperExpression;

        try
        {
            return base.TranslateExecuteDelete(source);
        }
        finally
        {
            _executeDeleteShaper = previousShaper;
        }

        static Expression UnwrapIncludes(Expression shaper)
        {
            while (shaper is IncludeExpression includeExpression)
            {
                shaper = includeExpression.EntityExpression;
            }

            return shaper;
        }
    }

    // SingleStore's DELETE grammar has no ORDER BY, and LIMIT is only supported in single-table DELETE statements.
    // Multi-table DELETE is supported with any join type, as long as the rows are deleted from the first (left-most) table:
    // SingleStore rejects deleting from any other table with "Operation 'Update/Delete right table of a join' is not allowed".
    // Whenever we return false, EF Core falls back to `DELETE FROM t WHERE <key> IN/EXISTS (<original query>)`.
    protected override bool IsValidSelectExpressionForExecuteDelete(SelectExpression selectExpression)
        => selectExpression is
           {
               Tables: [TableExpression, ..],
               Orderings: [],
               Offset: null,
               GroupBy: [],
               Having: null
           } &&
           (selectExpression.Tables.Count == 1 ||
            (selectExpression.Limit is null && IsDeleteTargetFirstTable(selectExpression)));

    private bool IsDeleteTargetFirstTable(SelectExpression selectExpression)
    {
        if (_executeDeleteShaper is not { ValueBufferExpression: ProjectionBindingExpression projectionBinding } shaper ||
            selectExpression.GetProjection(projectionBinding) is not StructuralTypeProjectionExpression projection)
        {
            return false;
        }

        // Same way EF Core determines the delete target in TranslateExecuteDelete(): via the table of the entity's first property.
        var column = projection.BindProperty(shaper.StructuralType.GetProperties().First());

        return column.TableAlias == selectExpression.Tables[0].Alias;
    }

    #endregion ExecuteDelete

    protected override bool IsValidSelectExpressionForExecuteUpdate(
        SelectExpression selectExpression,
        TableExpressionBase targetTable,
        [NotNullWhen(true)] out TableExpression tableExpression)
    {
        if (selectExpression is
            {
                Offset: null,
                IsDistinct: false,
                GroupBy: [],
                Having: null,
                Orderings: []
            })
        {
            TableExpressionBase table;
            if (selectExpression.Tables.Count == 1)
            {
                table = selectExpression.Tables[0];
            }
            else
            {
                table = targetTable;

                if (selectExpression.Tables.Count > 1 &&
                    table is JoinExpressionBase joinExpressionBase)

                {
                    table = joinExpressionBase.Table;
                }
            }

            if (table is TableExpression te)
            {
                tableExpression = te;
                return true;
            }
        }

        tableExpression = null;
        return false;
    }

    protected override ShapedQueryExpression TranslateAny(ShapedQueryExpression source, LambdaExpression predicate)
    {
        // Simplify x.Array.Any() => JSON_LENGTH(x.Array) > 0 instead of WHERE EXISTS (SELECT 1 FROM JSON_TABLE(x.Array))
        if (predicate is null
            && source.QueryExpression is SelectExpression
            {
                Tables: [TableValuedFunctionExpression { Name: "JSON_TABLE", Schema: null, IsBuiltIn: true, Arguments: [var array] }],
                GroupBy: [],
                Having: null,
                IsDistinct: false,
                Limit: null,
                Offset: null
            })
        {
            var translation =
                _sqlExpressionFactory.GreaterThan(
                    _sqlExpressionFactory.NullableFunction(
                        "JSON_LENGTH",
                        new[] { array },
                        typeof(int)),
                    _sqlExpressionFactory.Constant(0));

            return source.UpdateQueryExpression(new SelectExpression(translation, _sqlAliasManager));
        }

        return base.TranslateAny(source, predicate);
    }

    protected override ShapedQueryExpression TranslateElementAtOrDefault(
        ShapedQueryExpression source,
        Expression index,
        bool returnDefault)
    {
        if (!returnDefault
            && source.QueryExpression is SelectExpression
            {
                Tables:
                [
                    SingleStoreJsonTableExpression
                    {
                        Name: "JSON_TABLE", Schema: null, IsBuiltIn: true, JsonExpression: var jsonArrayColumn
                    } jsonEachExpression
                ],
                GroupBy: [],
                Having: null,
                IsDistinct: false,
                Orderings: [{ Expression: ColumnExpression { Name: "key" } orderingColumn, IsAscending: true }],
                Limit: null,
                Offset: null
            } selectExpression
            && orderingColumn.TableAlias == jsonEachExpression.Alias
            && TranslateExpression(index) is { } translatedIndex)
        {
            // Index on JSON array

            // Extract the column projected out of the source, and simplify the subquery to a simple JsonScalarExpression
            var shaperExpression = source.ShaperExpression;
            if (shaperExpression is UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression
                && unaryExpression.Operand.Type.IsNullableType()
                && unaryExpression.Operand.Type.UnwrapNullableType() == unaryExpression.Type)
            {
                shaperExpression = unaryExpression.Operand;
            }

            if (shaperExpression is ProjectionBindingExpression projectionBindingExpression
                && selectExpression.GetProjection(projectionBindingExpression) is ColumnExpression projectionColumn)
            {
                SqlExpression translation = new JsonScalarExpression(
                    jsonArrayColumn,
                    new[] { new PathSegment(translatedIndex) },
                    projectionColumn.Type,
                    projectionColumn.TypeMapping,
                    projectionColumn.IsNullable);

                // If we have a type mapping (i.e. translating over a column rather than a parameter), apply any necessary server-side
                // conversions.
                if (projectionColumn.TypeMapping is not null)
                {
                    translation = ApplyJsonSqlConversion(
                        translation, _sqlExpressionFactory, projectionColumn.TypeMapping, projectionColumn.IsNullable);
                }

                return source.UpdateQueryExpression(new SelectExpression(translation, _sqlAliasManager));
            }
        }

        return base.TranslateElementAtOrDefault(source, index, returnDefault);
    }

    // TODO: Implement for EF Core 7 JSON support.
    protected override ShapedQueryExpression TransformJsonQueryToTable(JsonQueryExpression jsonQueryExpression)
    {
        return base.TransformJsonQueryToTable(jsonQueryExpression);
    }

    protected override ShapedQueryExpression TranslatePrimitiveCollection(SqlExpression sqlExpression, IProperty property, string tableAlias)
    {
        // TODO: PLAT-7535 (primitive collections support)
        if (!_options.PrimitiveCollectionsSupport)
        {
            throw new InvalidOperationException(
                CoreStrings.TranslationFailedWithDetails(
                    sqlExpression.Print(),
                    "Primitive collections support has not been enabled."));
        }

        // if (!_options.ServerVersion.Supports.JsonTableImplementationUsesImplicitLateralJoin &&
        //     sqlExpression is ColumnExpression)
        // {
        //     // MariaDB will just return wrong results if the column of an outer table is being referenced.
        //     return null;
        // }

        // Generate the JSON_TABLE() function expression, and wrap it in a SelectExpression.

        // Note that where the elementTypeMapping is known (i.e. collection columns), we immediately generate JSON_TABLE() with a COLUMNS clause
        // (i.e. with a columnInfo), which determines the type conversion to apply to the JSON elements coming out.
        // For parameter collections, the element type mapping will only be inferred and applied later (see
        // SingleStoreInferredTypeMappingApplier below), at which point the we'll apply it to add the COLUMNS clause.
        var elementTypeMapping = (RelationalTypeMapping)sqlExpression.TypeMapping?.ElementTypeMapping;

        var jsonTableExpression = new SingleStoreJsonTableExpression(
            tableAlias,
            sqlExpression,
            new[] { new PathSegment(_sqlExpressionFactory.Constant("*", RelationalTypeMapping.NullMapping)) },
            elementTypeMapping is not null
                ? new[]
                {
                    new SingleStoreJsonTableExpression.ColumnInfo
                    {
                        Name = "value",
                        TypeMapping = elementTypeMapping,
                        Path = new[] { new PathSegment(_sqlExpressionFactory.Constant(0, _typeMappingSource.FindMapping(typeof(int)))) },
                    }
                }
                : null);

        var elementClrType = sqlExpression.Type.GetSequenceType();

        // If this is a collection property, get the element's nullability out of metadata. Otherwise, this is a parameter property, in
        // which case we only have the CLR type (note that we cannot produce different SQLs based on the nullability of an *element* in
        // a parameter collection - our caching mechanism only supports varying by the nullability of the parameter itself (i.e. the
        // collection).
        var isElementNullable = property?.GetElementType()!.IsNullable;

        var keyColumnTypeMapping = _typeMappingSource.FindMapping(typeof(int))!;

#pragma warning disable EF1001 // Internal EF Core API usage.
        var selectExpression = new SelectExpression(
            [jsonTableExpression],
            new ColumnExpression(
                "value",
                tableAlias,
                elementClrType.UnwrapNullableType(),
                elementTypeMapping,
                isElementNullable ?? elementClrType.IsNullableType()),
            identifier: [(new ColumnExpression("key", tableAlias, typeof(int), keyColumnTypeMapping, nullable: false), keyColumnTypeMapping.Comparer)],
            _sqlAliasManager);
#pragma warning restore EF1001 // Internal EF Core API usage.

        // JSON_TABLE() doesn't guarantee the ordering of the elements coming out; when using JSON_TABLE() without COLUMNS, a [key] column is returned
        // with the JSON array's ordering, which we can ORDER BY; this option doesn't exist with JSON_TABLE() with COLUMNS, unfortunately.
        // However, JSON_TABLE() with COLUMNS has better performance, and also applies JSON-specific conversions we cannot be done otherwise
        // (e.g. JSON_TABLE() with COLUMNS does base64 decoding for VARBINARY).
        // Here we generate JSON_TABLE() with COLUMNS, but also add an ordering by [key] - this is a temporary invalid representation.
        // In MySqlQueryTranslationPostprocessor, we'll post-process the expression; if the ORDER BY was stripped (e.g. because of
        // IN, EXISTS or a set operation), we'll just leave the JSON_TABLE() with COLUMNS. If not, we'll convert the JSON_TABLE() with COLUMNS to an
        // JSON_TABLE() without COLUMNS.
        // Note that the JSON_TABLE() 'key' column is an nvarchar - we convert it to an int before sorting.
        selectExpression.AppendOrdering(
            new OrderingExpression(
                selectExpression.CreateColumnExpression(
                    jsonTableExpression,
                    "key",
                    typeof(uint),
                    typeMapping: _typeMappingSource.FindMapping(typeof(uint)),
                    columnNullable: false),
                ascending: true));

        var shaperExpression = (Expression)new ProjectionBindingExpression(selectExpression, new ProjectionMember(), elementClrType.MakeNullable());
        if (shaperExpression.Type != elementClrType)
        {
            Check.DebugAssert(
                elementClrType.MakeNullable() == shaperExpression.Type,
                "expression.Type must be nullable of targetType");

            shaperExpression = Expression.Convert(shaperExpression, elementClrType);
        }

        return new ShapedQueryExpression(selectExpression, shaperExpression);
    }

    /// <summary>
    /// Wraps the given expression with any SQL logic necessary to convert a value coming out of a JSON document into the relational value
    /// represented by the given type mapping.
    /// </summary>
    private static SqlExpression ApplyJsonSqlConversion(
        SqlExpression expression,
        ISqlExpressionFactory sqlExpressionFactory,
        RelationalTypeMapping typeMapping,
        bool isNullable)
        => typeMapping switch
        {
            // TODO: EF Core 8 - Decide between UNHEX() and FROM_BASE64().
            ByteArrayTypeMapping => sqlExpressionFactory.Function("FROM_BASE64", new[] { expression }, isNullable, new[] { true }, typeof(byte[]), typeMapping),
            _ => expression
        };

    private sealed class FakeMemberInfo : MemberInfo
    {
        public FakeMemberInfo(string name)
            => Name = name;

        public override string Name { get; }

        public override object[] GetCustomAttributes(bool inherit)
            => throw new NotSupportedException();
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            => throw new NotSupportedException();
        public override bool IsDefined(Type attributeType, bool inherit)
            => throw new NotSupportedException();
        public override Type DeclaringType => throw new NotSupportedException();
        public override MemberTypes MemberType => throw new NotSupportedException();
        public override Type ReflectedType => throw new NotSupportedException();
    }
}
