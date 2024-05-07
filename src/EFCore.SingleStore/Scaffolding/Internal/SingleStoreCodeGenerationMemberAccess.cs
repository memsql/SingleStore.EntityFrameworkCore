// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Reflection;

namespace EntityFrameworkCore.SingleStore.Scaffolding.Internal
{
    internal class SingleStoreCodeGenerationMemberAccess
    {
        public MemberInfo MemberInfo { get; }

        public SingleStoreCodeGenerationMemberAccess(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
    }
}
