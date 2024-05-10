// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Query.Internal;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using EntityFrameworkCore.SingleStore.Query.ExpressionTranslators.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     EntityFrameworkCore.SingleStore.Json.Microsoft extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class SingleStoreJsonMicrosoftServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the services required for Microsoft JSON support (System.Text.Json) in Pomelo's MySQL provider for Entity Framework Core.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns> The same service collection so that multiple calls can be chained. </returns>
        public static IServiceCollection AddEntityFrameworkSingleStoreJsonMicrosoft(
            [NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<IRelationalTypeMappingSourcePlugin, SingleStoreJsonMicrosoftTypeMappingSourcePlugin>()
                .TryAdd<IMethodCallTranslatorPlugin, SingleStoreJsonMicrosoftMethodCallTranslatorPlugin>()
                .TryAdd<IMemberTranslatorPlugin, SingleStoreJsonMicrosoftMemberTranslatorPlugin>()
                .TryAddProviderSpecificServices(
                    x => x.TryAddScopedEnumerable<ISingleStoreJsonPocoTranslator, SingleStoreJsonMicrosoftPocoTranslator>());

            return serviceCollection;
        }
    }
}
