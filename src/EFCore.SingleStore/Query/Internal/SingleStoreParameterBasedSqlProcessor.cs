// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using JetBrains.Annotations;
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
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            bool useRelationalNulls,
            ISingleStoreOptions options)
            : base(dependencies, useRelationalNulls)
        {
            _sqlExpressionFactory = (SingleStoreSqlExpressionFactory)Dependencies.SqlExpressionFactory;
            _options = options;
        }

        public override SelectExpression Optimize(SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = base.Optimize(selectExpression, parametersValues, out canCache);

            if (_options.ServerVersion.Supports.SingleStoreBugLimit0Offset0ExistsWorkaround)
            {
                selectExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Process(selectExpression, parametersValues, out var canCache2);

                canCache &= canCache2;
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                selectExpression = (SelectExpression)new SingleStoreBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(selectExpression);
            }

            selectExpression = (SelectExpression)new SingleStoreHavingExpressionVisitor(_sqlExpressionFactory).Visit(selectExpression);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            selectExpression = (SelectExpression)new SingleStoreCompatibilityExpressionVisitor(_options).Visit(selectExpression);

            return selectExpression;
        }

        /// <inheritdoc />
        protected override SelectExpression ProcessSqlNullability(
            SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));
            Check.NotNull(parametersValues, nameof(parametersValues));

            selectExpression = new SingleStoreSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(selectExpression, parametersValues, out canCache);

            return selectExpression;
        }
    }
}
