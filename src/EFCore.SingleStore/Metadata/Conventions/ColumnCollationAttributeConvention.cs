// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EntityFrameworkCore.SingleStore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the column's collation for a property or field based on the applied <see cref="SingleStoreCollationAttribute" />.
    /// </summary>
    public class ColumnCollationAttributeConvention : PropertyAttributeConventionBase<SingleStoreCollationAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ColumnCollationAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public ColumnCollationAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            SingleStoreCollationAttribute attribute,
            MemberInfo clrMember,
            IConventionContext context)
            => propertyBuilder.UseCollation(attribute.CollationName);
    }
}
