// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    public class SingleStoreQueryTranslationPostprocessorFactory : IQueryTranslationPostprocessorFactory
    {
        private readonly QueryTranslationPostprocessorDependencies _dependencies;
        private readonly RelationalQueryTranslationPostprocessorDependencies _relationalDependencies;
        private readonly ISingleStoreOptions _options;
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;

        public SingleStoreQueryTranslationPostprocessorFactory(
            QueryTranslationPostprocessorDependencies dependencies,
            RelationalQueryTranslationPostprocessorDependencies relationalDependencies,
            ISingleStoreOptions options,
            ISqlExpressionFactory sqlExpressionFactory)
        {
            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
            _options = options;
            _sqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;
        }

        public virtual QueryTranslationPostprocessor Create(QueryCompilationContext queryCompilationContext)
            => new SingleStoreQueryTranslationPostprocessor(
                _dependencies,
                _relationalDependencies,
                queryCompilationContext,
                _options,
                _sqlExpressionFactory);
    }
}
