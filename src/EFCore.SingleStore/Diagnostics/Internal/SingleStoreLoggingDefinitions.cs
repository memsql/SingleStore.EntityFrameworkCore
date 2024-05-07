// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityFrameworkCore.SingleStore.Diagnostics.Internal
{
    public class SingleStoreLoggingDefinitions : RelationalLoggingDefinitions
    {
        public EventDefinitionBase LogDefaultDecimalTypeColumn;

        public EventDefinitionBase LogByteIdentityColumn;

        public EventDefinitionBase LogFoundDefaultSchema;

        public EventDefinitionBase LogFoundTypeAlias;

        public EventDefinitionBase LogFoundColumn;

        public EventDefinitionBase LogFoundForeignKey;

        public EventDefinitionBase LogPrincipalTableNotInSelectionSet;

        public EventDefinitionBase LogMissingSchema;

        public EventDefinitionBase LogMissingTable;

        public EventDefinitionBase LogFoundSequence;

        public EventDefinitionBase LogFoundTable;

        public EventDefinitionBase LogFoundIndex;

        public EventDefinitionBase LogFoundPrimaryKey;

        public EventDefinitionBase LogFoundUniqueConstraint;

        public EventDefinitionBase LogPrincipalColumnNotFound;

        public EventDefinitionBase LogReflexiveConstraintIgnored;

        public EventDefinitionBase LogDefaultValueNotSupported;
    }
}
