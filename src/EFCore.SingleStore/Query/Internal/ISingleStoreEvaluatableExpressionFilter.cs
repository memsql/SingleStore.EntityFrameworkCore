// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public interface ISingleStoreEvaluatableExpressionFilter
    {
        /// <summary>
        ///     Checks whether the given expression can be evaluated.
        /// </summary>
        /// <param name="expression"> The expression. </param>
        /// <param name="model"> The model. </param>
        /// <returns>
        /// <see langword="true" /> if the expression can be evaluated, <see langword="false" /> if it can't and <see langword="null" /> if
        /// it doesn't handle the given expression.
        /// </returns>
        bool? IsEvaluatableExpression([NotNull] Expression expression, [NotNull] IModel model);
    }
}
