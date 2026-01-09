// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using EntityFrameworkCore.SingleStore.Metadata.Internal;

namespace EntityFrameworkCore.SingleStore.Metadata.Conventions;

/// <summary>
///     A convention that creates an optimized copy of the mutable model.
///     The runtime model is only used at app runtime and not for design-time purposes.
///     Therefore, all annotations that are related to design-time concerns (i.e. databases, tables or columns) are superfluous and should
///     be removed.
/// </summary>
public class SingleStoreRuntimeModelConvention : RelationalRuntimeModelConvention
{
    /// <summary>
    ///     Creates a new instance of <see cref="SingleStoreRuntimeModelConvention" />.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
    /// <param name="relationalDependencies">Parameter object containing relational dependencies for this convention.</param>
    public SingleStoreRuntimeModelConvention(
        ProviderConventionSetBuilderDependencies dependencies,
        RelationalConventionSetBuilderDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <inheritdoc />
    protected override void ProcessModelAnnotations(
        Dictionary<string, object> annotations,
        IModel model,
        RuntimeModel runtimeModel,
        bool runtime)
    {
        base.ProcessModelAnnotations(annotations, model, runtimeModel, runtime);

        if (!runtime)
        {
            annotations.Remove(SingleStoreAnnotationNames.CharSet);
            annotations.Remove(SingleStoreAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.CollationDelegation);
            annotations.Remove(SingleStoreAnnotationNames.GuidCollation);
        }
    }

    /// <inheritdoc />
    protected override void ProcessEntityTypeAnnotations(
        Dictionary<string, object> annotations,
        IEntityType entityType,
        RuntimeEntityType runtimeEntityType,
        bool runtime)
    {
        base.ProcessEntityTypeAnnotations(annotations, entityType, runtimeEntityType, runtime);

        if (!runtime)
        {
            annotations.Remove(SingleStoreAnnotationNames.CharSet);
            annotations.Remove(SingleStoreAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.CollationDelegation);
            annotations.Remove(SingleStoreAnnotationNames.StoreOptions);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }
    }

    /// <inheritdoc />
    protected override void ProcessPropertyAnnotations(
        Dictionary<string, object> annotations,
        IProperty property,
        RuntimeProperty runtimeProperty,
        bool runtime)
    {
        base.ProcessPropertyAnnotations(annotations, property, runtimeProperty, runtime);

        if (!runtime)
        {
            annotations.Remove(SingleStoreAnnotationNames.CharSet);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.SpatialReferenceSystemId);

            if (!annotations.ContainsKey(SingleStoreAnnotationNames.ValueGenerationStrategy))
            {
                annotations[SingleStoreAnnotationNames.ValueGenerationStrategy] = property.GetValueGenerationStrategy();
            }
        }
    }

    /// <inheritdoc />
    protected override void ProcessIndexAnnotations(
        Dictionary<string, object> annotations,
        IIndex index,
        RuntimeIndex runtimeIndex,
        bool runtime)
    {
        base.ProcessIndexAnnotations(annotations, index, runtimeIndex, runtime);

        if (!runtime)
        {
            annotations.Remove(SingleStoreAnnotationNames.FullTextIndex);
            annotations.Remove(SingleStoreAnnotationNames.FullTextParser);
            annotations.Remove(SingleStoreAnnotationNames.IndexPrefixLength);
            annotations.Remove(SingleStoreAnnotationNames.SpatialIndex);
        }
    }

    /// <inheritdoc />
    protected override void ProcessKeyAnnotations(
        Dictionary<string, object> annotations,
        IKey key,
        RuntimeKey runtimeKey,
        bool runtime)
    {
        base.ProcessKeyAnnotations(annotations, key, runtimeKey, runtime);

        if (!runtime)
        {
            annotations.Remove(SingleStoreAnnotationNames.IndexPrefixLength);
        }
    }
}
