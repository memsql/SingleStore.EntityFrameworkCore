// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreConnectionStringOptionsValidator : ISingleStoreConnectionStringOptionsValidator
{
    public virtual bool EnsureMandatoryOptions(ref string connectionString)
    {
        if (connectionString is not null)
        {
            var csb = new SingleStoreConnectionStringBuilder(connectionString);

            var flagsChanged = false;

            if (!ValidateMandatoryOptions(csb))
            {
                csb.AllowUserVariables = true;
                csb.UseAffectedRows = false;
                flagsChanged = true;
            }

            if (flagsChanged)
            {
                // Only add attrs when we're already changing the string anyway
                AddConnectionAttributes(csb);
                connectionString = csb.ConnectionString;
                return true;
            }
        }

        return false;
    }

    public virtual bool EnsureMandatoryOptions(DbConnection connection)
    {
        if (connection is null)
        {
            return false;
        }

        var csb = new SingleStoreConnectionStringBuilder(connection.ConnectionString);

        if (!ValidateMandatoryOptions(csb))
        {
            try
            {
                csb.AllowUserVariables = true;
                csb.UseAffectedRows = false;

                connection.ConnectionString = csb.ConnectionString;
                return true;
            }
            catch (Exception e)
            {
                ThrowException(e);
            }
        }

        return false;
    }

    public virtual bool EnsureMandatoryOptions(DbDataSource dataSource)
    {
        if (dataSource is null)
        {
            return false;
        }

        var csb = new SingleStoreConnectionStringBuilder(dataSource.ConnectionString);

        // We can’t persist ConnectionAttributes changes back to the data source, so don’t attempt.
        if (!ValidateMandatoryOptions(csb))
        {
            ThrowException();
        }

        return true;
    }

    private static bool AddConnectionAttributes(SingleStoreConnectionStringBuilder csb)
    {
        var existing = csb.ConnectionAttributes?.TrimEnd(',') ?? "";

        var existingAttrs = existing
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(attr => attr.Trim())
            .Where(attr => !string.IsNullOrEmpty(attr))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var programVersion = typeof(SingleStoreConnectionStringOptionsValidator).Assembly.GetName().Version;
        var nameAttr = "_connector_name:SingleStore Entity Framework Core provider";
        var versionAttr = $"_connector_version:{programVersion}";

        var addedNameAttr = existingAttrs.Add(nameAttr);
        var addedVersionAttr = existingAttrs.Add(versionAttr);
        var changed = addedNameAttr || addedVersionAttr;

        if (changed)
        {
            csb.ConnectionAttributes = string.Join(",", existingAttrs);
            return true;
        }

        return false;
    }

    public virtual void ThrowException(Exception innerException = null)
        => throw new InvalidOperationException(
            @"The connection string of a connection used by EntityFrameworkCore.SingleStore must contain ""AllowUserVariables=True;UseAffectedRows=False"".",
            innerException);

    protected virtual bool ValidateMandatoryOptions(SingleStoreConnectionStringBuilder csb)
        => csb.AllowUserVariables &&
           !csb.UseAffectedRows;
}
