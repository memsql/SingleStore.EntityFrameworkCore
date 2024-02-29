// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Scaffolding.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SingleStoreCodeGenerator : ProviderCodeGenerator
    {
        private static readonly MethodInfo _useSingleStoreMethodInfo = typeof(SingleStoreDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(SingleStoreDbContextOptionsBuilderExtensions.UseSingleStore),
            typeof(DbContextOptionsBuilder),
            typeof(string),
            typeof(Action<SingleStoreDbContextOptionsBuilder>));

        private readonly ISingleStoreOptions _options;

        public SingleStoreCodeGenerator(
            [NotNull] ProviderCodeGeneratorDependencies dependencies,
            ISingleStoreOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateUseProvider(
            string connectionString,
            MethodCallCodeFragment providerOptions)
        {
            // Strip scaffolding specific connection string options first.
            connectionString = new SingleStoreScaffoldingConnectionSettings(connectionString).GetProviderCompatibleConnectionString();

            return new MethodCallCodeFragment(
                _useSingleStoreMethodInfo,
                providerOptions == null
                    ? new object[] { connectionString, new SingleStoreCodeGenerationServerVersionCreation(_options.ServerVersion) }
                    : new object[] { connectionString, new SingleStoreCodeGenerationServerVersionCreation(_options.ServerVersion), new NestedClosureCodeFragment("x", providerOptions) });
        }
    }
}
