// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Design.Internal;

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
}
