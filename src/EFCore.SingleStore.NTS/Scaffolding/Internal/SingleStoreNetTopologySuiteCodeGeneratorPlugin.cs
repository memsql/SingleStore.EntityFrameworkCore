// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace EntityFrameworkCore.SingleStore.Scaffolding.Internal
{
    public class SingleStoreNetTopologySuiteCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
    {
        private static readonly MethodInfo _useNetTopologySuiteMethodInfo =
            typeof(SingleStoreNetTopologySuiteDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
                nameof(SingleStoreNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite),
                typeof(SingleStoreDbContextOptionsBuilder));

        public override MethodCallCodeFragment GenerateProviderOptions()
            => new MethodCallCodeFragment(_useNetTopologySuiteMethodInfo);
    }
}
