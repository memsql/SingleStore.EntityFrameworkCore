// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;

namespace EntityFrameworkCore.SingleStore.Query.Expressions.Internal
{
    public enum SingleStoreBinaryExpressionOperatorType
    {
        /// <summary>
        /// TODO
        /// </summary>
        IntegerDivision,

        /// <summary>
        /// Use to force an equals expression, that will not be optimized by EF Core.
        /// Can be used, to force a `value = TRUE` expression.
        /// </summary>
        NonOptimizedEqual,
    }

    public class SingleStoreBinaryExpression : SqlExpression
    {
        private static ConstructorInfo _quotingConstructor;

        public SingleStoreBinaryExpression(
            SingleStoreBinaryExpressionOperatorType operatorType,
            SqlExpression left,
            SqlExpression right,
            Type type,
            RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            OperatorType = operatorType;

            Left = left;
            Right = right;
        }

        public virtual SingleStoreBinaryExpressionOperatorType OperatorType { get; }
        public virtual SqlExpression Left { get; }
        public virtual SqlExpression Right { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is SingleStoreQuerySqlGenerator mySqlQuerySqlGenerator // TODO: Move to VisitExtensions
                ? mySqlQuerySqlGenerator.VisitSingleStoreBinaryExpression(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var left = (SqlExpression)visitor.Visit(Left);
            var right = (SqlExpression)visitor.Visit(Right);

            return Update(left, right);
        }

        public override Expression Quote()
            => New(
                _quotingConstructor ??= typeof(SingleStoreBinaryExpression).GetConstructor(
                    [typeof(SingleStoreBinaryExpressionOperatorType), typeof(SqlExpression), typeof(SqlExpression), typeof(Type), typeof(RelationalTypeMapping)])!,
                Constant(OperatorType),
                Left.Quote(),
                Right.Quote(),
                Constant(Type),
                RelationalExpressionQuotingUtilities.QuoteTypeMapping(TypeMapping));

        public virtual SingleStoreBinaryExpression Update(SqlExpression left, SqlExpression right)
            => left != Left || right != Right
                ? new SingleStoreBinaryExpression(OperatorType, left, right, Type, TypeMapping)
                : this;

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            var requiresBrackets = RequiresBrackets(Left);

            if (requiresBrackets)
            {
                expressionPrinter.Append("(");
            }

            expressionPrinter.Visit(Left);

            if (requiresBrackets)
            {
                expressionPrinter.Append(")");
            }

            switch (OperatorType)
            {
                case SingleStoreBinaryExpressionOperatorType.IntegerDivision:
                    expressionPrinter.Append(" DIV ");
                    break;
                case SingleStoreBinaryExpressionOperatorType.NonOptimizedEqual:
                    expressionPrinter.Append(" = ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            requiresBrackets = RequiresBrackets(Right);

            if (requiresBrackets)
            {
                expressionPrinter.Append("(");
            }

            expressionPrinter.Visit(Right);

            if (requiresBrackets)
            {
                expressionPrinter.Append(")");
            }
        }

        private bool RequiresBrackets(SqlExpression expression)
        {
            return expression is SqlBinaryExpression sqlBinary
                && sqlBinary.OperatorType != ExpressionType.Coalesce
                || expression is LikeExpression;
        }

        public override bool Equals(object obj)
            => obj != null
            && (ReferenceEquals(this, obj)
                || obj is SingleStoreBinaryExpression sqlBinaryExpression
                    && Equals(sqlBinaryExpression));

        private bool Equals(SingleStoreBinaryExpression sqlBinaryExpression)
            => base.Equals(sqlBinaryExpression)
            && OperatorType == sqlBinaryExpression.OperatorType
            && Left.Equals(sqlBinaryExpression.Left)
            && Right.Equals(sqlBinaryExpression.Right);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), OperatorType, Left, Right);
    }
}
