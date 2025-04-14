// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using EntityFrameworkCore.SingleStore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Represents a <see cref="ServerVersion"/> for MySQL database servers.
    /// For MariaDB database servers, use <see cref="MariaDbServerVersion"/> instead.
    /// </summary>
    public class SingleStoreServerVersion : ServerVersion
    {
        public static readonly string SingleStoreTypeIdentifier = nameof(ServerType.SingleStore).ToLowerInvariant();
        public static readonly ServerVersion LatestSupportedServerVersion = new SingleStoreServerVersion(new Version(8, 9, 0));

        public override ServerVersionSupport Supports { get; }

        public SingleStoreServerVersion(Version version)
            : base(version, ServerType.SingleStore)
        {
            Supports = new SingleStoreServerVersionSupport(this);
        }

        public SingleStoreServerVersion(string versionString)
            : this(Parse(versionString, ServerType.SingleStore))
        {
        }

        public SingleStoreServerVersion(ServerVersion serverVersion)
            : base(serverVersion.Version, serverVersion.Type, serverVersion.TypeIdentifier)
        {
            if (Type != ServerType.SingleStore ||
                !string.Equals(TypeIdentifier, SingleStoreTypeIdentifier))
            {
                throw new ArgumentException($"{nameof(SingleStoreServerVersion)} is not compatible with the supplied server type.");
            }

            Supports = new SingleStoreServerVersionSupport(this);
        }

        public class SingleStoreServerVersionSupport : ServerVersionSupport
        {
            internal SingleStoreServerVersionSupport([NotNull] ServerVersion serverVersion)
                : base(serverVersion)
            {
            }

            public override bool DateTimeCurrentTimestamp => true;
            public override bool DateTime6 => true;
            public override bool LargerKeyLength => true;
            public override bool RenameIndex => false;
            public override bool RenameColumn => true;
            public override bool WindowFunctions => true;
            public override bool FloatCast => false; // https://docs.singlestore.com/managed-service/en/reference/sql-reference/conditional-functions/cast-or-convert.html
            public override bool DoubleCast => false;
            public override bool OuterApply => false;
            public override bool CrossApply => false;
            public override bool OuterReferenceInMultiLevelSubquery => false;
            public override bool Json => false; // TODO: PLAT-6409
            public override bool GeneratedColumns => true;
            public override bool NullableGeneratedColumns => true;
            public override bool ParenthesisEnclosedGeneratedColumnExpressions => GeneratedColumns;
            public override bool DefaultCharSetUtf8Mb4 => ServerVersion.Version >= new Version(8, 7, 0);
            public override bool DefaultExpression => true;
            public override bool AlternativeDefaultExpression => false;
            public override bool Sequences => false;
            public override bool SpatialIndexes => false; // TODO: PLAT-6286
            public override bool SpatialReferenceSystemRestrictedColumns => false;
            public override bool SpatialFunctionAdditions => false;
            public override bool SpatialSupportFunctionAdditions => false;
            public override bool SpatialSetSridFunction => false;
            public override bool SpatialDistanceFunctionImplementsAndoyer => false;
            public override bool SpatialDistanceSphereFunction => false;
            public override bool SpatialGeographic => false;
            public override bool ExceptIntercept => false;
            public override bool ExceptInterceptPrecedence => false;
            public override bool JsonDataTypeEmulation => false;
            public override bool ImplicitBoolCheckUsesIndex => false;
            public override bool SingleStoreBug96947Workaround => false;
            public override bool SingleStoreBug104294Workaround => false;
            public override bool FullTextParser => false;
            public override bool InformationSchemaCheckConstraintsTable => false;
            public override bool SingleStoreBugLimit0Offset0ExistsWorkaround => true;
            public override bool TextTypeSizeIsInCharacters => ServerVersion.Version >= new Version(7, 8, 0);
            public override bool ConnectionAttributes => ServerVersion.Version >= new Version(8, 1, 0);
            public override bool DescendingIndexes => false;
            public override bool CommonTableExpressions => false;
            public override bool LimitWithinInAllAnySomeSubquery => false;
            public override bool LimitWithNonConstantValue => false;
            public override bool JsonTable => false;
            public override bool JsonValue => false;
            public override bool Values => false;
            public override bool ValuesWithRows => false;
            public override bool OffsetReferencesOuterQuery => false;

            public override bool JsonTableImplementationStable => false;
            public override bool JsonTableImplementationWithoutMySqlBugs => false; // Other non-fatal bugs regarding JSON_TABLE.
            public override bool JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash => false; // MySQL non-deterministically crashes when using a parameter with JSON as the source of a JSON_TABLE call.
        }
    }
}
