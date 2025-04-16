// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using SingleStoreConnector;

namespace EntityFrameworkCore.SingleStore.Storage.Internal;

public class SingleStoreConnectionStringOptionsValidator : ISingleStoreConnectionStringOptionsValidator
{
    public virtual bool EnsureMandatoryOptions(ref string connectionString)
    {
        if (connectionString is not null)
        {
            var csb = new SingleStoreConnectionStringBuilder(connectionString);

            if (!ValidateMandatoryOptions(csb))
            {
                csb.AllowUserVariables = true;
                csb.UseAffectedRows = false;

                connectionString = csb.ConnectionString;

                return true;
            }
        }

        return false;
    }

    public virtual bool EnsureMandatoryOptions(DbConnection connection)
    {
        if (connection is not null)
        {
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

        if (!ValidateMandatoryOptions(csb))
        {
            // We can't alter the connection string of a DbDataSource/SingleStoreDataSource as we do for DbConnection/SingleStoreConnection in cases
            // where the necessary connection string options have not been set.
            // We can only throw.
            ThrowException();
        }

        return true;
    }

    public virtual void ThrowException(Exception innerException = null)
        => throw new InvalidOperationException(
            @"The connection string of a connection used by EntityFrameworkCore.SingleStore must contain ""AllowUserVariables=True;UseAffectedRows=False"".",
            innerException);

    protected virtual bool ValidateMandatoryOptions(SingleStoreConnectionStringBuilder csb)
        => csb.AllowUserVariables &&
           !csb.UseAffectedRows;
}
