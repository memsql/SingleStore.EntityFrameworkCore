// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using EntityFrameworkCore.SingleStore.Storage.Internal.Json;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SingleStoreByteArrayTypeMapping : ByteArrayTypeMapping
    {
        private const int MaxSize = 8000;

        private readonly int _maxSpecificSize;

        public static new SingleStoreByteArrayTypeMapping Default { get; } = new();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SingleStoreByteArrayTypeMapping(
            string storeType = null,
            int? size = null,
            bool fixedLength = false)
            : this(System.Data.DbType.Binary,
                storeType,
                size.HasValue && size < MaxSize ? size : null,
                fixedLength)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected SingleStoreByteArrayTypeMapping(
            DbType type,
            string storeType,
            int? size,
            bool fixedLength)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        typeof(byte[]),
                        jsonValueReaderWriter: JsonByteArrayReaderWriter.Instance /* SingleStoreJsonByteArrayAsHexStringReaderWriter.Instance */),
                    storeType ?? GetBaseType(size, fixedLength),
                    GetStoreTypePostfix(size),
                    type,
                    size: size,
                    fixedLength: fixedLength))
        {
        }

        private static string GetBaseType(int? size, bool isFixedLength)
            => size == null
                ? "longblob"
                : isFixedLength ? "binary" : "varbinary";

        private static StoreTypePostfix GetStoreTypePostfix(int? size)
            => size != null && size <= MaxSize ? StoreTypePostfix.Size : StoreTypePostfix.None;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected SingleStoreByteArrayTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
            _maxSpecificSize = CalculateSize(parameters.Size);
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreByteArrayTypeMapping(parameters);

        private static int CalculateSize(int? size)
            => size.HasValue && size < MaxSize ? size.Value : MaxSize;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            // For strings and byte arrays, set the max length to the size facet if specified, or
            // 8000 bytes if no size facet specified, if the data will fit so as to avoid query cache
            // fragmentation by setting lots of different Size values otherwise always set to
            // -1 (unbounded) to avoid SQL client size inference.

            var value = parameter.Value;
            var length = (value as string)?.Length ?? (value as byte[])?.Length;

            parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
                ? _maxSpecificSize
                : -1;
        }

        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral(object value)
            => value is byte[] { Length: > 0 } byteArray
                ? "0x" + Convert.ToHexString(byteArray)
                : "X''";
    }
}
