// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal
{
    public class SingleStoreJsonParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly SingleStoreSqlExpressionFactory _sqlExpressionFactory;
        private readonly ISingleStoreOptions _options;

        public SingleStoreJsonParameterExpressionVisitor(SingleStoreSqlExpressionFactory sqlExpressionFactory, ISingleStoreOptions options)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
            _options = options;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
            => extensionExpression switch
            {
                SqlParameterExpression sqlParameterExpression => VisitParameter(sqlParameterExpression),
                ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(Visit(shapedQueryExpression.QueryExpression), Visit(shapedQueryExpression.ShaperExpression)),
                _ => base.VisitExtension(extensionExpression)
            };

        protected virtual SqlExpression VisitParameter(SqlParameterExpression sqlParameterExpression)
        {
            if (sqlParameterExpression.TypeMapping is SingleStoreJsonTypeMapping)
            {
                var typeMapping = _sqlExpressionFactory.FindMapping(sqlParameterExpression.Type, "json");

                // MySQL has a real JSON datatype, and string parameters need to be converted to it.
                // MariaDB defines the JSON datatype just as a synonym for LONGTEXT.
                if (!_options.ServerVersion.Supports.JsonDataTypeEmulation)
                {
                    return _sqlExpressionFactory.Convert(
                        sqlParameterExpression,
                        typeMapping.ClrType, // will be typeof(string) when `sqlParameterExpression.Type`
                        typeMapping);        // is typeof(SingleStoreJsonString)
                }
            }

            return sqlParameterExpression;
        }
    }
}
