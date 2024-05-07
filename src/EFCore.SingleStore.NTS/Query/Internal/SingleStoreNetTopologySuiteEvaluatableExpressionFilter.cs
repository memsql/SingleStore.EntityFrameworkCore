using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.SingleStore.Query.Internal
{
    public class SingleStoreNetTopologySuiteEvaluatableExpressionFilter : ISingleStoreEvaluatableExpressionFilter
    {
        public virtual bool? IsEvaluatableExpression(Expression expression, IModel model)
        {
            if (expression is MethodCallExpression methodCallExpression &&
                methodCallExpression.Method.DeclaringType == typeof(SingleStoreSpatialDbFunctionsExtensions))
            {
                return false;
            }

            return null;
        }
    }
}
