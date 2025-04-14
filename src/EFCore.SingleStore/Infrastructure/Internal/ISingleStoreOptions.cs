// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.SingleStore.Infrastructure.Internal
{
    public interface ISingleStoreOptions : ISingletonOptions
    {
        SingleStoreConnectionSettings ConnectionSettings { get; }

        /// <remarks>
        /// If null, there might still be a `DbDataSource` in the ApplicationServiceProvider.
        /// </remarks>>
        DbDataSource DataSource { get; }

        ServerVersion ServerVersion { get; }
        CharSet DefaultCharSet { get; }
        CharSet NationalCharSet { get; }
        string DefaultGuidCollation { get; }
        bool NoBackslashEscapes { get; }
        bool ReplaceLineBreaksWithCharFunction { get; }
        SingleStoreDefaultDataTypeMappings DefaultDataTypeMappings { get; }
        SingleStoreSchemaNameTranslator SchemaNameTranslator { get; }
        bool IndexOptimizedBooleanColumns { get; }
        SingleStoreJsonChangeTrackingOptions JsonChangeTrackingOptions { get; }
        bool LimitKeyedOrIndexedStringColumnLength { get; }
        bool StringComparisonTranslations { get; }
        bool PrimitiveCollectionsSupport { get; }
    }
}
