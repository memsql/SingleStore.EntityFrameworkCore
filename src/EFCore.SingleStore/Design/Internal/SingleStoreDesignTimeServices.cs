// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using EntityFrameworkCore.SingleStore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.SingleStore.Design.Internal
{
    public class SingleStoreDesignTimeServices : IDesignTimeServices
    {
        public virtual void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSingleStore();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                .TryAdd<IAnnotationCodeGenerator, SingleStoreAnnotationCodeGenerator>()
                .TryAdd<IDatabaseModelFactory, SingleStoreDatabaseModelFactory>()
                .TryAdd<IProviderConfigurationCodeGenerator, SingleStoreCodeGenerator>()
                .TryAddCoreServices();
        }
    }
}
