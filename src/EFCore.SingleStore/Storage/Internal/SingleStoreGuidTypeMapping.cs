// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Storage;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Utilities;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public class SingleStoreGuidTypeMapping : GuidTypeMapping, IJsonSpecificTypeMapping
    {
        private readonly SingleStoreGuidFormat _guidFormat;

        public SingleStoreGuidTypeMapping(SingleStoreGuidFormat guidFormat)
            : this(new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(Guid)),
                    GetStoreType(guidFormat),
                    StoreTypePostfix.Size,
                    System.Data.DbType.Guid,
                    false,
                    GetSize(guidFormat),
                    true),
                guidFormat)
        {
        }

        protected SingleStoreGuidTypeMapping(RelationalTypeMappingParameters parameters, SingleStoreGuidFormat guidFormat)
            : base(parameters)
        {
            _guidFormat = guidFormat;
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreGuidTypeMapping(parameters, _guidFormat);

        public virtual bool IsCharBasedStoreType
            => GetStoreType(_guidFormat) == "char";

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            switch (_guidFormat)
            {
                case SingleStoreGuidFormat.Char36:
                    return $"'{value:D}'";

                case SingleStoreGuidFormat.Char32:
                    return $"'{value:N}'";

                case SingleStoreGuidFormat.Binary16:
                case SingleStoreGuidFormat.TimeSwapBinary16:
                case SingleStoreGuidFormat.LittleEndianBinary16:
                    return ByteArrayFormatter.ToHex(GetBytesFromGuid(_guidFormat, (Guid)value));

                case SingleStoreGuidFormat.None:
                case SingleStoreGuidFormat.Default:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetStoreType(SingleStoreGuidFormat guidFormat)
        {
            switch (guidFormat)
            {
                case SingleStoreGuidFormat.Char36:
                case SingleStoreGuidFormat.Char32:
                    return "char";

                case SingleStoreGuidFormat.Binary16:
                case SingleStoreGuidFormat.TimeSwapBinary16:
                case SingleStoreGuidFormat.LittleEndianBinary16:
                    return "binary";

                case SingleStoreGuidFormat.None:
                case SingleStoreGuidFormat.Default:
                default:
                    throw new InvalidOperationException();
            }
        }

        private static int GetSize(SingleStoreGuidFormat guidFormat)
        {
            switch (guidFormat)
            {
                case SingleStoreGuidFormat.Char36:
                    return 36;

                case SingleStoreGuidFormat.Char32:
                    return 32;

                case SingleStoreGuidFormat.Binary16:
                case SingleStoreGuidFormat.TimeSwapBinary16:
                case SingleStoreGuidFormat.LittleEndianBinary16:
                    return 16;

                case SingleStoreGuidFormat.None:
                case SingleStoreGuidFormat.Default:
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool IsValidGuidFormat(SingleStoreGuidFormat guidFormat)
            => guidFormat != SingleStoreGuidFormat.None &&
               guidFormat != SingleStoreGuidFormat.Default;

        protected static byte[] GetBytesFromGuid(SingleStoreGuidFormat guidFormat, Guid guid)
        {
            var bytes = guid.ToByteArray();

            if (guidFormat == SingleStoreGuidFormat.Binary16)
            {
                return new[] { bytes[3], bytes[2], bytes[1], bytes[0], bytes[5], bytes[4], bytes[7], bytes[6], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15] };
            }

            if (guidFormat == SingleStoreGuidFormat.TimeSwapBinary16)
            {
                return new[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15] };
            }

            return bytes;
        }

        /// <summary>
        /// For JSON values, we will always use the 36 character string representation.
        /// </summary>
        public virtual RelationalTypeMapping CloneAsJsonCompatible()
            => new SingleStoreGuidTypeMapping(SingleStoreGuidFormat.Char36);
    }
}
