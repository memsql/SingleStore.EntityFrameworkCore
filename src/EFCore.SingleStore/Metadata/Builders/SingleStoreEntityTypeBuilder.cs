// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pomelo.EntityFrameworkCore.SingleStore.Metadata.Builders
{
    public class SingleStoreEntityTypeBuilder : EntityTypeBuilder
    {
        public SingleStoreEntityTypeBuilder(IMutableEntityType entityType) : base(entityType)
        {

        }

        private InternalEntityTypeBuilder Builder { [DebuggerStepThrough] get; }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="OwnedNavigationBuilder.WithOwner" /> to fully configure the relationship.
        ///     </para>
        /// </summary>
        /// <param name="ownedTypeName">The name of the entity type that this relationship targets.</param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <returns>An object that can be used to configure the owned type and the relationship.</returns>
        public override OwnedNavigationBuilder OwnsOne(
            string ownedTypeName,
            string navigationName)
            => OwnsOneBuilder(
                new TypeIdentity(Check.NotEmpty(ownedTypeName, nameof(ownedTypeName))),
                Check.NotEmpty(navigationName, nameof(navigationName)));

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="OwnedNavigationBuilder.WithOwner" /> to fully configure the relationship.
        ///     </para>
        /// </summary>
        /// <param name="ownedTypeName">The name of the entity type that this relationship targets.</param>
        /// <param name="ownedType">The CLR type of the entity type that this relationship targets.</param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <returns>An object that can be used to configure the owned type and the relationship.</returns>
        public override OwnedNavigationBuilder OwnsOne(
            string ownedTypeName,
            Type ownedType,
            string navigationName)
            => OwnsOneBuilder(
                new TypeIdentity(Check.NotEmpty(ownedTypeName, nameof(ownedTypeName)), ownedType),
                Check.NotEmpty(navigationName, nameof(navigationName)));

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="OwnedNavigationBuilder.WithOwner" /> to fully configure the relationship.
        ///     </para>
        /// </summary>
        /// <param name="ownedType">The entity type that this relationship targets.</param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <returns>An object that can be used to configure the owned type and the relationship.</returns>
        public override OwnedNavigationBuilder OwnsOne(
            Type ownedType,
            string navigationName)
            => OwnsOneBuilder(
                new TypeIdentity(Check.NotNull(ownedType, nameof(ownedType)), (Model)Metadata.Model),
                Check.NotEmpty(navigationName, nameof(navigationName)));

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="OwnedNavigationBuilder.WithOwner" /> to fully configure the relationship.
        ///     </para>
        /// </summary>
        /// <param name="ownedTypeName">The name of the entity type that this relationship targets.</param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <param name="buildAction">An action that performs configuration of the owned type and the relationship.</param>
        /// <returns>An object that can be used to configure the entity type.</returns>
        public override EntityTypeBuilder OwnsOne(
            string ownedTypeName,
            string navigationName,
            Action<OwnedNavigationBuilder> buildAction)
        {
            Check.NotEmpty(ownedTypeName, nameof(ownedTypeName));
            Check.NotEmpty(navigationName, nameof(navigationName));
            Check.NotNull(buildAction, nameof(buildAction));

            buildAction(OwnsOneBuilder(new TypeIdentity(ownedTypeName), navigationName));
            return this;
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="OwnedNavigationBuilder.WithOwner" /> to fully configure the relationship.
        ///     </para>
        /// </summary>
        /// <param name="ownedTypeName">The name of the entity type that this relationship targets.</param>
        /// <param name="ownedType">The CLR type of the entity type that this relationship targets.</param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <param name="buildAction">An action that performs configuration of the owned type and the relationship.</param>
        /// <returns>An object that can be used to configure the entity type.</returns>
        public override EntityTypeBuilder OwnsOne(
            string ownedTypeName,
            Type ownedType,
            string navigationName,
            Action<OwnedNavigationBuilder> buildAction)
        {
            Check.NotEmpty(ownedTypeName, nameof(ownedTypeName));
            Check.NotNull(ownedType, nameof(ownedType));
            Check.NotEmpty(navigationName, nameof(navigationName));
            Check.NotNull(buildAction, nameof(buildAction));

            buildAction(OwnsOneBuilder(new TypeIdentity(ownedTypeName, ownedType), navigationName));
            return this;
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="OwnedNavigationBuilder.WithOwner" /> to fully configure the relationship.
        ///     </para>
        /// </summary>
        /// <param name="ownedType">The entity type that this relationship targets.</param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <param name="buildAction">An action that performs configuration of the owned type and the relationship.</param>
        /// <returns>An object that can be used to configure the entity type.</returns>
        public override EntityTypeBuilder OwnsOne(
            Type ownedType,
            string navigationName,
            Action<OwnedNavigationBuilder> buildAction)
        {
            Check.NotNull(ownedType, nameof(ownedType));
            Check.NotEmpty(navigationName, nameof(navigationName));
            Check.NotNull(buildAction, nameof(buildAction));

            buildAction(OwnsOneBuilder(new TypeIdentity(ownedType, (Model)Metadata.Model), navigationName));
            return this;
        }

        private OwnedNavigationBuilder OwnsOneBuilder(in TypeIdentity ownedType, string navigationName)
        {
            IMutableForeignKey foreignKey;
            using (var batch = Builder.Metadata.Model.DelayConventions())
            {
                var navigationMember = new MemberIdentity(navigationName);
                var relationship = Builder.HasOwnership(ownedType, navigationMember, ConfigurationSource.Explicit)!;
                relationship.IsUnique(false, ConfigurationSource.Explicit);
                foreignKey = (IMutableForeignKey)batch.Run(relationship.Metadata)!;
            }

            return new OwnedNavigationBuilder(foreignKey);
        }
    }
}
