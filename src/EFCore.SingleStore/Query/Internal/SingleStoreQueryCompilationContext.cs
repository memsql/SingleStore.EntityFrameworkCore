// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreQueryCompilationContext : RelationalQueryCompilationContext
    {
        public SingleStoreQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies, bool async)
            : base(dependencies, relationalDependencies, async)
        {
        }

        public override bool IsBuffering
            => base.IsBuffering ||
               QuerySplittingBehavior == Microsoft.EntityFrameworkCore.QuerySplittingBehavior.SplitQuery;
    }
}
