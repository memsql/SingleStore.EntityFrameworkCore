// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using EntityFrameworkCore.SingleStore.Infrastructure;

namespace EntityFrameworkCore.SingleStore.Scaffolding.Internal
{
    internal class SingleStoreCodeGenerationServerVersionCreationTypeMapping : RelationalTypeMapping
    {
        private const string DummyStoreType = "clrOnly";

        public SingleStoreCodeGenerationServerVersionCreationTypeMapping()
            : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(SingleStoreCodeGenerationServerVersionCreation)), DummyStoreType))
        {
        }

        protected SingleStoreCodeGenerationServerVersionCreationTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreCodeGenerationServerVersionCreationTypeMapping(parameters);

        public override string GenerateSqlLiteral(object value)
            => throw new InvalidOperationException("This type mapping exists for code generation only.");

        public override Expression GenerateCodeLiteral(object value)
            => value is SingleStoreCodeGenerationServerVersionCreation serverVersionCreation
                ? Expression.Call(
                    typeof(ServerVersion).GetMethod(nameof(ServerVersion.Parse), new[] {typeof(string)}),
                    Expression.Constant(serverVersionCreation.ServerVersion.ToString()))
                : null;
    }
}
