// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Expressions.Internal
{
    public class SingleStoreMatchExpression : SqlExpression
    {
        public SingleStoreMatchExpression(
            SqlExpression match,
            SqlExpression against,
            RelationalTypeMapping typeMapping)
            : base(typeof(bool), typeMapping)
        {
            Check.NotNull(match, nameof(match));
            Check.NotNull(against, nameof(against));

            Match = match;
            Against = against;
        }


        public virtual SqlExpression Match { get; }
        public virtual SqlExpression Against { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is SingleStoreQuerySqlGenerator mySqlQuerySqlGenerator // TODO: Move to VisitExtensions
                ? mySqlQuerySqlGenerator.VisitSingleStoreMatch(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var match = (SqlExpression)visitor.Visit(Match);
            var against = (SqlExpression)visitor.Visit(Against);

            return Update(match, against);
        }

        public virtual SingleStoreMatchExpression Update(SqlExpression match, SqlExpression against)
            => match != Match || against != Against
                ? new SingleStoreMatchExpression(
                    match,
                    against,
                    TypeMapping)
                : this;

        public override bool Equals(object obj)
            => obj != null && ReferenceEquals(this, obj)
            || obj is SingleStoreMatchExpression matchExpression && Equals(matchExpression);

        private bool Equals(SingleStoreMatchExpression matchExpression)
            => base.Equals(matchExpression)
               && Match.Equals(matchExpression.Match)
            && Against.Equals(matchExpression.Against);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Match, Against);

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("MATCH ");
            expressionPrinter.Append($"({expressionPrinter.Visit(Match)})");
            expressionPrinter.Append(" AGAINST ");
            expressionPrinter.Append($"({expressionPrinter.Visit(Against)}");
            expressionPrinter.Append(")");
        }
    }
}
