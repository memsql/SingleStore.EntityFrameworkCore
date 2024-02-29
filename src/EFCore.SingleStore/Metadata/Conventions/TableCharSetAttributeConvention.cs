// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EntityFrameworkCore.SingleStore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the character set for an entity based on the applied <see cref="SingleStoreCharSetAttribute" />.
    /// </summary>
    public class TableCharSetAttributeConvention : EntityTypeAttributeConventionBase<SingleStoreCharSetAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="TableCharSetAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public TableCharSetAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            SingleStoreCharSetAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
            => entityTypeBuilder.HasCharSet(attribute.CharSetName, attribute.DelegationModes);
    }
}
