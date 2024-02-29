// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public interface IJsonSpecificTypeMapping
{
    RelationalTypeMapping CloneAsJsonCompatible();
}
