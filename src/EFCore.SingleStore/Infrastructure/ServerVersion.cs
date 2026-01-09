// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Utilities;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// The abstract base class of <see cref="SingleStoreServerVersion"/> and <see cref="MariaDbServerVersion"/>.
    /// Contains static methods to create a <see cref="ServerVersion"/> from a string or to auto detect the server version from a database
    /// server.
    /// </summary>
    public abstract class ServerVersion
    {
        private static readonly Regex _versionRegex = new Regex(@"\d+\.\d+\.?(?:\d+)?");

        protected ServerVersion(Version version, ServerType type, string typeIdentifier = null)
        {
            Version = version;
            Type = type;
            TypeIdentifier = typeIdentifier ?? Enum.GetName(typeof(ServerType), type)
                .ToLowerInvariant();
        }

        public virtual Version Version { get; }
        public virtual ServerType Type { get; }
        public virtual string TypeIdentifier { get; }

        public abstract ServerVersionSupport Supports { get; }

        public virtual int MaxKeyLength => Supports.LargerKeyLength ? 3072 : 767;
        public virtual CharSet DefaultCharSet => Supports.DefaultCharSetUtf8Mb4 ? CharSet.Utf8Mb4 : CharSet.Utf8;
        public virtual string DefaultUtf8CsCollation => Supports.DefaultCharSetUtf8Mb4 ? "utf8mb4_bin" : "utf8_bin";
        public virtual string DefaultUtf8CiCollation => Supports.DefaultCharSetUtf8Mb4 ? "utf8mb4_general_ci" : "utf8_general_ci";

        public override bool Equals(object obj)
            => obj is ServerVersion version &&
               Equals(version);

        private bool Equals(ServerVersion other)
            => Version.Equals(other.Version) &&
               Type.Equals(other.Type) &&
               Equals(TypeIdentifier, other.TypeIdentifier);

        public override int GetHashCode()
            => HashCode.Combine(Version, Type, TypeIdentifier);

        /// <summary>
        /// Returns the server version and type in the format `major.minor.patch-type`.
        /// </summary>
        /// <returns>The server version and type string.</returns>
        public override string ToString()
            => $"{Version}-{TypeIdentifier}";

        /// <summary>
        /// Retrieves the <see cref="ServerVersion"/> (version number and server type) from a database server.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The <see cref="ServerVersion"/>.</returns>
        /// <remarks>
        /// Uses a connection string to open a connection to the database server and then executes a command.
        /// The connection will ignore the database specified in the connection string. It therefore makes not difference, whether the
        /// database already exists or not.
        /// </remarks>
        public static ServerVersion AutoDetect(string connectionString)
        {
            using var connection = new SingleStoreConnection(
                new SingleStoreConnectionStringBuilder(connectionString)
                {
                    Database = string.Empty,
                    AutoEnlist = false,
                    Pooling = false,
                }.ConnectionString);
            connection.Open();
            return Parse(connection.S2ServerVersion);
        }

        /// <summary>
        /// Retrieves the <see cref="ServerVersion"/> (version number and server type) from a database server.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The <see cref="ServerVersion"/>.</returns>
        /// <remarks>
        /// Uses a connection to the database server to execute a command.
        /// If the connection has already been opened, the connection is is being used as is. Otherwise, the connection is being cloned and
        /// ignores any database specified in the connection string of the connection. It therefore makes not difference, whether the
        /// database already exists or not, and the <see cref="ConnectionState"/> of the <paramref name="connection"/> parameter after the
        /// return of the call is the same as before the call.
        /// </remarks>
        public static ServerVersion AutoDetect(SingleStoreConnection connection)
        {
            string serverVersion;

            if (connection.State != ConnectionState.Open)
            {
                using var clonedConnection = connection.CloneWith(
                    new SingleStoreConnectionStringBuilder(connection.ConnectionString)
                    {
                        Database = string.Empty,
                        AutoEnlist = false,
                        Pooling = false,
                    }.ConnectionString);
                clonedConnection.Open();
                serverVersion = clonedConnection.S2ServerVersion;
            }
            else
            {
                serverVersion = connection.S2ServerVersion;
            }

            return Parse(serverVersion);
        }

        /// <summary>
        /// Converts a string, containing the server version and type, into a <see cref="ServerVersion"/>.
        /// </summary>
        /// <param name="versionString">The server version (mandatory) and type (optional).</param>
        /// <returns>The <see cref="ServerVersion"/>.</returns>
        /// <remarks>
        /// The general format is `major.minor.patch-type`, e.g. `8.0.21-mysql` or `10.5.3-mariadb`. If the type is being omitted, it is
        /// assumed to be MySQL (and not MariaDB).
        /// </remarks>
        public static ServerVersion Parse(string versionString)
            => Parse(versionString, null);

        /// <summary>
        /// Converts a string, containing the server version and type, into a <see cref="ServerVersion"/>.
        /// </summary>
        /// <param name="versionString">The server version (mandatory) and type (optional).</param>
        /// <param name="serverType">The <see cref="ServerType"/> or <see langword="null" />. </param>
        /// <returns>The <see cref="ServerVersion"/>.</returns>
        /// <remarks>
        /// The general format is `major.minor.patch-type`, e.g. `8.0.21-mysql` or `10.5.3-mariadb`. If the type is being omitted, it is
        /// assumed to be MySQL (and not MariaDB). The <paramref name="serverType"/> parameter takes precedence over a server type specified
        /// in the <paramref name="versionString"/> parameter, if not <see langword="null" />.
        /// </remarks>
        public static ServerVersion Parse(string versionString, ServerType? serverType)
        {
            Check.NotEmpty(versionString, nameof(versionString));

            if (!TryParse(versionString, serverType, out var serverVersion))
            {
                throw new InvalidOperationException($"Unable to determine server version from version string '${versionString}'.");
            }

            return serverVersion;
        }

        /// <summary>
        /// Tries to converts a string, containing the server version and type, into a <see cref="ServerVersion"/>.
        /// </summary>
        /// <param name="versionString">The server version (mandatory) and type (optional).</param>
        /// <param name="serverVersion">The <see cref="ServerVersion"/>.</param>
        /// <returns><see langword="true" /> if the conversion was successful, otherwise <see langword="false" />.</returns>
        /// <remarks>
        /// The general format is `major.minor.patch-type`, e.g. `8.0.21-mysql` or `10.5.3-mariadb`. If the type is being omitted, it is
        /// assumed to be MySQL (and not MariaDB).
        /// </remarks>
        public static bool TryParse(string versionString, out ServerVersion serverVersion)
            => TryParse(versionString, null, out serverVersion);

        /// <summary>
        /// Tries to converts a string, containing the server version and type, into a <see cref="ServerVersion"/>.
        /// </summary>
        /// <param name="versionString">The server version (mandatory) and type (optional).</param>
        /// <param name="serverType">The <see cref="ServerType"/> or <see langword="null" />. </param>
        /// <param name="serverVersion">The <see cref="ServerVersion"/>.</param>
        /// <returns><see langword="true" /> if the conversion was successful, otherwise <see langword="false" />.</returns>
        /// <remarks>
        /// The general format is `major.minor.patch-type`, e.g. `8.0.21-mysql` or `10.5.3-mariadb`. If the type is being omitted, it is
        /// assumed to be MySQL (and not MariaDB). The <paramref name="serverType"/> parameter takes precedence over a server type specified
        /// in the <paramref name="versionString"/> parameter, if not <see langword="null" />.
        /// </remarks>
        public static bool TryParse(string versionString, ServerType? serverType, out ServerVersion serverVersion)
        {
            Check.NotEmpty(versionString, nameof(versionString));

            serverVersion = null;

            var semanticVersion = _versionRegex.Matches(versionString);
            if (semanticVersion.Count > 0)
            {
                var type = serverType ??
                           (versionString.ToLower().Contains(MariaDbServerVersion.MariaDbTypeIdentifier)
                               ? ServerType.MariaDb
                               : ServerType.SingleStore);

                var version = type == ServerType.MariaDb &&
                              semanticVersion.Count > 1
                    ? Version.Parse(semanticVersion[1].Value)
                    : Version.Parse(semanticVersion[0].Value);

                serverVersion = type switch
                {
                    ServerType.SingleStore => new SingleStoreServerVersion(version),
                    ServerType.MariaDb => new MariaDbServerVersion(version),
                    _ => null
                };
            }

            return serverVersion != null;
        }

        /// <summary>
        /// Creates a <see cref="ServerVersion"/> object from a version and type.
        /// </summary>
        /// <param name="version">The <see cref="Version"/> of the database server.</param>
        /// <param name="serverType">The <see cref="ServerType"/> of the database server.</param>
        /// <returns>The <see cref="ServerVersion"/>.</returns>
        public static ServerVersion Create(Version version, ServerType serverType)
            => serverType switch
            {
                ServerType.SingleStore => new SingleStoreServerVersion(version),
                ServerType.MariaDb => new MariaDbServerVersion(version),
                _ => throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null)
            };

        /// <summary>
        /// Creates a <see cref="ServerVersion"/> object from a version and type.
        /// </summary>
        /// <param name="major">The major version of the database server.</param>
        /// <param name="minor">The minor version of the database server.</param>
        /// <param name="patch">The patch level of the database server.</param>
        /// <param name="serverType">The <see cref="ServerType"/> of the database server.</param>
        /// <returns>The <see cref="ServerVersion"/>.</returns>
        public static ServerVersion Create(int major, int minor, int patch, ServerType serverType)
            => Create(new Version(major, minor, patch), serverType);
    }
}
