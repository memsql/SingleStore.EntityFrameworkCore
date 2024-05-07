// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    public class SingleStoreSqlTranslatingExpressionVisitorFactory : IRelationalSqlTranslatingExpressionVisitorFactory
    {
        private readonly RelationalSqlTranslatingExpressionVisitorDependencies _dependencies;
        private readonly ISingleStoreJsonPocoTranslator _jsonPocoTranslator;

        public SingleStoreSqlTranslatingExpressionVisitorFactory(
            [NotNull] RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] IServiceProvider serviceProvider)
        {
            _dependencies = dependencies;
            _jsonPocoTranslator = serviceProvider.GetService<ISingleStoreJsonPocoTranslator>();
        }

        public virtual RelationalSqlTranslatingExpressionVisitor Create(
            QueryCompilationContext queryCompilationContext,
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
            => new SingleStoreSqlTranslatingExpressionVisitor(
                _dependencies,
                queryCompilationContext,
                queryableMethodTranslatingExpressionVisitor,
                _jsonPocoTranslator);
    }
}
