// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public SingleStoreMethodCallTranslatorProvider(
            [NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies,
            [NotNull] ISingleStoreOptions options,
            [NotNull] IDbContextOptions dbContextOptions)
            : base(dependencies)
        {
            var sqlExpressionFactory = (SingleStoreSqlExpressionFactory)dependencies.SqlExpressionFactory;
            var relationalTypeMappingSource = (SingleStoreTypeMappingSource)dependencies.RelationalTypeMappingSource;

            AddTranslators(new IMethodCallTranslator[]
            {
                new SingleStoreByteArrayMethodTranslator(sqlExpressionFactory),
                new SingleStoreConvertTranslator(sqlExpressionFactory),
                new SingleStoreDateTimeMethodTranslator(sqlExpressionFactory),
                new SingleStoreDateDiffFunctionsTranslator(sqlExpressionFactory),
                new SingleStoreDbFunctionsExtensionsMethodTranslator(sqlExpressionFactory, dbContextOptions),
                new SingleStoreJsonDbFunctionsTranslator(sqlExpressionFactory),
                new SingleStoreMathMethodTranslator(sqlExpressionFactory),
                new SingleStoreNewGuidTranslator(sqlExpressionFactory),
                new SingleStoreObjectToStringTranslator(sqlExpressionFactory),
                new SingleStoreRegexIsMatchTranslator(sqlExpressionFactory),
                new SingleStoreStringComparisonMethodTranslator(sqlExpressionFactory, () => QueryCompilationContext, options),
                new SingleStoreStringMethodTranslator(sqlExpressionFactory, relationalTypeMappingSource, () => QueryCompilationContext, options),
            });
        }


        public virtual QueryCompilationContext QueryCompilationContext { get; set; }

        public override SqlExpression Translate(
            IModel model,
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
            => QueryCompilationContext is not null
                ? base.Translate(model, instance, method, arguments, logger)
                : throw new InvalidOperationException();
    }
}
