// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Query.Internal
{
    public class SingleStoreJsonMicrosoftMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        public SingleStoreJsonMicrosoftMethodCallTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            ISingleStoreJsonPocoTranslator jsonPocoTranslator)
        {
            var mySqlSqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;
            var mySqlJsonPocoTranslator = (SingleStoreJsonPocoTranslator)jsonPocoTranslator;

            Translators = new IMethodCallTranslator[]
            {
                new SingleStoreJsonMicrosoftDomTranslator(
                    mySqlSqlExpressionFactory,
                    typeMappingSource,
                    mySqlJsonPocoTranslator),
            };
        }

        public virtual IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
