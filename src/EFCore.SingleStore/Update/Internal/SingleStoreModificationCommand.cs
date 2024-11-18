// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.SingleStore.Update.Internal;

public class SingleStoreModificationCommand : ModificationCommand
{
    private readonly bool _detailedErrorsEnabled;

    public SingleStoreModificationCommand(in ModificationCommandParameters modificationCommandParameters)
        : base(in modificationCommandParameters)
        => _detailedErrorsEnabled = modificationCommandParameters.DetailedErrorsEnabled;

    public SingleStoreModificationCommand(in NonTrackedModificationCommandParameters modificationCommandParameters)
        : base(in modificationCommandParameters)
    {
    }

    public override void PropagateResults(RelationalDataReader relationalReader)
    {
        base.PropagateResults(relationalReader);
    }
}
