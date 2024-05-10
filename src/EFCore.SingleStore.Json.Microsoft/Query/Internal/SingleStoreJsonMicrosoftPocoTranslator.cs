using System.Reflection;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.SingleStore.Query.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Query.Internal
{
    public class SingleStoreJsonMicrosoftPocoTranslator : SingleStoreJsonPocoTranslator
    {
        public SingleStoreJsonMicrosoftPocoTranslator(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] ISqlExpressionFactory sqlExpressionFactory)
        : base(typeMappingSource, (SingleStoreSqlExpressionFactory)sqlExpressionFactory)
        {
        }

        public override string GetJsonPropertyName(MemberInfo member)
            => member.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
    }
}
