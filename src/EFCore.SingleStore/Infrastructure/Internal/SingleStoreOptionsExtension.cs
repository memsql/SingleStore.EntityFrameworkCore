// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Infrastructure.Internal
{
    public class SingleStoreOptionsExtension : RelationalOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        public SingleStoreOptionsExtension()
        {
            ReplaceLineBreaksWithCharFunction = true;

            // TODO: Change to `true` for EF Core 5.
            IndexOptimizedBooleanColumns = false;

            LimitKeyedOrIndexedStringColumnLength = true;
        }

        public SingleStoreOptionsExtension([NotNull] SingleStoreOptionsExtension copyFrom)
            : base(copyFrom)
        {
            DataSource = copyFrom.DataSource;
            ServerVersion = copyFrom.ServerVersion;
            NoBackslashEscapes = copyFrom.NoBackslashEscapes;
            UpdateSqlModeOnOpen = copyFrom.UpdateSqlModeOnOpen;
            ReplaceLineBreaksWithCharFunction = copyFrom.ReplaceLineBreaksWithCharFunction;
            DefaultDataTypeMappings = copyFrom.DefaultDataTypeMappings;
            SchemaBehavior = copyFrom.SchemaBehavior;
            SchemaNameTranslator = copyFrom.SchemaNameTranslator;
            IndexOptimizedBooleanColumns = copyFrom.IndexOptimizedBooleanColumns;
            LimitKeyedOrIndexedStringColumnLength = copyFrom.LimitKeyedOrIndexedStringColumnLength;
            StringComparisonTranslations = copyFrom.StringComparisonTranslations;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        protected override RelationalOptionsExtension Clone()
            => new SingleStoreOptionsExtension(this);

        /// <summary>
        ///     The <see cref="DbDataSource" />, or <see langword="null" /> if a connection string or <see cref="DbConnection" /> was used
        ///     instead of a <see cref="DbDataSource" />.
        /// </summary>
        public virtual DbDataSource DataSource { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ServerVersion ServerVersion { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool NoBackslashEscapes { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool UpdateSqlModeOnOpen { get; private set; }

        public virtual bool ReplaceLineBreaksWithCharFunction { get; private set; }

        public virtual SingleStoreDefaultDataTypeMappings DefaultDataTypeMappings { get; private set; }

        public virtual SingleStoreSchemaBehavior SchemaBehavior { get; private set; }
        public virtual SingleStoreSchemaNameTranslator SchemaNameTranslator { get; private set; }
        public virtual bool IndexOptimizedBooleanColumns { get; private set; }
        public virtual bool LimitKeyedOrIndexedStringColumnLength { get; private set; }
        public virtual bool StringComparisonTranslations { get; private set; }
        public virtual bool PrimitiveCollectionsSupport { get; private set; }

        /// <summary>
        ///     Creates a new instance with all options the same as for this instance, but with the given option changed.
        ///     It is unusual to call this method directly. Instead use <see cref="DbContextOptionsBuilder" />.
        /// </summary>
        /// <param name="dataSource">The option to change.</param>
        /// <returns>A new instance with the option changed.</returns>
        public virtual RelationalOptionsExtension WithDataSource(DbDataSource dataSource)
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.DataSource = dataSource;
            return clone;
        }

        /// <inheritdoc />
        public override RelationalOptionsExtension WithConnectionString(string connectionString)
        {
            var clone = (SingleStoreOptionsExtension)base.WithConnectionString(connectionString);
            clone.DataSource = null;
            return clone;
        }

        /// <inheritdoc />
        public override RelationalOptionsExtension WithConnection(DbConnection connection)
        {
            var clone = (SingleStoreOptionsExtension)base.WithConnection(connection);
            clone.DataSource = null;
            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SingleStoreOptionsExtension WithServerVersion(ServerVersion serverVersion)
        {
            var clone = (SingleStoreOptionsExtension)Clone();

            clone.ServerVersion = serverVersion;

            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SingleStoreOptionsExtension WithDisabledBackslashEscaping()
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.NoBackslashEscapes = true;
            return clone;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual SingleStoreOptionsExtension WithSettingSqlModeOnOpen()
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.UpdateSqlModeOnOpen = true;
            return clone;
        }

        public virtual SingleStoreOptionsExtension WithDisabledLineBreakToCharSubstition()
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.ReplaceLineBreaksWithCharFunction = false;
            return clone;
        }

        public virtual SingleStoreOptionsExtension WithDefaultDataTypeMappings(SingleStoreDefaultDataTypeMappings defaultDataTypeMappings)
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.DefaultDataTypeMappings = defaultDataTypeMappings;
            return clone;
        }

        public virtual SingleStoreOptionsExtension WithSchemaBehavior(SingleStoreSchemaBehavior behavior, SingleStoreSchemaNameTranslator translator = null)
        {
            if (behavior == SingleStoreSchemaBehavior.Translate && translator == null)
            {
                throw new ArgumentException($"The {nameof(translator)} parameter is mandatory when using `{nameof(SingleStoreSchemaBehavior)}.{nameof(SingleStoreSchemaBehavior.Translate)}` as the specified behavior.");
            }

            var clone = (SingleStoreOptionsExtension)Clone();

            clone.SchemaBehavior = behavior;
            clone.SchemaNameTranslator = behavior == SingleStoreSchemaBehavior.Translate
                ? translator
                : null;

            return clone;
        }

        public virtual SingleStoreOptionsExtension WithIndexOptimizedBooleanColumns(bool enable)
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.IndexOptimizedBooleanColumns = enable;
            return clone;
        }

        public virtual SingleStoreOptionsExtension WithKeyedOrIndexedStringColumnLengthLimit(bool enable)
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.LimitKeyedOrIndexedStringColumnLength = enable;
            return clone;
        }

        public virtual SingleStoreOptionsExtension WithStringComparisonTranslations(bool enable)
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.StringComparisonTranslations = enable;
            return clone;
        }

        public virtual SingleStoreOptionsExtension WithPrimitiveCollectionsSupport(bool enable)
        {
            var clone = (SingleStoreOptionsExtension)Clone();
            clone.PrimitiveCollectionsSupport = enable;
            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkSingleStore();

        private sealed class ExtensionInfo : RelationalExtensionInfo
        {
            private int? _serviceProviderHash;
            private string _logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new SingleStoreOptionsExtension Extension
                => (SingleStoreOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append(base.LogFragment);

                        if (Extension.ServerVersion != null)
                        {
                            builder.Append("ServerVersion=")
                                .Append(Extension.ServerVersion)
                                .Append(" ");
                        }

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override int GetServiceProviderHashCode()
            {
                if (_serviceProviderHash == null)
                {
                    var hashCode = new HashCode();
                    hashCode.Add(base.GetServiceProviderHashCode());
                    hashCode.Add(Extension.DataSource?.ConnectionString);
                    hashCode.Add(Extension.ServerVersion);
                    hashCode.Add(Extension.NoBackslashEscapes);
                    hashCode.Add(Extension.UpdateSqlModeOnOpen);
                    hashCode.Add(Extension.ReplaceLineBreaksWithCharFunction);
                    hashCode.Add(Extension.DefaultDataTypeMappings);
                    hashCode.Add(Extension.SchemaBehavior);
                    hashCode.Add(Extension.SchemaNameTranslator);
                    hashCode.Add(Extension.IndexOptimizedBooleanColumns);
                    hashCode.Add(Extension.LimitKeyedOrIndexedStringColumnLength);
                    hashCode.Add(Extension.StringComparisonTranslations);
                    hashCode.Add(Extension.PrimitiveCollectionsSupport);

                    _serviceProviderHash = hashCode.ToHashCode();
                }

                return _serviceProviderHash.Value;
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
                => other is ExtensionInfo otherInfo &&
                   base.ShouldUseSameServiceProvider(other) &&
                   ReferenceEquals(Extension.DataSource, otherInfo.Extension.DataSource) &&
                   Equals(Extension.ServerVersion, otherInfo.Extension.ServerVersion) &&
                   Extension.NoBackslashEscapes == otherInfo.Extension.NoBackslashEscapes &&
                   Extension.UpdateSqlModeOnOpen == otherInfo.Extension.UpdateSqlModeOnOpen &&
                   Extension.ReplaceLineBreaksWithCharFunction == otherInfo.Extension.ReplaceLineBreaksWithCharFunction &&
                   Equals(Extension.DefaultDataTypeMappings, otherInfo.Extension.DefaultDataTypeMappings) &&
                   Extension.SchemaBehavior == otherInfo.Extension.SchemaBehavior &&
                   Extension.SchemaNameTranslator == otherInfo.Extension.SchemaNameTranslator &&
                   Extension.IndexOptimizedBooleanColumns == otherInfo.Extension.IndexOptimizedBooleanColumns &&
                   Extension.LimitKeyedOrIndexedStringColumnLength == otherInfo.Extension.LimitKeyedOrIndexedStringColumnLength &&
                   Extension.StringComparisonTranslations == otherInfo.Extension.StringComparisonTranslations &&
                   Extension.PrimitiveCollectionsSupport == otherInfo.Extension.PrimitiveCollectionsSupport;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(Extension.ServerVersion)] = HashCode.Combine(Extension.ServerVersion).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.DisableBackslashEscaping)] = HashCode.Combine(Extension.NoBackslashEscapes).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.SetSqlModeOnOpen)] = HashCode.Combine(Extension.UpdateSqlModeOnOpen).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.DisableLineBreakToCharSubstition)] = HashCode.Combine(Extension.ReplaceLineBreaksWithCharFunction).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.DefaultDataTypeMappings)] = HashCode.Combine(Extension.DefaultDataTypeMappings).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.SchemaBehavior)] = HashCode.Combine(Extension.SchemaBehavior).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(Extension.SchemaNameTranslator)] = HashCode.Combine(Extension.SchemaNameTranslator).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.EnableIndexOptimizedBooleanColumns)] = HashCode.Combine(Extension.IndexOptimizedBooleanColumns).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.LimitKeyedOrIndexedStringColumnLength)] = HashCode.Combine(Extension.LimitKeyedOrIndexedStringColumnLength).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.EnableStringComparisonTranslations)] = HashCode.Combine(Extension.StringComparisonTranslations).ToString(CultureInfo.InvariantCulture);
                debugInfo["EntityFrameworkCore.SingleStore:" + nameof(SingleStoreDbContextOptionsBuilder.EnablePrimitiveCollectionsSupport)] = HashCode.Combine(Extension.PrimitiveCollectionsSupport).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
