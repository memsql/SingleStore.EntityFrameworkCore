// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    /// <summary>
    ///     Detects the exceptions caused by MySQL transient failures.
    /// </summary>
    public static class SingleStoreTransientExceptionDetector
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static bool ShouldRetryOn([NotNull] Exception ex)
            => ex is SingleStoreException mySqlException
                ? mySqlException.IsTransient
                : ex is TimeoutException;
    }
}
