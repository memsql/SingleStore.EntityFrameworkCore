// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Metadata.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Design.Internal;

// Used to generate a compiled model. The compiled model is only used at app runtime and not for design-time purposes.
// Therefore, all annotations that are related to design-time concerns (i.e. databases, tables or columns) are superfluous and should be
// removed.
// TOOD: Check behavior for `ValueGenerationStrategy`, `LegacyValueGeneratedOnAdd` and `LegacyValueGeneratedOnAddOrUpdate`.
public class SingleStoreCSharpRuntimeAnnotationCodeGenerator : RelationalCSharpRuntimeAnnotationCodeGenerator
{
    public SingleStoreCSharpRuntimeAnnotationCodeGenerator(
        CSharpRuntimeAnnotationCodeGeneratorDependencies dependencies,
        RelationalCSharpRuntimeAnnotationCodeGeneratorDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    public override bool Create(
        CoreTypeMapping typeMapping,
        CSharpRuntimeAnnotationCodeGeneratorParameters parameters,
        ValueComparer valueComparer = null,
        ValueComparer keyValueComparer = null,
        ValueComparer providerValueComparer = null)
    {
        var result = base.Create(typeMapping, parameters, valueComparer, keyValueComparer, providerValueComparer);

        if (typeMapping is ISingleStoreCSharpRuntimeAnnotationTypeMappingCodeGenerator extension)
        {
            extension.Create(parameters, Dependencies);
        }

        return result;
    }

    public override void Generate(IModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.CharSet);
            annotations.Remove(SingleStoreAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.CollationDelegation);
            annotations.Remove(SingleStoreAnnotationNames.GuidCollation);
        }

        base.Generate(model, parameters);
    }

    public override void Generate(IRelationalModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.CharSet);
            annotations.Remove(SingleStoreAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.CollationDelegation);
            annotations.Remove(SingleStoreAnnotationNames.GuidCollation);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(model, parameters);
    }

    public override void Generate(IEntityType entityType, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.CharSet);
            annotations.Remove(SingleStoreAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.CollationDelegation);
            annotations.Remove(SingleStoreAnnotationNames.StoreOptions);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(entityType, parameters);
    }

    public override void Generate(ITable table, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.CharSet);
            annotations.Remove(SingleStoreAnnotationNames.CharSetDelegation);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.CollationDelegation);
            annotations.Remove(SingleStoreAnnotationNames.StoreOptions);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(table, parameters);
    }

    public override void Generate(IProperty property, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.CharSet);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.SpatialReferenceSystemId);

            annotations.Remove(RelationalAnnotationNames.Collation);

            if (!annotations.ContainsKey(SingleStoreAnnotationNames.ValueGenerationStrategy))
            {
                annotations[SingleStoreAnnotationNames.ValueGenerationStrategy] = property.GetValueGenerationStrategy();
            }
        }

        base.Generate(property, parameters);
    }

    public override void Generate(IColumn column, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.CharSet);
#pragma warning disable CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.Collation);
#pragma warning restore CS0618 // Type or member is obsolete
            annotations.Remove(SingleStoreAnnotationNames.SpatialReferenceSystemId);
            annotations.Remove(SingleStoreAnnotationNames.ValueGenerationStrategy);

            annotations.Remove(RelationalAnnotationNames.Collation);
        }

        base.Generate(column, parameters);
    }

    public override void Generate(IIndex index, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.FullTextIndex);
            annotations.Remove(SingleStoreAnnotationNames.FullTextParser);
            annotations.Remove(SingleStoreAnnotationNames.IndexPrefixLength);
            annotations.Remove(SingleStoreAnnotationNames.SpatialIndex);
        }

        base.Generate(index, parameters);
    }

    public override void Generate(ITableIndex index, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.FullTextIndex);
            annotations.Remove(SingleStoreAnnotationNames.FullTextParser);
            annotations.Remove(SingleStoreAnnotationNames.IndexPrefixLength);
            annotations.Remove(SingleStoreAnnotationNames.SpatialIndex);
        }

        base.Generate(index, parameters);
    }

    public override void Generate(IKey key, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(SingleStoreAnnotationNames.IndexPrefixLength);
        }

        base.Generate(key, parameters);
    }
}
