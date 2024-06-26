﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    /// When using constant values in an LEFT JOIN, an later an ORDER BY is applied, MySQL 5.7+ will incorrectly return a NULL values for
    /// some columns.
    /// This is not an issue with any MariaDB release and not an issue with MySQL 5.6.
    ///
    /// See https://bugs.mysql.com/bug.php?id=96947
    ///     https://github.com/OData/WebApi/issues/2124
    ///     https://github.com/PomeloFoundation/EntityFrameworkCore.SingleStore/issues/1293
    /// </summary>
    public class SingleStoreBug96947WorkaroundExpressionVisitor : ExpressionVisitor
    {
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        private bool _usesOrderBy;
        private bool _insideLeftJoin;
        private bool _insideLeftJoinSelect;

        public SingleStoreBug96947WorkaroundExpressionVisitor(SingleStoreSqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                LeftJoinExpression leftJoinExpression => VisitLeftJoin(leftJoinExpression),
                SelectExpression selectExpression => VisitSelect(selectExpression),
                ProjectionExpression projectionExpression => VisitProjection(projectionExpression),
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual Expression VisitLeftJoin(LeftJoinExpression leftJoinExpression)
        {
            var oldInsideLeftJoin = _insideLeftJoin;

            _insideLeftJoin = true;

            var expression = base.VisitExtension(leftJoinExpression);

            _insideLeftJoin = oldInsideLeftJoin;

            return expression;
        }

        protected virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            var oldInsideLeftJoinSelect = _insideLeftJoinSelect;

            if (_insideLeftJoin)
            {
                _insideLeftJoinSelect = !_insideLeftJoinSelect;
            }

            var oldUsesOrderBy = _usesOrderBy;

            _usesOrderBy = selectExpression.Orderings.Count > 0 ||
                           oldUsesOrderBy;

            var expression = base.VisitExtension(selectExpression);

            _usesOrderBy = oldUsesOrderBy;
            _insideLeftJoinSelect = oldInsideLeftJoinSelect;

            return expression;
        }

        protected virtual Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            if (_insideLeftJoinSelect &&
                _usesOrderBy)
            {
                var expression = (SqlExpression)Visit(projectionExpression.Expression);

                // Parameters trigger this bug as well, because they get inlined by SingleStoreConnector and
                // become constant values in the end.
                if (expression is SqlConstantExpression ||
                    expression is SqlParameterExpression)
                {
                    expression = _sqlExpressionFactory.Convert(
                        expression,
                        projectionExpression.Type,
                        expression.TypeMapping);
                }

                return projectionExpression.Update(expression);
            }

            return base.VisitExtension(projectionExpression);
        }
    }
}
