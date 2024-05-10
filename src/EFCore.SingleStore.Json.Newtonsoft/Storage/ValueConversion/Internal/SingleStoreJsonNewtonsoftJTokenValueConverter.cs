// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.ValueConversion.Internal
{
    public class SingleStoreJsonNewtonsoftJTokenValueConverter : ValueConverter<JToken, string>
    {
        public SingleStoreJsonNewtonsoftJTokenValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(JToken v)
            => v.ToString(Formatting.None);

        private static JToken ConvertFromProviderCore(string v)
            => JToken.Parse(v);
    }
}
