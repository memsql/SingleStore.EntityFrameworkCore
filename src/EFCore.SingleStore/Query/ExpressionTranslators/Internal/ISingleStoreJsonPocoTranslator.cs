// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal
{
    public interface ISingleStoreJsonPocoTranslator : IMemberTranslator
    {
        SqlExpression TranslateMemberAccess([NotNull] SqlExpression instance, [NotNull] SqlExpression member, [NotNull] Type returnType);
        SqlExpression TranslateArrayLength([NotNull] SqlExpression expression);
        string GetJsonPropertyName([CanBeNull] MemberInfo member);
    }
}
