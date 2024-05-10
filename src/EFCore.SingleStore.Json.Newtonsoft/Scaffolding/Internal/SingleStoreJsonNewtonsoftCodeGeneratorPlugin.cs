// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Scaffolding.Internal
{
    public class SingleStoreJsonNewtonsoftCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        private static readonly MethodInfo _useNewtonsoftJsonMethodInfo =
            typeof(SingleStoreJsonNewtonsoftDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreJsonNewtonsoftDbContextOptionsBuilderExtensions.UseNewtonsoftJson),
                typeof(SingleStoreDbContextOptionsBuilder),
                typeof(SingleStoreCommonJsonChangeTrackingOptions));

        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(_useNewtonsoftJsonMethodInfo);
    }
}
