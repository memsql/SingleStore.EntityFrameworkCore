// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Utilities;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreQueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        private readonly QueryCompilationContextDependencies _dependencies;
        private readonly RelationalQueryCompilationContextDependencies _relationalDependencies;

        public SingleStoreQueryCompilationContextFactory(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(relationalDependencies, nameof(relationalDependencies));

            _dependencies = dependencies;
            _relationalDependencies = relationalDependencies;
        }

        public virtual QueryCompilationContext Create(bool async)
            => new SingleStoreQueryCompilationContext(_dependencies, _relationalDependencies, async);

        public virtual QueryCompilationContext CreatePrecompiled(bool async, IReadOnlySet<string> nonNullableReferenceTypeParameters)
            => new SingleStoreQueryCompilationContext(
                _dependencies, _relationalDependencies, async, precompiling: true, nonNullableReferenceTypeParameters);
    }
}
