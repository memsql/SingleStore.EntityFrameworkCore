// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    public class SingleStoreQueryTranslationPostprocessor : RelationalQueryTranslationPostprocessor
    {
        private readonly ISingleStoreOptions _options;
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreQueryTranslationPostprocessor(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            QueryCompilationContext queryCompilationContext,
            ISingleStoreOptions options,
            SingleStoreSqlExpressionFactory sqlExpressionFactory)
            : base(dependencies, relationalDependencies, queryCompilationContext)
        {
            _options = options;
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public override Expression Process(Expression query)
        {
            query = base.Process(query);

            query = new SingleStoreJsonParameterExpressionVisitor(_sqlExpressionFactory, _options).Visit(query);

            if (_options.ServerVersion.Supports.SingleStoreBug96947Workaround)
            {
                query = new SingleStoreBug96947WorkaroundExpressionVisitor(_sqlExpressionFactory).Visit(query);
            }

            return query;
        }
    }
}
