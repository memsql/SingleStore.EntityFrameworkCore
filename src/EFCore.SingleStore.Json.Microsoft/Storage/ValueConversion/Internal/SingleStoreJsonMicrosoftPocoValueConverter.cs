// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.ValueConversion.Internal
{
    public class SingleStoreJsonMicrosoftPocoValueConverter<T> : ValueConverter<T, string>
    {
        public SingleStoreJsonMicrosoftPocoValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        public static string ConvertToProviderCore(T v)
            => JsonSerializer.Serialize(v);

        public static T ConvertFromProviderCore(string v)
            => JsonSerializer.Deserialize<T>(v);
    }
}
