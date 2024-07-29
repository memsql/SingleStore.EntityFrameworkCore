// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
    {
        private readonly ISingleStoreOptions _options;
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreParameterBasedSqlProcessor(
            RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            ISingleStoreOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _sqlExpressionFactory = (SingleStoreSqlExpressionFactory)Dependencies.SqlExpressionFactory;
            _options = options;
        }

        public override Expression Optimize(
            Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
            out bool canCache)
        {
            queryExpression = base.Optimize(queryExpression, parametersValues, out canCache);

            if (_options.ServerVersion.Supports.SingleStoreBugLimit0Offset0ExistsWorkaround)
            {
                queryExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Process(queryExpression, parametersValues, out var canCache2);

                canCache &= canCache2;
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                queryExpression = new SingleStoreBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(queryExpression);
            }

            queryExpression = (SelectExpression)new SingleStoreHavingExpressionVisitor(_sqlExpressionFactory).Visit(queryExpression);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            queryExpression = (SelectExpression)new SingleStoreCompatibilityExpressionVisitor(_options).Visit(queryExpression);

            return queryExpression;
        }

        /// <inheritdoc />
        protected override Expression ProcessSqlNullability(
        Expression queryExpression,
            IReadOnlyDictionary<string, object?> parametersValues,
        out bool canCache)
        {
            Check.NotNull(queryExpression, nameof(queryExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            queryExpression = new SingleStoreSqlNullabilityProcessor(Dependencies, UseRelationalNulls)
                .Process(queryExpression, parametersValues, out canCache);

            return queryExpression;
        }
    }
}
