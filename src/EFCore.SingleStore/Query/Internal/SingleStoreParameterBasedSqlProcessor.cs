// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

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

        public SingleStoreParameterBasedSqlProcessor(
            RelationalParameterBasedSqlProcessorDependencies dependencies,
            RelationalParameterBasedSqlProcessorParameters parameters,
            ISingleStoreOptions options)
            : base(dependencies, parameters)
        {
            _options = options;
        }

        public override Expression Process(Expression queryExpression, ParametersCacheDecorator parametersDecorator)
        {
            queryExpression = base.Process(queryExpression, parametersDecorator);

            if (_options.ServerVersion.Supports.SingleStoreBugLimit0Offset0ExistsWorkaround)
            {
                queryExpression = new SkipTakeCollapsingExpressionVisitor(Dependencies.SqlExpressionFactory)
                    .Process(queryExpression, parametersDecorator);
            }

            if (_options.IndexOptimizedBooleanColumns)
            {
                queryExpression = new SingleStoreBoolOptimizingExpressionVisitor(Dependencies.SqlExpressionFactory).Visit(queryExpression);
            }

            queryExpression = new SingleStoreParameterInliningExpressionVisitor(
                Dependencies.TypeMappingSource,
                Dependencies.SqlExpressionFactory,
                _options).Process(queryExpression, parametersDecorator);

            // Run the compatibility checks as late in the query pipeline (before the actual SQL translation happens) as reasonable.
            queryExpression = new SingleStoreCompatibilityExpressionVisitor(_options).Visit(queryExpression);

            return queryExpression;
        }

        /// <inheritdoc />
        protected override Expression ProcessSqlNullability(
            Expression queryExpression,
            ParametersCacheDecorator parametersDecorator)
        {
            Check.NotNull(queryExpression, nameof(queryExpression));
            Check.NotNull(parametersDecorator, nameof(parametersDecorator));

            queryExpression = new SingleStoreSqlNullabilityProcessor(Dependencies, Parameters)
                .Process(queryExpression, parametersDecorator);

            return queryExpression;
        }
    }
}
