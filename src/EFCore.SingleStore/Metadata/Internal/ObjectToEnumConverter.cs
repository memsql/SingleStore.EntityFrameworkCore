// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace EntityFrameworkCore.SingleStore.Metadata.Internal
{
    public static class ObjectToEnumConverter
    {
        /// <summary>
        /// Can be used to allow substitution of enum values with their underlying type in annotations, so that multi-provider models can
        /// be setup without provider specific dependencies.
        /// </summary>
        /// <remarks>
        /// See https://github.com/PomeloFoundation/EntityFrameworkCore.MySql/issues/1205 for further information.
        /// </remarks>
        public static T? GetEnumValue<T>(object value)
            where T : struct
            => value != null &&
               Enum.IsDefined(typeof(T), value)
                ? (T?)(T)Enum.ToObject(typeof(T), value)
                : null;
    }
}
