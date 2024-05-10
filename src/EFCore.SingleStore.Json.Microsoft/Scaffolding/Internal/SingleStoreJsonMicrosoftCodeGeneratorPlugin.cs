// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Scaffolding.Internal
{
    public class SingleStoreJsonMicrosoftCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        private static readonly MethodInfo _useMicrosoftJsonMethodInfo =
            typeof(SingleStoreJsonMicrosoftDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreJsonMicrosoftDbContextOptionsBuilderExtensions.UseMicrosoftJson),
                typeof(SingleStoreDbContextOptionsBuilder),
                typeof(SingleStoreCommonJsonChangeTrackingOptions));

        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(_useMicrosoftJsonMethodInfo);
    }
}
