// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreCompiledQueryCacheKeyGenerator : RelationalCompiledQueryCacheKeyGenerator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SingleStoreCompiledQueryCacheKeyGenerator(
            [NotNull] CompiledQueryCacheKeyGeneratorDependencies dependencies,
            [NotNull] RelationalCompiledQueryCacheKeyGeneratorDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override object GenerateCacheKey(Expression query, bool async)
        {
            var extensions = RelationalDependencies.ContextOptions.FindExtension<SingleStoreOptionsExtension>();
            return new SingleStoreCompiledQueryCacheKey(
                GenerateCacheKeyCore(query, async),
                extensions?.ServerVersion,
                extensions?.NoBackslashEscapes ?? false);
        }

        private readonly struct SingleStoreCompiledQueryCacheKey
        {
            private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;
            private readonly ServerVersion _serverVersion;
            private readonly bool _noBackslashEscapes;

            public SingleStoreCompiledQueryCacheKey(
                RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey,
                ServerVersion serverVersion,
                bool noBackslashEscapes)
            {
                _relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
                _serverVersion = serverVersion;
                _noBackslashEscapes = noBackslashEscapes;
            }

            public override bool Equals(object obj)
                => !(obj is null)
                   && obj is SingleStoreCompiledQueryCacheKey key
                   && Equals(key);

            private bool Equals(SingleStoreCompiledQueryCacheKey other)
                => _relationalCompiledQueryCacheKey.Equals(other._relationalCompiledQueryCacheKey)
                   && Equals(_serverVersion, other._serverVersion)
                   && _noBackslashEscapes == other._noBackslashEscapes
                ;

            public override int GetHashCode()
                => HashCode.Combine(_relationalCompiledQueryCacheKey,
                    _serverVersion,
                    _noBackslashEscapes);
        }
    }
}
