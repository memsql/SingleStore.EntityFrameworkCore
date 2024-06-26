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
    public class SingleStoreJsonMicrosoftJsonDocumentValueConverter : ValueConverter<JsonDocument, string>
    {
        public SingleStoreJsonMicrosoftJsonDocumentValueConverter()
            : base(
                v => ConvertToProviderCore(v),
                v => ConvertFromProviderCore(v))
        {
        }

        private static string ConvertToProviderCore(JsonDocument v)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            v.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static JsonDocument ConvertFromProviderCore(string v)
            => JsonDocument.Parse(v);
    }
}
