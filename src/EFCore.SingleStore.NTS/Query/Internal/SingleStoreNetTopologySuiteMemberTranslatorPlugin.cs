// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreNetTopologySuiteMemberTranslatorPlugin : IMemberTranslatorPlugin
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SingleStoreNetTopologySuiteMemberTranslatorPlugin(
            IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory,
            ISingleStoreOptions options)
        {
            var mySqlSqlExpressionFactory = (SingleStoreSqlExpressionFactory)sqlExpressionFactory;

            Translators = new IMemberTranslator[]
            {
                new SingleStoreGeometryMemberTranslator(typeMappingSource, mySqlSqlExpressionFactory),
                new SingleStoreGeometryCollectionMemberTranslator(mySqlSqlExpressionFactory),
                new SingleStoreLineStringMemberTranslator(typeMappingSource, mySqlSqlExpressionFactory, options),
                new SingleStoreMultiLineStringMemberTranslator(mySqlSqlExpressionFactory),
                new SingleStorePointMemberTranslator(mySqlSqlExpressionFactory),
                new SingleStorePolygonMemberTranslator(typeMappingSource, mySqlSqlExpressionFactory)
            };
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerable<IMemberTranslator> Translators { get; }
    }
}
