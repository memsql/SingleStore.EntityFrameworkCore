// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;

public class SingleStoreQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    private readonly QueryTranslationPreprocessorDependencies _dependencies;
    private readonly RelationalQueryTranslationPreprocessorDependencies _relationalDependencies;

    public SingleStoreQueryTranslationPreprocessorFactory(
        QueryTranslationPreprocessorDependencies dependencies,
        RelationalQueryTranslationPreprocessorDependencies relationalDependencies)
    {
        _dependencies = dependencies;
        _relationalDependencies = relationalDependencies;
    }

    public virtual QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new SingleStoreQueryTranslationPreprocessor(
            _dependencies,
            _relationalDependencies,
            queryCompilationContext);
}
