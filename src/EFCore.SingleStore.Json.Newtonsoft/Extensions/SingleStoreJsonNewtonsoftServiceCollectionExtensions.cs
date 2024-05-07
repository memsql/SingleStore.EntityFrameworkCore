// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Query.Internal;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     EntityFrameworkCore.SingleStore.Json.Newtonsoft extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class SingleStoreJsonNewtonsoftServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the services required for JSON.NET support (Newtonsoft.Json) in Pomelo's MySQL provider for Entity Framework Core.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddEntityFrameworkSingleStoreJsonNewtonsoft(
            [NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<IRelationalTypeMappingSourcePlugin, SingleStoreJsonNewtonsoftTypeMappingSourcePlugin>()
                .TryAdd<IMethodCallTranslatorPlugin, SingleStoreJsonNewtonsoftMethodCallTranslatorPlugin>()
                .TryAdd<IMemberTranslatorPlugin, SingleStoreJsonNewtonsoftMemberTranslatorPlugin>()
                .TryAddProviderSpecificServices(
                    x => x.TryAddScopedEnumerable<ISingleStoreJsonPocoTranslator, SingleStoreJsonNewtonsoftPocoTranslator>());

            return serviceCollection;
        }
    }
}
