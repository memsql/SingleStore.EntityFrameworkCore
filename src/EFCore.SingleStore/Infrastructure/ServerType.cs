// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace EntityFrameworkCore.SingleStore.Infrastructure
{
    public enum ServerType
    {
        /// <summary>
        /// Custom server implementation
        /// </summary>
        Custom = -1,

        /// <summary>
        /// MySQL server
        /// </summary>
        SingleStore,

        /// <summary>
        /// MariaDB server
        /// </summary>
        MariaDb
    }
}
