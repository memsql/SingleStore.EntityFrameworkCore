// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
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

        /// <summary>
        /// Gets the timeout used by the commands that acquire the migrations lock.
        /// </summary>
        /// <remarks>
        /// This timeout applies while creating the lock table and while waiting to acquire the lock row.
        /// It does not limit how long the lock is held after acquisition; the lock is held until the
        /// dedicated lock transaction/connection is disposed.
        /// </remarks>
        public TimeSpan MigrationLockTimeout { get; }
    }
}
