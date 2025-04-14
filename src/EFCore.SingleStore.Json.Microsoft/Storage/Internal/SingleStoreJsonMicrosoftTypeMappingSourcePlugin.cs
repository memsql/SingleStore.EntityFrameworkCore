// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.ValueComparison.Internal;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.ValueConversion.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.Internal
{
    public class SingleStoreJsonMicrosoftTypeMappingSourcePlugin : SingleStoreJsonTypeMappingSourcePlugin
    {
        private static readonly Lazy<SingleStoreJsonMicrosoftJsonDocumentValueConverter> _jsonDocumentValueConverter = new Lazy<SingleStoreJsonMicrosoftJsonDocumentValueConverter>();
        private static readonly Lazy<SingleStoreJsonMicrosoftJsonElementValueConverter> _jsonElementValueConverter = new Lazy<SingleStoreJsonMicrosoftJsonElementValueConverter>();
        private static readonly Lazy<SingleStoreJsonMicrosoftStringValueConverter> _jsonStringValueConverter = new Lazy<SingleStoreJsonMicrosoftStringValueConverter>();

        public SingleStoreJsonMicrosoftTypeMappingSourcePlugin(
            [NotNull] ISingleStoreOptions options)
            : base(options)
        {
        }

        protected override Type SingleStoreJsonTypeMappingType => typeof(SingleStoreJsonMicrosoftTypeMapping<>);

        protected override RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;

            if (clrType == typeof(JsonDocument) ||
                clrType == typeof(JsonElement))
            {
                return (RelationalTypeMapping)Activator.CreateInstance(
                    SingleStoreJsonTypeMappingType.MakeGenericType(clrType),
                    "json",
                    GetValueConverter(clrType),
                    GetValueComparer(clrType),
                    Options.NoBackslashEscapes,
                    Options.ReplaceLineBreaksWithCharFunction);
            }

            return null;
        }

        protected override ValueConverter GetValueConverter(Type clrType)
        {
            if (clrType == typeof(JsonDocument))
            {
                return _jsonDocumentValueConverter.Value;
            }

            if (clrType == typeof(JsonElement))
            {
                return _jsonElementValueConverter.Value;
            }

            if (clrType == typeof(string))
            {
                return _jsonStringValueConverter.Value;
            }

            return (ValueConverter)Activator.CreateInstance(
                typeof(SingleStoreJsonMicrosoftPocoValueConverter<>).MakeGenericType(clrType));
        }

        protected override ValueComparer GetValueComparer(Type clrType)
            => SingleStoreJsonMicrosoftValueComparer.Create(clrType, Options.JsonChangeTrackingOptions);
    }
}
