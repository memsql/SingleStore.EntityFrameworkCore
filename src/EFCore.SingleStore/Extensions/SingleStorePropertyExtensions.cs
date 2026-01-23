// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EntityFrameworkCore.SingleStore.Internal;
using EntityFrameworkCore.SingleStore.Metadata.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     MySQL specific extension methods for properties.
    /// </summary>
    public static class SingleStorePropertyExtensions
    {
        /// <summary>
        ///     <para>
        ///         Returns the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property.
        ///     </para>
        ///     <para>
        ///         If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        ///     </para>
        /// </summary>
        /// <returns> The strategy, or <see cref="SingleStoreValueGenerationStrategy.None"/> if none was set. </returns>
        public static SingleStoreValueGenerationStrategy GetValueGenerationStrategy([NotNull] this IReadOnlyProperty property)
        {
            if (property.FindAnnotation(SingleStoreAnnotationNames.ValueGenerationStrategy) is { } annotation)
            {
                if (annotation.Value is { } annotationValue)
                {
                    // Allow users to use the underlying type value instead of the enum itself.
                    // Workaround for: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1205
                    if (ObjectToEnumConverter.GetEnumValue<SingleStoreValueGenerationStrategy>(annotationValue) is { } enumValue)
                    {
                        return enumValue;
                    }

                    return (SingleStoreValueGenerationStrategy)annotationValue;
                }

                return SingleStoreValueGenerationStrategy.None;
            }

            if (property.ValueGenerated == ValueGenerated.OnAdd)
            {
                if (property.IsForeignKey()
                    || property.TryGetDefaultValue(out _)
                    || property.GetDefaultValueSql() != null
                    || property.GetComputedColumnSql() != null)
                {
                    return SingleStoreValueGenerationStrategy.None;
                }

                return GetDefaultValueGenerationStrategy(property);
            }

            if (property.ValueGenerated == ValueGenerated.OnAddOrUpdate)
            {
                // We explicitly check for RowVersion when generation migrations. We therefore handle RowVersion separately from other cases
                // of using CURRENT_TIMESTAMP etc. and we don't generate a SingleStoreValueGenerationStrategy.ComputedColumn annotation.
                if (IsCompatibleComputedColumn(property) &&
                    !property.IsConcurrencyToken)
                {
                    return SingleStoreValueGenerationStrategy.ComputedColumn;
                }
            }

            return SingleStoreValueGenerationStrategy.None;
        }

        public static SingleStoreValueGenerationStrategy GetValueGenerationStrategy(
            this IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject)
            => GetValueGenerationStrategy(property, storeObject, null);

        internal static SingleStoreValueGenerationStrategy GetValueGenerationStrategy(
            this IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            [CanBeNull] ITypeMappingSource typeMappingSource)
        {
            if (property.FindOverrides(storeObject)?.FindAnnotation(SingleStoreAnnotationNames.ValueGenerationStrategy) is { } @override)
            {
                return ObjectToEnumConverter.GetEnumValue<SingleStoreValueGenerationStrategy>(@override.Value) ?? SingleStoreValueGenerationStrategy.None;
            }

            var annotation = property.FindAnnotation(SingleStoreAnnotationNames.ValueGenerationStrategy);
            if (annotation?.Value is { } annotationValue
                && ObjectToEnumConverter.GetEnumValue<SingleStoreValueGenerationStrategy>(annotationValue) is { } enumValue
                && StoreObjectIdentifier.Create(property.DeclaringType, storeObject.StoreObjectType) == storeObject)
            {
                return enumValue;
            }

            var table = storeObject;
            var sharedTableRootProperty = property.FindSharedStoreObjectRootProperty(storeObject);
            if (sharedTableRootProperty != null)
            {
                return sharedTableRootProperty.GetValueGenerationStrategy(storeObject, typeMappingSource)
                    == SingleStoreValueGenerationStrategy.IdentityColumn
                    && table.StoreObjectType == StoreObjectType.Table
                    && !property.GetContainingForeignKeys().Any(
                        fk =>
                            !fk.IsBaseLinking()
                            || (StoreObjectIdentifier.Create(fk.PrincipalEntityType, StoreObjectType.Table)
                                    is StoreObjectIdentifier principal
                                && fk.GetConstraintName(table, principal) != null))
                        ? SingleStoreValueGenerationStrategy.IdentityColumn
                        : SingleStoreValueGenerationStrategy.None;
            }

            if (property.ValueGenerated == ValueGenerated.OnAdd)
            {
                if (table.StoreObjectType != StoreObjectType.Table
                    || property.TryGetDefaultValue(storeObject, out _)
                    || property.GetDefaultValueSql(storeObject) != null
                    || property.GetComputedColumnSql(storeObject) != null
                    || property.GetContainingForeignKeys()
                        .Any(
                            fk =>
                                !fk.IsBaseLinking()
                                || (StoreObjectIdentifier.Create(fk.PrincipalEntityType, StoreObjectType.Table)
                                        is StoreObjectIdentifier principal
                                    && fk.GetConstraintName(table, principal) != null)))
                {
                    return SingleStoreValueGenerationStrategy.None;
                }

                var defaultStrategy = GetDefaultValueGenerationStrategy(property, storeObject, typeMappingSource);
                if (defaultStrategy != SingleStoreValueGenerationStrategy.None)
                {
                    if (annotation != null)
                    {
                        return (SingleStoreValueGenerationStrategy?)annotation.Value ?? SingleStoreValueGenerationStrategy.None;
                    }
                }

                return defaultStrategy;
            }

            if (property.ValueGenerated == ValueGenerated.OnAddOrUpdate)
            {
                // We explicitly check for RowVersion when generation migrations. We therefore handle RowVersion separately from other cases
                // of using CURRENT_TIMESTAMP etc. and we don't generate a SingleStoreValueGenerationStrategy.ComputedColumn annotation.
                if (IsCompatibleComputedColumn(property, storeObject, typeMappingSource) &&
                    !property.IsConcurrencyToken)
                {
                    return SingleStoreValueGenerationStrategy.ComputedColumn;
                }
            }

            return SingleStoreValueGenerationStrategy.None;
        }

        /// <summary>
        ///     Returns the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <remarks>
        ///     If no strategy is set for the property, then the strategy to use will be taken from the <see cref="IModel" />.
        /// </remarks>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The strategy, or <see cref="SingleStoreValueGenerationStrategy.None" /> if none was set.</returns>
        public static SingleStoreValueGenerationStrategy? GetValueGenerationStrategy(this IReadOnlyRelationalPropertyOverrides overrides)
            => overrides.FindAnnotation(SingleStoreAnnotationNames.ValueGenerationStrategy) is { } @override
                ? ObjectToEnumConverter.GetEnumValue<SingleStoreValueGenerationStrategy>(@override.Value) ??
                  SingleStoreValueGenerationStrategy.None
                : null;

        private static SingleStoreValueGenerationStrategy GetDefaultValueGenerationStrategy(IReadOnlyProperty property)
        {
            var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

            return modelStrategy == SingleStoreValueGenerationStrategy.IdentityColumn &&
                   IsCompatibleIdentityColumn(property)
                ? SingleStoreValueGenerationStrategy.IdentityColumn
                : SingleStoreValueGenerationStrategy.None;
        }

        private static SingleStoreValueGenerationStrategy GetDefaultValueGenerationStrategy(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            [CanBeNull] ITypeMappingSource typeMappingSource)
        {
            var modelStrategy = property.DeclaringType.Model.GetValueGenerationStrategy();

            return modelStrategy == SingleStoreValueGenerationStrategy.IdentityColumn
                   && IsCompatibleIdentityColumn(property, storeObject, typeMappingSource)
                ? SingleStoreValueGenerationStrategy.IdentityColumn
                : SingleStoreValueGenerationStrategy.None;
        }

        /// <summary>
        ///     Sets the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        public static void SetValueGenerationStrategy(
            [NotNull] this IMutableProperty property,
            SingleStoreValueGenerationStrategy? value)
            => property.SetOrRemoveAnnotation(
                SingleStoreAnnotationNames.ValueGenerationStrategy,
                CheckValueGenerationStrategy(property, value));

        /// <summary>
        ///     Sets the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <param name="value"> The strategy to use. </param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static SingleStoreValueGenerationStrategy? SetValueGenerationStrategy(
            [NotNull] this IConventionProperty property,
            SingleStoreValueGenerationStrategy? value,
            bool fromDataAnnotation = false)
            => (SingleStoreValueGenerationStrategy?)property.SetOrRemoveAnnotation(
                    SingleStoreAnnotationNames.ValueGenerationStrategy,
                    CheckValueGenerationStrategy(property, value),
                    fromDataAnnotation)
                ?.Value;

        /// <summary>
        ///     Sets the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        public static void SetValueGenerationStrategy(
            this IMutableProperty property,
            SingleStoreValueGenerationStrategy? value,
            in StoreObjectIdentifier storeObject)
            => property.GetOrCreateOverrides(storeObject)
                .SetValueGenerationStrategy(value);

        /// <summary>
        ///     Sets the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static SingleStoreValueGenerationStrategy? SetValueGenerationStrategy(
            this IConventionProperty property,
            SingleStoreValueGenerationStrategy? value,
            in StoreObjectIdentifier storeObject,
            bool fromDataAnnotation = false)
            => property.GetOrCreateOverrides(storeObject, fromDataAnnotation)
                .SetValueGenerationStrategy(value, fromDataAnnotation);

        /// <summary>
        ///     Sets the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="value">The strategy to use.</param>
        public static void SetValueGenerationStrategy(
            this IMutableRelationalPropertyOverrides overrides,
            SingleStoreValueGenerationStrategy? value)
            => overrides.SetOrRemoveAnnotation(
                SingleStoreAnnotationNames.ValueGenerationStrategy,
                CheckValueGenerationStrategy(overrides.Property, value));

        /// <summary>
        ///     Sets the <see cref="SingleStoreValueGenerationStrategy" /> to use for the property for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <param name="value">The strategy to use.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>The configured value.</returns>
        public static SingleStoreValueGenerationStrategy? SetValueGenerationStrategy(
            this IConventionRelationalPropertyOverrides overrides,
            SingleStoreValueGenerationStrategy? value,
            bool fromDataAnnotation = false)
            => (SingleStoreValueGenerationStrategy?)overrides.SetOrRemoveAnnotation(
                SingleStoreAnnotationNames.ValueGenerationStrategy,
                CheckValueGenerationStrategy(overrides.Property, value),
                fromDataAnnotation)?.Value;

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the <see cref="SingleStoreValueGenerationStrategy" />.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the <see cref="SingleStoreValueGenerationStrategy" />.</returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(
            this IConventionProperty property)
            => property.FindAnnotation(SingleStoreAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the <see cref="SingleStoreValueGenerationStrategy" /> for a particular table.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="storeObject">The identifier of the table containing the column.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the <see cref="SingleStoreValueGenerationStrategy" />.</returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(
            this IConventionProperty property,
            in StoreObjectIdentifier storeObject)
            => property.FindOverrides(storeObject)?.GetValueGenerationStrategyConfigurationSource();

        /// <summary>
        ///     Returns the <see cref="ConfigurationSource" /> for the <see cref="SingleStoreValueGenerationStrategy" /> for a particular table.
        /// </summary>
        /// <param name="overrides">The property overrides.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the <see cref="SingleStoreValueGenerationStrategy" />.</returns>
        public static ConfigurationSource? GetValueGenerationStrategyConfigurationSource(
            this IConventionRelationalPropertyOverrides overrides)
            => overrides.FindAnnotation(SingleStoreAnnotationNames.ValueGenerationStrategy)?.GetConfigurationSource();

        private static SingleStoreValueGenerationStrategy? CheckValueGenerationStrategy(IReadOnlyProperty property, SingleStoreValueGenerationStrategy? value)
        {
            if (value == null)
            {
                return null;
            }

            var propertyType = property.ClrType;

            if (value == SingleStoreValueGenerationStrategy.IdentityColumn
                && !IsCompatibleIdentityColumn(property))
            {
                throw new ArgumentException(
                    SingleStoreStrings.IdentityBadType(
                        property.Name, property.DeclaringType.DisplayName(), propertyType.ShortDisplayName()));
            }

            if (value == SingleStoreValueGenerationStrategy.ComputedColumn
                && !IsCompatibleComputedColumn(property))
            {
                throw new ArgumentException(
                    SingleStoreStrings.ComputedBadType(
                        property.Name, property.DeclaringType.DisplayName(), propertyType.ShortDisplayName()));
            }

            return value;
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="SingleStoreValueGenerationStrategy.IdentityColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleIdentityColumn(IReadOnlyProperty property)
            => IsCompatibleAutoIncrementColumn(property) ||
               IsCompatibleCurrentTimestampColumn(property);

        private static bool IsCompatibleIdentityColumn(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            [CanBeNull] ITypeMappingSource typeMappingSource)
            => IsCompatibleAutoIncrementColumn(property, storeObject, typeMappingSource) ||
               IsCompatibleCurrentTimestampColumn(property, storeObject, typeMappingSource);

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with an `AUTO_INCREMENT` column.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleAutoIncrementColumn(IReadOnlyProperty property)
        {
            var valueConverter = property.GetValueConverter() ??
                        property.FindTypeMapping()?.Converter;

            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
            return type.IsInteger() ||
                   type.IsEnum ||
                   type == typeof(decimal);
        }

        private static bool IsCompatibleAutoIncrementColumn(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            [CanBeNull] ITypeMappingSource typeMappingSource)
        {
            if (storeObject.StoreObjectType != StoreObjectType.Table)
            {
                return false;
            }

            var valueConverter = property.GetValueConverter() ??
                        (property.FindRelationalTypeMapping(storeObject) ??
                        typeMappingSource?.FindMapping((IProperty)property))?.Converter;
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();

            return (type.IsInteger() ||
                    type.IsEnum ||
                    type == typeof(decimal));
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with a `CURRENT_TIMESTAMP` column default.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleCurrentTimestampColumn(IReadOnlyProperty property)
        {
            var valueConverter = GetConverter(property);
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();
            return type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset);
        }

        private static bool IsCompatibleCurrentTimestampColumn(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            [CanBeNull] ITypeMappingSource typeMappingSource)
        {
            if (storeObject.StoreObjectType != StoreObjectType.Table)
            {
                return false;
            }

            var valueConverter = GetConverter(property, storeObject, typeMappingSource);
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();

            return type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset);
        }

        /// <summary>
        ///     Returns a value indicating whether the property is compatible with <see cref="SingleStoreValueGenerationStrategy.ComputedColumn"/>.
        /// </summary>
        /// <param name="property"> The property. </param>
        /// <returns> <see langword="true"/> if compatible. </returns>
        public static bool IsCompatibleComputedColumn(IReadOnlyProperty property)
        {
            var valueConverter = GetConverter(property);
            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();

            // RowVersion uses byte[] and the BytesToDateTimeConverter.
            return type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(byte[]) && valueConverter is BytesToDateTimeConverter;
        }

        private static bool IsCompatibleComputedColumn(
            IReadOnlyProperty property,
            in StoreObjectIdentifier storeObject,
            ITypeMappingSource typeMappingSource)
        {
            if (storeObject.StoreObjectType != StoreObjectType.Table)
            {
                return false;
            }

            var valueConverter = property.GetValueConverter() ??
                                 (property.FindRelationalTypeMapping(storeObject) ??
                                  typeMappingSource?.FindMapping((IProperty)property))?.Converter;

            var type = (valueConverter?.ProviderClrType ?? property.ClrType).UnwrapNullableType();

            // RowVersion uses byte[] and the BytesToDateTimeConverter.
            return type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(byte[]) && valueConverter is BytesToDateTimeConverter;
        }

        private static ValueConverter GetConverter(IReadOnlyProperty property)
            => property.GetValueConverter() ??
               property.FindTypeMapping()?.Converter;

        private static ValueConverter GetConverter(
            IReadOnlyProperty property,
            StoreObjectIdentifier storeObject,
            [CanBeNull] ITypeMappingSource typeMappingSource)
            => property.GetValueConverter()
               ?? (property.FindRelationalTypeMapping(storeObject)
                   ?? typeMappingSource?.FindMapping((IProperty)property))?.Converter;

        /// <summary>
        /// Returns the name of the charset used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns charset from.</param>
        /// <returns>The name of the charset or null, if no explicit charset was set.</returns>
        public static string GetCharSet([NotNull] this IReadOnlyProperty property)
            => (property is RuntimeProperty)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : property[SingleStoreAnnotationNames.CharSet] as string ??
                  property.GetSingleStoreLegacyCharSet();

        /// <summary>
        /// Returns the name of the charset used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns charset from.</param>
        /// <param name="storeObject">The identifier of the table-like store object containing the column.</param>
        /// <returns>The name of the charset or null, if no explicit charset was set.</returns>
        public static string GetCharSet(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
            => property is RuntimeProperty
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : property.FindAnnotation(SingleStoreAnnotationNames.CharSet) is { } annotation
                    ? annotation.Value as string ??
                      property.GetSingleStoreLegacyCharSet()
                    : property.FindSharedStoreObjectRootProperty(storeObject)?.GetCharSet(storeObject);

        /// <summary>
        /// Returns the name of the charset used by the column of the property, defined as part of the column type.
        /// </summary>
        /// <remarks>
        /// It was common before 5.0 to specify charsets this way, because there were no character set specific annotations available yet.
        /// Users might still use migrations generated with previous versions and just add newer migrations on top of those.
        /// </remarks>
        /// <param name="property">The property of which to get the columns charset from.</param>
        /// <returns>The name of the charset or null, if no explicit charset was set.</returns>
        internal static string GetSingleStoreLegacyCharSet([NotNull] this IReadOnlyProperty property)
        {
            var columnType = property.GetColumnType();

            if (columnType is not null)
            {
                const string characterSet = "character set";
                const string charSet = "charset";

                var characterSetOccurrenceIndex = columnType.IndexOf(characterSet, StringComparison.OrdinalIgnoreCase);
                var clauseLength = characterSet.Length;

                if (characterSetOccurrenceIndex < 0)
                {
                    characterSetOccurrenceIndex = columnType.IndexOf(charSet, StringComparison.OrdinalIgnoreCase);
                    clauseLength = charSet.Length;
                }

                if (characterSetOccurrenceIndex >= 0)
                {
                    var result = string.Concat(
                        columnType.Skip(characterSetOccurrenceIndex + clauseLength)
                            .SkipWhile(c => c == ' ')
                            .TakeWhile(c => c != ' '));

                    if (result.Length > 0)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the name of the charset in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns charset for.</param>
        /// <param name="charSet">The name of the charset used for the column of the property.</param>
        public static void SetCharSet([NotNull] this IMutableProperty property, string charSet)
            => property.SetOrRemoveAnnotation(SingleStoreAnnotationNames.CharSet, charSet);

        /// <summary>
        /// Sets the name of the charset in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns charset for.</param>
        /// <param name="charSet">The name of the charset used for the column of the property.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static string SetCharSet([NotNull] this IConventionProperty property, string charSet, bool fromDataAnnotation = false)
        {
            property.SetOrRemoveAnnotation(SingleStoreAnnotationNames.CharSet, charSet, fromDataAnnotation);

            return charSet;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the character set.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the character set.</returns>
        public static ConfigurationSource? GetCharSetConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(SingleStoreAnnotationNames.CharSet)?.GetConfigurationSource();

        /// <summary>
        /// Returns the name of the collation used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns collation from.</param>
        /// <returns>The name of the collation or null, if no explicit collation was set.</returns>
#pragma warning disable 618
        internal static string GetSingleStoreLegacyCollation([NotNull] this IReadOnlyProperty property)
            => property[SingleStoreAnnotationNames.Collation] as string;
#pragma warning restore 618

        /// <summary>
        /// Returns the Spatial Reference System Identifier (SRID) used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns SRID from.</param>
        /// <returns>The SRID or null, if no explicit SRID has been set.</returns>
        public static int? GetSpatialReferenceSystem([NotNull] this IReadOnlyProperty property)
            => (property is RuntimeProperty)
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : (int?)property[SingleStoreAnnotationNames.SpatialReferenceSystemId];

        /// <summary>
        /// Returns the Spatial Reference System Identifier (SRID) used by the column of the property.
        /// </summary>
        /// <param name="property">The property of which to get the columns SRID from.</param>
        /// <param name="storeObject">The identifier of the table-like store object containing the column.</param>
        /// <returns>The SRID or null, if no explicit SRID has been set.</returns>
        public static int? GetSpatialReferenceSystem(this IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
            => property is RuntimeProperty
                ? throw new InvalidOperationException(CoreStrings.RuntimeModelMissingData)
                : property.FindAnnotation(SingleStoreAnnotationNames.SpatialReferenceSystemId) is { } annotation
                    ? (int?)annotation.Value
                    : property.FindSharedStoreObjectRootProperty(storeObject)?.GetSpatialReferenceSystem(storeObject);

        /// <summary>
        /// Sets the Spatial Reference System Identifier (SRID) in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns SRID for.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        public static void SetSpatialReferenceSystem([NotNull] this IMutableProperty property, int? srid)
            => property.SetOrRemoveAnnotation(SingleStoreAnnotationNames.SpatialReferenceSystemId, srid);

        /// <summary>
        /// Sets the Spatial Reference System Identifier (SRID) in use by the column of the property.
        /// </summary>
        /// <param name="property">The property to set the columns SRID for.</param>
        /// <param name="srid">The SRID to configure for the property's column.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        public static int? SetSpatialReferenceSystem([NotNull] this IConventionProperty property, int? srid, bool fromDataAnnotation = false)
        {
            property.SetOrRemoveAnnotation(SingleStoreAnnotationNames.SpatialReferenceSystemId, srid, fromDataAnnotation);

            return srid;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSource" /> for the Spatial Reference System Identifier (SRID).
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The <see cref="ConfigurationSource" /> for the Spatial Reference System Identifier (SRID).</returns>
        public static ConfigurationSource? GetSpatialReferenceSystemConfigurationSource(this IConventionProperty property)
            => property.FindAnnotation(SingleStoreAnnotationNames.SpatialReferenceSystemId)?.GetConfigurationSource();
    }
}
