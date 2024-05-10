// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Infrastructure.Internal
{
    public class SingleStoreJsonMicrosoftOptionsExtension : SingleStoreJsonOptionsExtension
    {
        public SingleStoreJsonMicrosoftOptionsExtension()
        {
        }

        public SingleStoreJsonMicrosoftOptionsExtension([NotNull] SingleStoreJsonOptionsExtension copyFrom)
            : base(copyFrom)
        {
        }

        protected override SingleStoreJsonOptionsExtension Clone()
            => new SingleStoreJsonMicrosoftOptionsExtension(this);

        public override string UseJsonOptionName => nameof(SingleStoreJsonMicrosoftDbContextOptionsBuilderExtensions.UseMicrosoftJson);
        public override string AddEntityFrameworkName => nameof(SingleStoreJsonMicrosoftServiceCollectionExtensions.AddEntityFrameworkSingleStoreJsonMicrosoft);
        public override Type TypeMappingSourcePluginType => typeof(SingleStoreJsonMicrosoftTypeMappingSourcePlugin);

        /// <summary>
        ///     Adds the services required to make the selected options work. This is used when there
        ///     is no external <see cref="IServiceProvider" /> and EF is maintaining its own service
        ///     provider internally. This allows database providers (and other extensions) to register their
        ///     required services when EF is creating an service provider.
        /// </summary>
        /// <param name="services"> The collection to add services to. </param>
        public override void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkSingleStoreJsonMicrosoft();
    }
}
