// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace EntityFrameworkCore.SingleStore.Storage.Internal.Json;

public sealed class SingleStoreJsonByteArrayAsHexStringReaderWriter : JsonValueReaderWriter<byte[]>
{
    public static SingleStoreJsonByteArrayAsHexStringReaderWriter Instance { get; } = new();

    private SingleStoreJsonByteArrayAsHexStringReaderWriter()
    {
    }

    public override byte[] FromJsonTyped(ref Utf8JsonReaderManager manager, object existingObject = null)
        => Convert.FromHexString(manager.CurrentReader.GetString()!);

    public override void ToJsonTyped(Utf8JsonWriter writer, byte[] value)
        => writer.WriteStringValue(Convert.ToHexString(value));
}
