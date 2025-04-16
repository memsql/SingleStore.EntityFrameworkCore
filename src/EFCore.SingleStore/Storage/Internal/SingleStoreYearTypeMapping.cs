// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    public class SingleStoreYearTypeMapping : SingleStoreTypeMapping
    {
        public static SingleStoreYearTypeMapping Default { get; } = new("year");

        public SingleStoreYearTypeMapping([NotNull] string storeType)
            : base(
                storeType,
                typeof(short),
                SingleStoreDbType.Year,
                System.Data.DbType.Int16,
                jsonValueReaderWriter: JsonInt16ReaderWriter.Instance)
        {
        }

        protected SingleStoreYearTypeMapping(RelationalTypeMappingParameters parameters, SingleStoreDbType mySqlDbType)
            : base(parameters, mySqlDbType)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreYearTypeMapping(parameters, SingleStoreDbType);
    }
}
