// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Metadata.Internal;

namespace EntityFrameworkCore.SingleStore.Design.Internal
{
    public class SingleStoreAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        private static readonly MethodInfo _modelHasCharSetMethodInfo
            = typeof(SingleStoreModelBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreModelBuilderExtensions.HasCharSet),
                typeof(ModelBuilder),
                typeof(string),
                typeof(DelegationModes?));

        private static readonly MethodInfo _modelUseCollationMethodInfo
            = typeof(SingleStoreModelBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreModelBuilderExtensions.UseCollation),
                typeof(ModelBuilder),
                typeof(string),
                typeof(DelegationModes?));

        private static readonly MethodInfo _modelUseGuidCollationMethodInfo
            = typeof(SingleStoreModelBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreModelBuilderExtensions.UseGuidCollation),
                typeof(ModelBuilder),
                typeof(string));

        private static readonly MethodInfo _entityTypeHasCharSetMethodInfo
            = typeof(SingleStoreEntityTypeBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreEntityTypeBuilderExtensions.HasCharSet),
                typeof(EntityTypeBuilder),
                typeof(string),
                typeof(DelegationModes?));

        private static readonly MethodInfo _entityTypeUseCollationMethodInfo
            = typeof(SingleStoreEntityTypeBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreEntityTypeBuilderExtensions.UseCollation),
                typeof(EntityTypeBuilder),
                typeof(string),
                typeof(DelegationModes?));

        private static readonly MethodInfo _propertyHasCharSetMethodInfo
            = typeof(SingleStorePropertyBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStorePropertyBuilderExtensions.HasCharSet),
                typeof(PropertyBuilder),
                typeof(string));

        public SingleStoreAnnotationCodeGenerator([NotNull] AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IEnumerable<IAnnotation> FilterIgnoredAnnotations(IEnumerable<IAnnotation> annotations)
        {
            annotations = base.FilterIgnoredAnnotations(annotations).ToArray();

            var hasCharSetAnnotation = annotations.Any(a => a.Name == SingleStoreAnnotationNames.CharSet);
            var hasCollationAnnotation = annotations.Any(a => a.Name == RelationalAnnotationNames.Collation);

            foreach (var annotation in annotations)
            {
                // Charsets and their delegation and collations and their delegation are handled in the same Fluent API call.
                // Since the GenerateFluentApi methods cannot skip annotations, we have to ignore one of them here early, if both have been
                // set, so we don't output a HasCharSet()/UseCollation() call and a CharSetDelegation/CollationDelegation annotation in
                // addition to that.
                if (annotation.Name == SingleStoreAnnotationNames.CharSetDelegation && hasCharSetAnnotation ||
                    annotation.Name == SingleStoreAnnotationNames.CollationDelegation && hasCollationAnnotation)
                {
                    continue;
                }

                yield return annotation;
            }
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IModel model, IAnnotation annotation)
        {
            if (annotation.Name == SingleStoreAnnotationNames.CharSet)
            {
                var delegationModes = model[SingleStoreAnnotationNames.CharSetDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    _modelHasCharSetMethodInfo,
                    new[] {annotation.Value}
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == SingleStoreAnnotationNames.CharSetDelegation &&
                model[SingleStoreAnnotationNames.CharSet] is null)
            {
                return new MethodCallCodeFragment(
                    _modelHasCharSetMethodInfo,
                    null,
                    annotation.Value);
            }

            // EF Core currently just falls back on using the `Relational:Collation` annotation instead of generating the `UseCollation()`
            // method call (though it could), so we can return our method call fragment here, without generating an ugly duplicate.
            if (annotation.Name == RelationalAnnotationNames.Collation)
            {
                var delegationModes = model[SingleStoreAnnotationNames.CollationDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    _modelUseCollationMethodInfo,
                    new[] {annotation.Value}
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == SingleStoreAnnotationNames.CollationDelegation &&
                model[RelationalAnnotationNames.Collation] is null)
            {
                return new MethodCallCodeFragment(
                    _modelUseCollationMethodInfo,
                    null,
                    annotation.Value);
            }

            if (annotation.Name == SingleStoreAnnotationNames.GuidCollation)
            {
                return new MethodCallCodeFragment(
                    _modelUseGuidCollationMethodInfo,
                    annotation.Value);
            }

            return null;
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IEntityType entityType, IAnnotation annotation)
        {
            if (annotation.Name == SingleStoreAnnotationNames.CharSet)
            {
                var delegationModes = entityType[SingleStoreAnnotationNames.CharSetDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    _entityTypeHasCharSetMethodInfo,
                    new[] {annotation.Value}
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == SingleStoreAnnotationNames.CharSetDelegation &&
                entityType[SingleStoreAnnotationNames.CharSet] is null)
            {
                return new MethodCallCodeFragment(
                    _entityTypeHasCharSetMethodInfo,
                    null,
                    annotation.Value);
            }

            if (annotation.Name == RelationalAnnotationNames.Collation)
            {
                var delegationModes = entityType[SingleStoreAnnotationNames.CollationDelegation] as DelegationModes?;
                return new MethodCallCodeFragment(
                    _entityTypeUseCollationMethodInfo,
                    new[] {annotation.Value}
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == SingleStoreAnnotationNames.CollationDelegation &&
                entityType[RelationalAnnotationNames.Collation] is null)
            {
                return new MethodCallCodeFragment(
                    _entityTypeUseCollationMethodInfo,
                    null,
                    annotation.Value);
            }

            return null;
        }

        protected override AttributeCodeFragment GenerateDataAnnotation(IEntityType entityType, IAnnotation annotation)
        {
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(annotation, nameof(annotation));

            if (annotation.Name == SingleStoreAnnotationNames.CharSet)
            {
                var delegationModes = entityType[SingleStoreAnnotationNames.CharSetDelegation] as DelegationModes?;
                return new AttributeCodeFragment(
                    typeof(SingleStoreCharSetAttribute),
                    new[] {annotation.Value}
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == SingleStoreAnnotationNames.CharSetDelegation &&
                entityType[SingleStoreAnnotationNames.CharSet] is null)
            {
                return new AttributeCodeFragment(
                    typeof(SingleStoreCharSetAttribute),
                    null,
                    annotation.Value);
            }

            if (annotation.Name == RelationalAnnotationNames.Collation)
            {
                var delegationModes = entityType[SingleStoreAnnotationNames.CollationDelegation] as DelegationModes?;
                return new AttributeCodeFragment(
                    typeof(SingleStoreCollationAttribute),
                    new[] {annotation.Value}
                        .AppendIfTrue(delegationModes.HasValue, delegationModes)
                        .ToArray());
            }

            if (annotation.Name == SingleStoreAnnotationNames.CollationDelegation &&
                entityType[RelationalAnnotationNames.Collation] is null)
            {
                return new AttributeCodeFragment(
                    typeof(SingleStoreCollationAttribute),
                    null,
                    annotation.Value);
            }

            return base.GenerateDataAnnotation(entityType, annotation);
        }

        protected override MethodCallCodeFragment GenerateFluentApi(IProperty property, IAnnotation annotation)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(annotation, nameof(annotation));

            // At this point, all legacy `SingleStore:Collation` annotations should have been replaced by `Relational:Collation` ones.
#pragma warning disable 618
            Debug.Assert(annotation.Name != SingleStoreAnnotationNames.Collation);
#pragma warning restore 618

            switch (annotation.Name)
            {
                case SingleStoreAnnotationNames.CharSet when annotation.Value is string {Length: > 0} charSet:
                    return new MethodCallCodeFragment(
                        _propertyHasCharSetMethodInfo,
                        charSet);

                default:
                    return null;
            }
        }

        protected override AttributeCodeFragment GenerateDataAnnotation(IProperty property, IAnnotation annotation)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(annotation, nameof(annotation));

            return annotation.Name switch
            {
                SingleStoreAnnotationNames.CharSet when annotation.Value is string {Length: > 0} charSet => new AttributeCodeFragment(typeof(SingleStoreCharSetAttribute), charSet),
                RelationalAnnotationNames.Collation when annotation.Value is string {Length: > 0} collation => new AttributeCodeFragment(typeof(SingleStoreCollationAttribute), collation),
                _ => base.GenerateDataAnnotation(property, annotation)
            };
        }
    }
}
