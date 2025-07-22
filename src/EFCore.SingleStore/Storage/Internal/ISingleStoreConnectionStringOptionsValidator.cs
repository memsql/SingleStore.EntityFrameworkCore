// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public interface ISingleStoreConnectionStringOptionsValidator
{
    bool EnsureMandatoryOptions(ref string connectionString);
    bool EnsureMandatoryOptions(DbConnection connection);
    bool EnsureMandatoryOptions(DbDataSource dataSource);

    void ThrowException(Exception innerException = null);
}
