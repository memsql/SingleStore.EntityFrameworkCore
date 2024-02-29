// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.SingleStore.Scaffolding.Internal
{
    internal class SingleStoreCodeGenerationMemberAccessTypeMapping : RelationalTypeMapping
    {
        private const string DummyStoreType = "clrOnly";

        public SingleStoreCodeGenerationMemberAccessTypeMapping()
            : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(typeof(SingleStoreCodeGenerationMemberAccess)), DummyStoreType))
        {
        }

        protected SingleStoreCodeGenerationMemberAccessTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreCodeGenerationMemberAccessTypeMapping(parameters);

        public override string GenerateSqlLiteral(object value)
            => throw new InvalidOperationException("This type mapping exists for code generation only.");

        public override Expression GenerateCodeLiteral(object value)
            => value is SingleStoreCodeGenerationMemberAccess memberAccess
                ? Expression.MakeMemberAccess(null, memberAccess.MemberInfo)
                : null;
    }
}
