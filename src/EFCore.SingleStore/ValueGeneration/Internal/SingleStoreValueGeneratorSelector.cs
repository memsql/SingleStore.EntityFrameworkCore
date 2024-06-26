﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EntityFrameworkCore.SingleStore.ValueGeneration.Internal
{
    public class SingleStoreValueGeneratorSelector : RelationalValueGeneratorSelector
    {
        private readonly ISingleStoreOptions _options;

        public SingleStoreValueGeneratorSelector(
            [NotNull] ValueGeneratorSelectorDependencies dependencies,
            [NotNull] ISingleStoreOptions options)
            : base(dependencies)
        {
            _options = options;
        }


        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            var ret = property.ClrType.UnwrapNullableType() == typeof(Guid)
                ? property.ValueGenerated == ValueGenerated.Never
                  || property.GetDefaultValueSql() != null
                    ? (ValueGenerator)new TemporaryGuidValueGenerator()
                    : new SingleStoreSequentialGuidValueGenerator(_options)
                : base.Create(property, entityType);
            return ret;
        }
    }
}
