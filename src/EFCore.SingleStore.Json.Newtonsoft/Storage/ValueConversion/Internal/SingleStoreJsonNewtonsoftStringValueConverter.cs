using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class SingleStoreJsonNewtonsoftStringValueConverter : ValueConverter<string, string>
    {
        public SingleStoreJsonNewtonsoftStringValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(string v)
            => ProcessJsonString(v);

        private static string ConvertFromProviderCore(string v)
            => ProcessJsonString(v);

        internal static string ProcessJsonString(string v)
            => JToken.Parse(v).ToString(Formatting.None);
    }
}
