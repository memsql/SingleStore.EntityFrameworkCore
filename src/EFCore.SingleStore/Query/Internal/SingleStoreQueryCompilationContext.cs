// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreQueryCompilationContext : RelationalQueryCompilationContext
    {
        public SingleStoreQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies,
            bool async)
            : base(dependencies, relationalDependencies, async)
        {
        }

        public SingleStoreQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies,
            bool async,
            bool precompiling,
            IReadOnlySet<string> nonNullableReferenceTypeParameters)
            : base(dependencies, relationalDependencies, async, precompiling, nonNullableReferenceTypeParameters)
        {
        }

        public override bool IsBuffering
            => base.IsBuffering ||
               QuerySplittingBehavior == Microsoft.EntityFrameworkCore.QuerySplittingBehavior.SplitQuery;

        /// <inheritdoc />
        public override bool SupportsPrecompiledQuery => false;
    }
}
