// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreMethodCallTranslatorProvider : RelationalMethodCallTranslatorProvider
    {
        public SingleStoreMethodCallTranslatorProvider(
            [NotNull] RelationalMethodCallTranslatorProviderDependencies dependencies,
            [NotNull] ISingleStoreOptions options)
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
                new SingleStoreDbFunctionsExtensionsMethodTranslator(sqlExpressionFactory),
                new SingleStoreJsonDbFunctionsTranslator(sqlExpressionFactory),
                new SingleStoreMathMethodTranslator(sqlExpressionFactory),
                new SingleStoreNewGuidTranslator(sqlExpressionFactory),
                new SingleStoreObjectToStringTranslator(sqlExpressionFactory),
                new SingleStoreRegexIsMatchTranslator(sqlExpressionFactory),
                new SingleStoreStringComparisonMethodTranslator(sqlExpressionFactory, options),
                new SingleStoreStringMethodTranslator(sqlExpressionFactory, relationalTypeMappingSource, options),
            });
        }
    }
}
