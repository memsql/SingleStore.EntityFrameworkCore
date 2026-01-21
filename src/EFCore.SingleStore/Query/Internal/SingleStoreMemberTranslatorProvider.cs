// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreMemberTranslatorProvider : RelationalMemberTranslatorProvider
    {
        public SingleStoreMemberTranslatorProvider([NotNull] RelationalMemberTranslatorProviderDependencies dependencies, IDbContextOptions dbContextOptions)
            : base(dependencies)
        {
            var sqlExpressionFactory = (SingleStoreSqlExpressionFactory)dependencies.SqlExpressionFactory;

            AddTranslators(
                new IMemberTranslator[] {
                    new SingleStoreDateTimeMemberTranslator(sqlExpressionFactory, dbContextOptions),
                    new SingleStoreStringMemberTranslator(sqlExpressionFactory),
                    new SingleStoreTimeSpanMemberTranslator(sqlExpressionFactory),
                });
        }
    }
}
