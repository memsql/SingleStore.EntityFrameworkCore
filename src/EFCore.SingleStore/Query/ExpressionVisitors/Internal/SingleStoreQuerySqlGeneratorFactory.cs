// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    public class SingleStoreQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly ISingleStoreOptions _options;

        public SingleStoreQuerySqlGeneratorFactory(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            ISingleStoreOptions options)
        {
            _dependencies = dependencies;
            _options = options;
        }

        public virtual QuerySqlGenerator Create()
            => new SingleStoreQuerySqlGenerator(_dependencies, _options);
    }
}
