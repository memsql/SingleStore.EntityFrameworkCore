// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.ValueComparison.Internal;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.ValueConversion.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.Internal
{
    public class SingleStoreJsonNewtonsoftTypeMappingSourcePlugin : SingleStoreJsonTypeMappingSourcePlugin
    {
        private static readonly Lazy<SingleStoreJsonNewtonsoftJTokenValueConverter> _jTokenValueConverter = new Lazy<SingleStoreJsonNewtonsoftJTokenValueConverter>();
        private static readonly Lazy<SingleStoreJsonNewtonsoftStringValueConverter> _jsonStringValueConverter = new Lazy<SingleStoreJsonNewtonsoftStringValueConverter>();

        public SingleStoreJsonNewtonsoftTypeMappingSourcePlugin(
            [NotNull] ISingleStoreOptions options)
            : base(options)
        {
        }

        protected override Type SingleStoreJsonTypeMappingType => typeof(SingleStoreJsonNewtonsoftTypeMapping<>);

        protected override RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;

            if (typeof(JToken).IsAssignableFrom(clrType))
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
            if (typeof(JToken).IsAssignableFrom(clrType))
            {
                return _jTokenValueConverter.Value;
            }

            if (clrType == typeof(string))
            {
                return _jsonStringValueConverter.Value;
            }

            return (ValueConverter)Activator.CreateInstance(
                typeof(SingleStoreJsonNewtonsoftPocoValueConverter<>).MakeGenericType(clrType));
        }

        protected override ValueComparer GetValueComparer(Type clrType)
            => SingleStoreJsonNewtonsoftValueComparer.Create(clrType, Options.JsonChangeTrackingOptions);
    }
}
