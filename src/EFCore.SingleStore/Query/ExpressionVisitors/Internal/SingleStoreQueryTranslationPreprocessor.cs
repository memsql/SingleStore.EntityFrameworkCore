// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;

public class SingleStoreQueryTranslationPreprocessor : RelationalQueryTranslationPreprocessor
{
    private readonly RelationalQueryCompilationContext _relationalQueryCompilationContext;

    public SingleStoreQueryTranslationPreprocessor(
        QueryTranslationPreprocessorDependencies dependencies,
        RelationalQueryTranslationPreprocessorDependencies relationalDependencies,
        QueryCompilationContext queryCompilationContext)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
        _relationalQueryCompilationContext = (RelationalQueryCompilationContext)queryCompilationContext;
    }

    /// <summary>
    /// Workaround https://github.com/dotnet/efcore/issues/30386.
    /// </summary>
    public override Expression NormalizeQueryableMethod(Expression expression)
    {
        // Implementation of base (RelationalQueryTranslationPreprocessor).
        expression = new RelationalQueryMetadataExtractingExpressionVisitor(_relationalQueryCompilationContext).Visit(expression);

        // Implementation of base.base (QueryTranslationPreprocessor), using `SingleStoreQueryableMethodNormalizingExpressionVisitor` instead of
        // `QueryableMethodNormalizingExpressionVisitor` directly.
        expression = new SingleStoreQueryableMethodNormalizingExpressionVisitor(QueryCompilationContext).Normalize(expression);
        expression = ProcessQueryRoots(expression);

        return expression;
    }
}
