// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using EntityFrameworkCore.SingleStore.Metadata.Internal;

namespace EntityFrameworkCore.SingleStore.Internal
{
    public static class SingleStoreValueGenerationStrategyCompatibility
    {
        public static SingleStoreValueGenerationStrategy? GetValueGenerationStrategy(IAnnotation[] annotations)
        {
            var valueGenerationStrategy = ObjectToEnumConverter.GetEnumValue<SingleStoreValueGenerationStrategy>(
                annotations.FirstOrDefault(a => a.Name == SingleStoreAnnotationNames.ValueGenerationStrategy)?.Value);

            if (!valueGenerationStrategy.HasValue ||
                valueGenerationStrategy == SingleStoreValueGenerationStrategy.None)
            {
                var generatedOnAddAnnotation = annotations.FirstOrDefault(a => a.Name == SingleStoreAnnotationNames.LegacyValueGeneratedOnAdd)?.Value;
                if (generatedOnAddAnnotation != null && (bool)generatedOnAddAnnotation)
                {
                    valueGenerationStrategy = SingleStoreValueGenerationStrategy.IdentityColumn;
                }

                var generatedOnAddOrUpdateAnnotation = annotations.FirstOrDefault(a => a.Name == SingleStoreAnnotationNames.LegacyValueGeneratedOnAddOrUpdate)?.Value;
                if (generatedOnAddOrUpdateAnnotation != null && (bool)generatedOnAddOrUpdateAnnotation)
                {
                    valueGenerationStrategy = SingleStoreValueGenerationStrategy.ComputedColumn;
                }
            }

            return valueGenerationStrategy;
        }
    }
}
