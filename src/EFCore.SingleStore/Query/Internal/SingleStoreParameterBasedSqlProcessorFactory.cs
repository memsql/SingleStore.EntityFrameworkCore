// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreParameterBasedSqlProcessorFactory : IRelationalParameterBasedSqlProcessorFactory
    {
        private readonly RelationalParameterBasedSqlProcessorDependencies _dependencies;
        [NotNull] private readonly ISingleStoreOptions _options;

        public SingleStoreParameterBasedSqlProcessorFactory(
            [NotNull] RelationalParameterBasedSqlProcessorDependencies dependencies,
            [NotNull] ISingleStoreOptions options)
        {
            _dependencies = dependencies;
            _options = options;
        }

        public virtual RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
            => new SingleStoreParameterBasedSqlProcessor(_dependencies, useRelationalNulls, _options);
    }
}
