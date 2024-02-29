// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public abstract class SingleStoreJsonTypeMappingSourcePlugin
        : IRelationalTypeMappingSourcePlugin
    {
        [NotNull]
        public virtual ISingleStoreOptions Options { get; }

        protected SingleStoreJsonTypeMappingSourcePlugin(
            [NotNull] ISingleStoreOptions options)
        {
            Options = options;
        }

        public virtual RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;

            if (clrType == typeof(SingleStoreJsonString))
            {
                clrType = typeof(string);
                storeTypeName = "json";
            }

            if (storeTypeName != null)
            {
                clrType ??= typeof(string);
                return storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase)
                    ? (RelationalTypeMapping)Activator.CreateInstance(
                        SingleStoreJsonTypeMappingType.MakeGenericType(clrType),
                        storeTypeName,
                        GetValueConverter(clrType),
                        GetValueComparer(clrType),
                        Options)
                    : null;
            }

            return FindDomMapping(mappingInfo);
        }

        protected abstract Type SingleStoreJsonTypeMappingType { get; }
        protected abstract RelationalTypeMapping FindDomMapping(RelationalTypeMappingInfo mappingInfo);
        protected abstract ValueConverter GetValueConverter(Type clrType);
        protected abstract ValueComparer GetValueComparer(Type clrType);
    }
}
