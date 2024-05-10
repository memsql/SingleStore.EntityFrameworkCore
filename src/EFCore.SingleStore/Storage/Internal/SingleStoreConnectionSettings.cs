// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Linq;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public class SingleStoreConnectionSettings
    {
        public SingleStoreConnectionSettings()
        {
        }

        public SingleStoreConnectionSettings(DbConnection connection)
            : this(connection.ConnectionString)
        {
        }

        public SingleStoreConnectionSettings(string connectionString)
        {
            var csb = new SingleStoreConnectionStringBuilder(connectionString);

            if (csb.GuidFormat == SingleStoreGuidFormat.Default)
            {
                GuidFormat = csb.OldGuids
                    ? SingleStoreGuidFormat.LittleEndianBinary16
                    : SingleStoreGuidFormat.Char36;
            }
            else
            {
                GuidFormat = csb.GuidFormat;
            }

            // It would be nice to have access to a public and currently non-existing
            // SingleStoreConnectionStringOption.TreatTinyAsBoolean.HasValue() method, so we can safely find out, whether
            // TreatTinyAsBoolean has been explicitly set or not.
            var treatTinyAsBooleanKeys = new[] {"Treat Tiny As Boolean", "TreatTinyAsBoolean"};
            TreatTinyAsBoolean = treatTinyAsBooleanKeys.Any(k => csb.ContainsKey(k))
                ? (bool?)csb.TreatTinyAsBoolean
                : null;
        }

        public virtual SingleStoreGuidFormat GuidFormat { get; }
        public virtual bool? TreatTinyAsBoolean { get; }

        protected virtual bool Equals(SingleStoreConnectionSettings other)
        {
            return GuidFormat == other.GuidFormat &&
                   TreatTinyAsBoolean == other.TreatTinyAsBoolean;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SingleStoreConnectionSettings)obj);
        }

        public override int GetHashCode()
            => HashCode.Combine(
                GuidFormat,
                TreatTinyAsBoolean);
    }
}
