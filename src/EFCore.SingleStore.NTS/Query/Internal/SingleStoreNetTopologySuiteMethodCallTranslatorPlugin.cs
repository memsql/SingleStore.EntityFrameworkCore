// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreNetTopologySuiteMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SingleStoreNetTopologySuiteMethodCallTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            ISingleStoreOptions options)
        {
            var mySqlSqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;

            Translators = new IMethodCallTranslator[]
            {
                new SingleStoreGeometryMethodTranslator(typeMappingSource, mySqlSqlExpressionFactory, options),
                new SingleStoreGeometryCollectionMethodTranslator(typeMappingSource, mySqlSqlExpressionFactory),
                new SingleStoreLineStringMethodTranslator(typeMappingSource, mySqlSqlExpressionFactory),
                new SingleStorePolygonMethodTranslator(typeMappingSource, mySqlSqlExpressionFactory),
                new SingleStoreSpatialDbFunctionsExtensionsMethodTranslator(typeMappingSource, mySqlSqlExpressionFactory, options)
            };
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
