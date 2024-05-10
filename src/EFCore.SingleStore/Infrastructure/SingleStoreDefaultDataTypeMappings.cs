// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace EntityFrameworkCore.SingleStore.Infrastructure
{
    public class SingleStoreDefaultDataTypeMappings
    {
        public SingleStoreDefaultDataTypeMappings()
        {
        }

        protected SingleStoreDefaultDataTypeMappings(SingleStoreDefaultDataTypeMappings copyFrom)
        {
            ClrBoolean = copyFrom.ClrBoolean;
            ClrDateTime = copyFrom.ClrDateTime;
            ClrDateTimeOffset = copyFrom.ClrDateTimeOffset;
            ClrTimeSpan = copyFrom.ClrTimeSpan;
            ClrTimeOnlyPrecision = copyFrom.ClrTimeOnlyPrecision;
        }

        public virtual SingleStoreBooleanType ClrBoolean { get; private set; }
        public virtual SingleStoreDateTimeType ClrDateTime { get; private set; }
        public virtual SingleStoreDateTimeType ClrDateTimeOffset { get; private set; }
        public virtual SingleStoreTimeSpanType ClrTimeSpan { get; private set; }
        public virtual int ClrTimeOnlyPrecision { get; private set; } = -1;

        public virtual SingleStoreDefaultDataTypeMappings WithClrBoolean(SingleStoreBooleanType mysqlBooleanType)
        {
            var clone = Clone();
            clone.ClrBoolean = mysqlBooleanType;
            return clone;
        }

        public virtual SingleStoreDefaultDataTypeMappings WithClrDateTime(SingleStoreDateTimeType mysqlDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTime = mysqlDateTimeType;
            return clone;
        }

        public virtual SingleStoreDefaultDataTypeMappings WithClrDateTimeOffset(SingleStoreDateTimeType mysqlDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTimeOffset = mysqlDateTimeType;
            return clone;
        }

        // TODO: Remove Time6, add optional precision parameter for Time types.
        public virtual SingleStoreDefaultDataTypeMappings WithClrTimeSpan(SingleStoreTimeSpanType mysqlTimeSpanType)
        {
            var clone = Clone();
            clone.ClrTimeSpan = mysqlTimeSpanType;
            return clone;
        }

        /// <summary>
        /// Set the default precision for `TimeOnly` CLR type mapping to a MySQL TIME type.
        /// Set <paramref name="precision"/> to <see langword="null"/>, to use the highest supported precision.
        /// Otherwise, set <paramref name="precision"/> to a valid value between `0` and `6`.
        /// </summary>
        /// <param name="precision">The precision used for the MySQL TIME type.</param>
        /// <returns>The same instance, to allow chained method calls.</returns>
        public virtual SingleStoreDefaultDataTypeMappings WithClrTimeOnly(int? precision = null)
        {
            if (precision is < 0 or > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(precision));
            }

            var clone = Clone();
            clone.ClrTimeOnlyPrecision = precision ?? -1;
            return clone;
        }

        protected virtual SingleStoreDefaultDataTypeMappings Clone() => new SingleStoreDefaultDataTypeMappings(this);

        protected virtual bool Equals(SingleStoreDefaultDataTypeMappings other)
        {
            return ClrBoolean == other.ClrBoolean &&
                   ClrDateTime == other.ClrDateTime &&
                   ClrDateTimeOffset == other.ClrDateTimeOffset &&
                   ClrTimeSpan == other.ClrTimeSpan &&
                   ClrTimeOnlyPrecision == other.ClrTimeOnlyPrecision;
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

            return Equals((SingleStoreDefaultDataTypeMappings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ClrBoolean;
                hashCode = (hashCode * 397) ^ (int)ClrDateTime;
                hashCode = (hashCode * 397) ^ (int)ClrDateTimeOffset;
                hashCode = (hashCode * 397) ^ (int)ClrTimeSpan;
                return hashCode;
            }
        }
    }

    public enum SingleStoreBooleanType
    {
        /// <summary>
        /// TODO
        /// </summary>
        None = -1, // TODO: Remove in EF Core 5; see SingleStoreTypeMappingTest.Bool_with_SingleStoreBooleanType_None_maps_to_null()

        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        TinyInt1 = 1,

        /// <summary>
        /// TODO
        /// </summary>
        Bit1 = 2
    }

    public enum SingleStoreDateTimeType
    {
        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        DateTime = 1,

        /// <summary>
        /// TODO
        /// </summary>
        DateTime6 = 2,

        /// <summary>
        /// TODO
        /// </summary>
        Timestamp6 = 3,

        /// <summary>
        /// TODO
        /// </summary>
        Timestamp = 4,
    }

    public enum SingleStoreTimeSpanType
    {
        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        Time = 1,

        /// <summary>
        /// TODO
        /// </summary>
        Time6 = 2,
    }
}
