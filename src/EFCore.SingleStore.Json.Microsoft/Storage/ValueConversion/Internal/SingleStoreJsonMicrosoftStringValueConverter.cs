﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.ValueConversion.Internal
{
    public class SingleStoreJsonMicrosoftStringValueConverter : ValueConverter<string, string>
    {
        public SingleStoreJsonMicrosoftStringValueConverter()
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
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            JsonDocument.Parse(v)
                .WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
