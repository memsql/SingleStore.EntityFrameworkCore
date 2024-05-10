using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Query.Internal
{
    public class SingleStoreJsonNewtonsoftPocoTranslator : SingleStoreJsonPocoTranslator
    {
        public SingleStoreJsonNewtonsoftPocoTranslator(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] ISqlExpressionFactory sqlExpressionFactory)
        : base(typeMappingSource, (SingleStoreSqlExpressionFactory)sqlExpressionFactory)
        {
        }

        public override string GetJsonPropertyName(MemberInfo member)
            => member.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
    }
}
