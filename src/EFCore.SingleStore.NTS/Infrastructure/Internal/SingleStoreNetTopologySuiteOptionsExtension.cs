// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SingleStore.Infrastructure.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SingleStoreNetTopologySuiteOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkSingleStoreNetTopologySuite();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
            var internalServiceProvider = options.FindExtension<CoreOptionsExtension>()?.InternalServiceProvider;
            if (internalServiceProvider != null)
            {
                using (var scope = internalServiceProvider.CreateScope())
                {
                    var plugins = scope.ServiceProvider.GetService<IEnumerable<IRelationalTypeMappingSourcePlugin>>();
                    if (plugins?.Any(s => s is SingleStoreNetTopologySuiteTypeMappingSourcePlugin) != true)
                    {
                        throw new InvalidOperationException($"{nameof(SingleStoreNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite)} requires {nameof(SingleStoreNetTopologySuiteServiceCollectionExtensions.AddEntityFrameworkSingleStoreNetTopologySuite)} to be called on the internal service provider used.");
                    }
                }
            }
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new SingleStoreNetTopologySuiteOptionsExtension Extension
                => (SingleStoreNetTopologySuiteOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override int GetServiceProviderHashCode() => 0;

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
                => other is ExtensionInfo;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
                => debugInfo["SingleStore:" + nameof(SingleStoreNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite)] = "1";

            public override string LogFragment => "using NetTopologySuite ";
        }
    }
}
