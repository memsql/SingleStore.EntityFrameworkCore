// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestProviderCodeGenerator : ProviderCodeGenerator
    {
        private static readonly MethodInfo _useSingleStoreMethodInfo = typeof(SingleStoreDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(SingleStoreDbContextOptionsBuilderExtensions.UseSingleStore),
            typeof(string),
            typeof(Action<SingleStoreDbContextOptionsBuilder>));

        public TestProviderCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override MethodCallCodeFragment GenerateUseProvider(
            string connectionString,
            MethodCallCodeFragment providerOptions)
            => new MethodCallCodeFragment(
                _useSingleStoreMethodInfo,
                providerOptions == null
                    ? new object[] { connectionString }
                    : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
    }
}
