// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.SingleStore.Storage;

namespace EntityFrameworkCore.SingleStore.Scaffolding.Internal
{
    internal class SingleStoreCodeGenerationServerVersionCreation
    {
        public ServerVersion ServerVersion { get; }

        public SingleStoreCodeGenerationServerVersionCreation(ServerVersion serverVersion)
        {
            ServerVersion = serverVersion;
        }
    }
}
