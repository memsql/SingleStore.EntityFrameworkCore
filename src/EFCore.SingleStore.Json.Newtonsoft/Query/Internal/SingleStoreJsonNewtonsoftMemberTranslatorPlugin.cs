// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Query.Internal
{
    public class SingleStoreJsonNewtonsoftMemberTranslatorPlugin : IMemberTranslatorPlugin
    {
        public SingleStoreJsonNewtonsoftMemberTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            ISingleStoreJsonPocoTranslator jsonPocoTranslator)
        {
            var mySqlSqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;
            var mySqlJsonPocoTranslator = (SingleStoreJsonPocoTranslator)jsonPocoTranslator;

            Translators = new IMemberTranslator[]
            {
                new SingleStoreJsonNewtonsoftDomTranslator(
                    mySqlSqlExpressionFactory,
                    typeMappingSource,
                    mySqlJsonPocoTranslator),
                jsonPocoTranslator,
            };
        }

        public virtual IEnumerable<IMemberTranslator> Translators { get; }
    }
}
