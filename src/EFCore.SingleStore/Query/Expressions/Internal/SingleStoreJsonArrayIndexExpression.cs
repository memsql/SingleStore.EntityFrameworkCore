// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Query.Expressions.Internal
{
    /// <summary>
    /// Represents a MySQL JSON array index (i.e. x[y]).
    /// </summary>
    public class SingleStoreJsonArrayIndexExpression : SqlExpression, IEquatable<SingleStoreJsonArrayIndexExpression>
    {
        private static ConstructorInfo _quotingConstructor;

        [NotNull]
        public virtual SqlExpression Expression { get; }

        public SingleStoreJsonArrayIndexExpression(
            [NotNull] SqlExpression expression,
            [NotNull] Type type,
            [CanBeNull] RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Expression = expression;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update((SqlExpression)visitor.Visit(Expression));

        /// <inheritdoc />
        public override Expression Quote()
            => New(
                _quotingConstructor ??= typeof(SingleStoreInlinedParameterExpression).GetConstructor(
                    [typeof(SqlExpression), typeof(Type), typeof(RelationalTypeMapping)])!,
                Expression.Quote(),
                Constant(Type),
                RelationalExpressionQuotingUtilities.QuoteTypeMapping(TypeMapping));

        public virtual SingleStoreJsonArrayIndexExpression Update(
            [NotNull] SqlExpression expression)
            => expression == Expression
                ? this
                : new SingleStoreJsonArrayIndexExpression(expression, Type, TypeMapping);

        public override bool Equals(object obj)
            => Equals(obj as SingleStoreJsonArrayIndexExpression);

        public virtual bool Equals(SingleStoreJsonArrayIndexExpression other)
            => ReferenceEquals(this, other) ||
               other != null &&
               base.Equals(other) &&
               Equals(Expression, other.Expression);

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Expression);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("[");
            expressionPrinter.Visit(Expression);
            expressionPrinter.Append("]");
        }

        public override string ToString()
            => $"[{Expression}]";

        public virtual SqlExpression ApplyTypeMapping(RelationalTypeMapping typeMapping)
            => new SingleStoreJsonArrayIndexExpression(Expression, Type, typeMapping);
    }
}
