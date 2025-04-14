// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Query.Expressions.Internal;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;

namespace EntityFrameworkCore.SingleStore.Query.ExpressionVisitors.Internal;

/// <summary>
/// Inject parameter inlining expressions where parameters are not supported for some reason.
/// </summary>
public class SingleStoreParameterInliningExpressionVisitor : ExpressionVisitor
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly ISqlExpressionFactory _sqlExpressionFactory;
    private readonly ISingleStoreOptions _options;

    private IReadOnlyDictionary<string, object> _parametersValues;
    private bool _canCache;

    private bool _inJsonTableSourceParameterCall;

    public SingleStoreParameterInliningExpressionVisitor(
        IRelationalTypeMappingSource typeMappingSource,
        ISqlExpressionFactory sqlExpressionFactory,
        ISingleStoreOptions options)
    {
        _typeMappingSource = typeMappingSource;
        _sqlExpressionFactory = sqlExpressionFactory;
        _options = options;
    }

    public virtual Expression Process(Expression expression, IReadOnlyDictionary<string, object> parametersValues, out bool canCache)
    {
        Check.NotNull(expression, nameof(expression));

        _parametersValues = parametersValues;
        _canCache = true;

        var result = Visit(expression);

        canCache = _canCache;

        return result;
    }

    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            SingleStoreJsonTableExpression jsonTableExpression => VisitJsonTable(jsonTableExpression),
            SqlParameterExpression sqlParameterExpression => VisitSqlParameter(sqlParameterExpression),
            ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(
                Visit(shapedQueryExpression.QueryExpression),
                Visit(shapedQueryExpression.ShaperExpression)),
            _ => base.VisitExtension(extensionExpression)
        };

    protected virtual Expression VisitJsonTable(SingleStoreJsonTableExpression jsonTableExpression)
    {
        var parentInJsonTableSourceParameterCall = _inJsonTableSourceParameterCall;
        _inJsonTableSourceParameterCall = true;
        var jsonExpression = (SqlExpression)Visit(jsonTableExpression.JsonExpression);
        _inJsonTableSourceParameterCall = parentInJsonTableSourceParameterCall;

        return jsonTableExpression.Update(
            jsonExpression,
            jsonTableExpression.Path,
            jsonTableExpression.ColumnInfos);
    }

    protected virtual Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
    {
        // For test simplicity, we currently inline parameters even for non MySQL database engines (even though it should not be necessary
        // for e.g. MariaDB).
        // TODO: Use inlined parameters only if JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash is true.
        if (_inJsonTableSourceParameterCall /*&&
            !_options.ServerVersion.Supports.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash*/)
        {
            _canCache = false;

            return new SingleStoreInlinedParameterExpression(
                sqlParameterExpression,
                _sqlExpressionFactory.Constant(
                    _parametersValues[sqlParameterExpression.Name],
                    sqlParameterExpression.TypeMapping));
        }

        return sqlParameterExpression;
    }
}
