// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public class SingleStoreGuidTypeMapping : GuidTypeMapping, IJsonSpecificTypeMapping, ISingleStoreCSharpRuntimeAnnotationTypeMappingCodeGenerator
    {
        public static new SingleStoreGuidTypeMapping Default { get; } = new(SingleStoreGuidFormat.Char36);

        public virtual SingleStoreGuidFormat GuidFormat { get; }

        public SingleStoreGuidTypeMapping(SingleStoreGuidFormat guidFormat)
            : this(new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        typeof(Guid),
                        jsonValueReaderWriter: JsonGuidReaderWriter.Instance),
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
            GuidFormat = guidFormat;
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreGuidTypeMapping(parameters, GuidFormat);

        public virtual RelationalTypeMapping Clone(SingleStoreGuidFormat guidFormat)
            => new SingleStoreGuidTypeMapping(Parameters, guidFormat);

        public virtual bool IsCharBasedStoreType
            => GetStoreType(GuidFormat) == "char";

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            switch (GuidFormat)
            {
                case SingleStoreGuidFormat.Char36:
                    return $"'{value:D}'";

                case SingleStoreGuidFormat.Char32:
                    return $"'{value:N}'";

                case SingleStoreGuidFormat.Binary16:
                case SingleStoreGuidFormat.TimeSwapBinary16:
                case SingleStoreGuidFormat.LittleEndianBinary16:
                    return "0x" + Convert.ToHexString(GetBytesFromGuid(GuidFormat, (Guid)value));

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

        void ISingleStoreCSharpRuntimeAnnotationTypeMappingCodeGenerator.Create(
            CSharpRuntimeAnnotationCodeGeneratorParameters codeGeneratorParameters,
            CSharpRuntimeAnnotationCodeGeneratorDependencies codeGeneratorDependencies)
        {
            var defaultTypeMapping = Default;
            if (defaultTypeMapping == this)
            {
                return;
            }

            var code = codeGeneratorDependencies.CSharpHelper;

            var cloneParameters = new List<string>();

            if (GuidFormat != defaultTypeMapping.GuidFormat)
            {
                cloneParameters.Add($"guidFormat: {code.Literal(GuidFormat, true)}");
            }

            if (cloneParameters.Any())
            {
                var mainBuilder = codeGeneratorParameters.MainBuilder;

                mainBuilder.AppendLine(";");

                mainBuilder
                    .AppendLine($"{codeGeneratorParameters.TargetName}.TypeMapping = (({code.Reference(GetType())}){codeGeneratorParameters.TargetName}.TypeMapping).Clone(")
                    .IncrementIndent();

                for (var i = 0; i < cloneParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        mainBuilder.AppendLine(",");
                    }

                    mainBuilder.Append(cloneParameters[i]);
                }

                mainBuilder
                    .Append(")")
                    .DecrementIndent();
            }
        }
    }
}
