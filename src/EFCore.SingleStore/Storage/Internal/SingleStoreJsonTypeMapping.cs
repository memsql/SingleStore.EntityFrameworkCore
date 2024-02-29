// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public class SingleStoreJsonTypeMapping<T> : SingleStoreJsonTypeMapping
    {
        public SingleStoreJsonTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            [NotNull] ISingleStoreOptions options)
            : base(
                storeType,
                typeof(T),
                valueConverter,
                valueComparer,
                options)
        {
        }

        protected SingleStoreJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            SingleStoreDbType mySqlDbType,
            ISingleStoreOptions options)
            : base(parameters, mySqlDbType, options)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreJsonTypeMapping<T>(parameters, SingleStoreDbType, Options);
    }

    public abstract class SingleStoreJsonTypeMapping : SingleStoreStringTypeMapping
    {
        [NotNull]
        protected virtual ISingleStoreOptions Options { get; }

        public SingleStoreJsonTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            [NotNull] ISingleStoreOptions options)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        clrType,
                        valueConverter,
                        valueComparer),
                    storeType,
                    unicode: true),
                SingleStoreDbType.JSON,
                options,
                false,
                false)
        {
            if (storeType != "json")
            {
                throw new ArgumentException($"The store type '{nameof(storeType)}' must be 'json'.", nameof(storeType));
            }

            Options = options;
        }

        protected SingleStoreJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            SingleStoreDbType mySqlDbType,
            ISingleStoreOptions options)
            : base(parameters, mySqlDbType, options, false, false)
        {
            Options = options;
        }

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            // SingleStoreConnector does not know how to handle our custom SingleStoreJsonString type, that could be used when a
            // string parameter is explicitly cast to it.
            if (parameter.Value is SingleStoreJsonString mySqlJsonString)
            {
                parameter.Value = (string)mySqlJsonString;
            }
        }
    }
}
