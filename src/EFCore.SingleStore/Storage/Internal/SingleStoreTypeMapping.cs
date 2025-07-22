// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    // TODO: Use as base class for all type mappings.
    /// <summary>
    /// The base class for mapping SingleStore-specific types. It configures parameters with the
    /// <see cref="SingleStoreDbType"/> provider-specific type enum.
    /// </summary>
    public abstract class SingleStoreTypeMapping : RelationalTypeMapping
    {
        /// <summary>
        /// The database type used by SingleStore Distributed.
        /// </summary>
        public virtual SingleStoreDbType SingleStoreDbType { get; }

        // ReSharper disable once PublicConstructorInAbstractClass
        public SingleStoreTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            SingleStoreDbType mySqlDbType,
            DbType? dbType = null,
            bool unicode = false,
            int? size = null,
            ValueConverter valueConverter = null,
            ValueComparer valueComparer = null,
            JsonValueReaderWriter jsonValueReaderWriter = null)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        clrType,
                        valueConverter,
                        valueComparer,
                        jsonValueReaderWriter: jsonValueReaderWriter),
                    storeType,
                    StoreTypePostfix.None,
                    dbType,
                    unicode,
                    size))
            => SingleStoreDbType = mySqlDbType;

        /// <summary>
        /// Constructs an instance of the <see cref="SingleStoreTypeMapping"/> class.
        /// </summary>
        /// <param name="parameters">The parameters for this mapping.</param>
        /// <param name="mySqlDbType">The database type of the range subtype.</param>
        protected SingleStoreTypeMapping(RelationalTypeMappingParameters parameters, SingleStoreDbType mySqlDbType)
            : base(parameters)
            => SingleStoreDbType = mySqlDbType;

        protected override void ConfigureParameter(DbParameter parameter)
        {
            if (!(parameter is SingleStoreParameter mySqlParameter))
            {
                throw new ArgumentException($"SingleStore-specific type mapping {GetType()} being used with non-SingleStore parameter type {parameter.GetType().Name}");
            }

            base.ConfigureParameter(parameter);

            mySqlParameter.SingleStoreDbType = SingleStoreDbType;
        }
    }
}
