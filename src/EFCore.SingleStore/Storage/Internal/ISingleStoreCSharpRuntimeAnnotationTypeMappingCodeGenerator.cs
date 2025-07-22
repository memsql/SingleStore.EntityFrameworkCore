// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Design.Internal;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public interface ISingleStoreCSharpRuntimeAnnotationTypeMappingCodeGenerator
{
    void Create(
        CSharpRuntimeAnnotationCodeGeneratorParameters codeGeneratorParameters,
        CSharpRuntimeAnnotationCodeGeneratorDependencies codeGeneratorDependencies);
}
