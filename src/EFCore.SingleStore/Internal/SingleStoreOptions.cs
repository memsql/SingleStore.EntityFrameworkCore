// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Linq;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.SingleStore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Internal
{
    public class SingleStoreOptions : ISingleStoreOptions
    {
        private static readonly SingleStoreSchemaNameTranslator _ignoreSchemaNameTranslator = (_, objectName) => objectName;

        public SingleStoreOptions()
        {
            ConnectionSettings = new SingleStoreConnectionSettings();
            DataSource = null;
            ServerVersion = null;

            // We explicitly use `utf8` in all instances, where charset based calculations need to be done, but accessing annotations
            // isn't possible (e.g. in `SingleStoreTypeMappingSource`).
            // This is also being used as the universal fallback character set, if no character set was explicitly defined for the model,
            // which will result in similar behavior as in previous versions and ensure that databases use a decent/the recommended charset
            // by default, if none was explicitly set.
            DefaultCharSet = CharSet.Utf8Mb4;

            // Optimize space and performance for GUID columns.
            DefaultGuidCollation = "utf8mb4_bin";

            ReplaceLineBreaksWithCharFunction = true;
            DefaultDataTypeMappings = new SingleStoreDefaultDataTypeMappings();

            // Throw by default if a schema is being used with any type.
            SchemaNameTranslator = null;

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;

            LimitKeyedOrIndexedStringColumnLength = true;
            StringComparisonTranslations = false;
            PrimitiveCollectionsSupport = false;
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<SingleStoreOptionsExtension>() ?? new SingleStoreOptionsExtension();
            var mySqlJsonOptions = (SingleStoreJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is SingleStoreJsonOptionsExtension);

            ConnectionSettings = GetConnectionSettings(mySqlOptions);
            DataSource = mySqlOptions.DataSource;
            ServerVersion = mySqlOptions.ServerVersion ?? throw new InvalidOperationException($"The {nameof(ServerVersion)} has not been set.");
            NoBackslashEscapes = mySqlOptions.NoBackslashEscapes;
            ReplaceLineBreaksWithCharFunction = mySqlOptions.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = ApplyDefaultDataTypeMappings(mySqlOptions.DefaultDataTypeMappings, ConnectionSettings);
            SchemaNameTranslator = mySqlOptions.SchemaNameTranslator ?? (mySqlOptions.SchemaBehavior == SingleStoreSchemaBehavior.Ignore
                ? _ignoreSchemaNameTranslator
                : null);
            IndexOptimizedBooleanColumns = mySqlOptions.IndexOptimizedBooleanColumns;
            JsonChangeTrackingOptions = mySqlJsonOptions?.JsonChangeTrackingOptions ?? default;
            LimitKeyedOrIndexedStringColumnLength = mySqlOptions.LimitKeyedOrIndexedStringColumnLength;
            StringComparisonTranslations = mySqlOptions.StringComparisonTranslations;
            PrimitiveCollectionsSupport = mySqlOptions.PrimitiveCollectionsSupport;
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var mySqlOptions = options.FindExtension<SingleStoreOptionsExtension>() ?? new SingleStoreOptionsExtension();
            var mySqlJsonOptions = (SingleStoreJsonOptionsExtension)options.Extensions.LastOrDefault(e => e is SingleStoreJsonOptionsExtension);
            var connectionSettings = GetConnectionSettings(mySqlOptions);

            //
            // CHECK: To we have to ensure that the ApplicationServiceProvider itself is not replaced, because we rely on it in our
            //        DbDataSource check, or is that not possible?
            //

            // Even though we only save a DbDataSource that has been explicitly set using the SingleStoreOptionsExtensions here in SingleStoreOptions,
            // we will later also fall back to a DbDataSource that has been added as a service to the ApplicationServiceProvider, if no
            // DbDataSource has been explicitly set here. We call that DbDataSource the "effective" DbDataSource and handle it in the same
            // way we would handle a singleton option.
            var effectiveDataSource = mySqlOptions.DataSource ??
                                      options.FindExtension<CoreOptionsExtension>()?.ApplicationServiceProvider?.GetService<SingleStoreDataSource>();
            if (effectiveDataSource is not null &&
                !ReferenceEquals(DataSource, mySqlOptions.DataSource))
            {
                throw new InvalidOperationException(
                    SingleStoreStrings.TwoDataSourcesInSameServiceProvider(nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ServerVersion, mySqlOptions.ServerVersion))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreOptionsExtension.ServerVersion),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ConnectionSettings.TreatTinyAsBoolean, connectionSettings.TreatTinyAsBoolean))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreConnectionStringBuilder.TreatTinyAsBoolean),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ConnectionSettings.GuidFormat, connectionSettings.GuidFormat))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreConnectionStringBuilder.GuidFormat),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(NoBackslashEscapes, mySqlOptions.NoBackslashEscapes))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.DisableBackslashEscaping),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(ReplaceLineBreaksWithCharFunction, mySqlOptions.ReplaceLineBreaksWithCharFunction))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.DisableLineBreakToCharSubstition),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(DefaultDataTypeMappings, ApplyDefaultDataTypeMappings(mySqlOptions.DefaultDataTypeMappings ?? new SingleStoreDefaultDataTypeMappings(), connectionSettings)))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.DefaultDataTypeMappings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(
                SchemaNameTranslator,
                mySqlOptions.SchemaBehavior == SingleStoreSchemaBehavior.Ignore
                    ? _ignoreSchemaNameTranslator
                    : SchemaNameTranslator))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.SchemaBehavior),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(IndexOptimizedBooleanColumns, mySqlOptions.IndexOptimizedBooleanColumns))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.EnableIndexOptimizedBooleanColumns),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(JsonChangeTrackingOptions, mySqlJsonOptions?.JsonChangeTrackingOptions ?? default))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreJsonOptionsExtension.JsonChangeTrackingOptions),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(LimitKeyedOrIndexedStringColumnLength, mySqlOptions.LimitKeyedOrIndexedStringColumnLength))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.LimitKeyedOrIndexedStringColumnLength),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(StringComparisonTranslations, mySqlOptions.StringComparisonTranslations))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.EnableStringComparisonTranslations),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }

            if (!Equals(PrimitiveCollectionsSupport, mySqlOptions.PrimitiveCollectionsSupport))
            {
                throw new InvalidOperationException(
                    CoreStrings.SingletonOptionChanged(
                        nameof(SingleStoreDbContextOptionsBuilder.EnablePrimitiveCollectionsSupport),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)));
            }
        }

        protected virtual SingleStoreDefaultDataTypeMappings ApplyDefaultDataTypeMappings(SingleStoreDefaultDataTypeMappings defaultDataTypeMappings, SingleStoreConnectionSettings connectionSettings)
        {
            defaultDataTypeMappings ??= DefaultDataTypeMappings;

            // Explicitly set SingleStoreDefaultDataTypeMappings values take precedence over connection string options.
            if (connectionSettings.TreatTinyAsBoolean.HasValue &&
                defaultDataTypeMappings.ClrBoolean == SingleStoreBooleanType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrBoolean(
                    connectionSettings.TreatTinyAsBoolean.Value
                        ? SingleStoreBooleanType.TinyInt1
                        : SingleStoreBooleanType.Bit1);
            }

            if (defaultDataTypeMappings.ClrDateTime == SingleStoreDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTime(
                    ServerVersion.Supports.DateTime6
                        ? SingleStoreDateTimeType.DateTime6
                        : SingleStoreDateTimeType.DateTime);
            }

            if (defaultDataTypeMappings.ClrDateTimeOffset == SingleStoreDateTimeType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrDateTimeOffset(
                    ServerVersion.Supports.DateTime6
                        ? SingleStoreDateTimeType.DateTime6
                        : SingleStoreDateTimeType.DateTime);
            }

            if (defaultDataTypeMappings.ClrTimeSpan == SingleStoreTimeSpanType.Default)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeSpan(
                    ServerVersion.Supports.DateTime6
                        ? SingleStoreTimeSpanType.Time6
                        : SingleStoreTimeSpanType.Time);
            }

            if (defaultDataTypeMappings.ClrTimeOnlyPrecision < 0)
            {
                defaultDataTypeMappings = defaultDataTypeMappings.WithClrTimeOnly(
                    ServerVersion.Supports.DateTime6
                        ? 6
                        : 0);
            }

            return defaultDataTypeMappings;
        }

        private static SingleStoreConnectionSettings GetConnectionSettings(SingleStoreOptionsExtension relationalOptions)
            => relationalOptions.Connection != null
                ? new SingleStoreConnectionSettings(relationalOptions.Connection)
                : new SingleStoreConnectionSettings(relationalOptions.ConnectionString);

        protected virtual bool Equals(SingleStoreOptions other)
        {
            return Equals(ConnectionSettings, other.ConnectionSettings) &&
                   ReferenceEquals(DataSource, other.DataSource) &&
                   Equals(ServerVersion, other.ServerVersion) &&
                   Equals(DefaultCharSet, other.DefaultCharSet) &&
                   Equals(NationalCharSet, other.NationalCharSet) &&
                   Equals(DefaultGuidCollation, other.DefaultGuidCollation) &&
                   NoBackslashEscapes == other.NoBackslashEscapes &&
                   ReplaceLineBreaksWithCharFunction == other.ReplaceLineBreaksWithCharFunction &&
                   Equals(DefaultDataTypeMappings, other.DefaultDataTypeMappings) &&
                   Equals(SchemaNameTranslator, other.SchemaNameTranslator) &&
                   IndexOptimizedBooleanColumns == other.IndexOptimizedBooleanColumns &&
                   JsonChangeTrackingOptions == other.JsonChangeTrackingOptions &&
                   LimitKeyedOrIndexedStringColumnLength == other.LimitKeyedOrIndexedStringColumnLength &&
                   StringComparisonTranslations == other.StringComparisonTranslations &&
                   PrimitiveCollectionsSupport == other.PrimitiveCollectionsSupport;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SingleStoreOptions)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(ConnectionSettings);
            hashCode.Add(DataSource?.ConnectionString);
            hashCode.Add(ServerVersion);
            hashCode.Add(DefaultCharSet);
            hashCode.Add(NationalCharSet);
            hashCode.Add(DefaultGuidCollation);
            hashCode.Add(NoBackslashEscapes);
            hashCode.Add(ReplaceLineBreaksWithCharFunction);
            hashCode.Add(DefaultDataTypeMappings);
            hashCode.Add(SchemaNameTranslator);
            hashCode.Add(IndexOptimizedBooleanColumns);
            hashCode.Add(JsonChangeTrackingOptions);
            hashCode.Add(LimitKeyedOrIndexedStringColumnLength);
            hashCode.Add(StringComparisonTranslations);
            hashCode.Add(PrimitiveCollectionsSupport);

            return hashCode.ToHashCode();
        }

        public virtual SingleStoreConnectionSettings ConnectionSettings { get; private set; }

        /// <summary>
        /// If null, there might still be a `DbDataSource` in the ApplicationServiceProvider.
        /// </summary>
        public virtual DbDataSource DataSource { get; private set; }

        public virtual ServerVersion ServerVersion { get; private set; }
        public virtual CharSet DefaultCharSet { get; private set; }
        public virtual CharSet NationalCharSet { get; }
        public virtual string DefaultGuidCollation { get; private set; }
        public virtual bool NoBackslashEscapes { get; private set; }
        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }
        public virtual SingleStoreDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }
        public virtual SingleStoreSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public virtual bool IndexOptimizedBooleanColumns { get; private set; }
        public virtual SingleStoreJsonChangeTrackingOptions JsonChangeTrackingOptions { get; private set; }
        public virtual bool LimitKeyedOrIndexedStringColumnLength { get; private set; }
        public virtual bool StringComparisonTranslations { get; private set; }
        public virtual bool PrimitiveCollectionsSupport { get; private set; }
    }
}
