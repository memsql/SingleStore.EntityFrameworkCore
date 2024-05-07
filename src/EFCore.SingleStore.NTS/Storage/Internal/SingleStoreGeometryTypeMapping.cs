// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.
/*
using System;
using System.Data.Common;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SingleStoreConnector;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.ValueConversion.Internal;

namespace EntityFrameworkCore.SingleStore.Storage.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SingleStoreGeometryTypeMapping<TGeometry> : RelationalGeometryTypeMapping<TGeometry, SingleStoreGeometry>
        where TGeometry : Geometry
    {
        private readonly ISingleStoreOptions _options;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly MethodInfo _getSingleStoreGeometry
            = typeof(SingleStoreDataReader).GetRuntimeMethod(nameof(SingleStoreDataReader.GetSingleStoreGeometry), new[] { typeof(int) });

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        [UsedImplicitly]
        public SingleStoreGeometryTypeMapping(NtsGeometryServices geometryServices, string storeType, ISingleStoreOptions options)
            : base(
                new GeometryValueConverter<TGeometry>(geometryServices),
                storeType)
        {
            _options = options;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected SingleStoreGeometryTypeMapping(
            RelationalTypeMappingParameters parameters,
            ValueConverter<TGeometry, SingleStoreGeometry> converter,
            ISingleStoreOptions options)
            : base(parameters, converter)
        {
            _options = options;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreGeometryTypeMapping<TGeometry>(parameters, SpatialConverter, _options);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override string GenerateNonNullSqlLiteral(object value)
        {
            // We are skipping the whole "ST_GeomFromText()" call here, because MySQL 8 switches the (lon, lat)
            // parameter order depending on what SRID is being used.
            // Just supplying the value as a WKB hex string should work for all database servers and versions.
            var mySqlGeometry = (SingleStoreGeometry)SpatialConverter.ConvertToProvider(value);
            var hexString = BitConverter.ToString(mySqlGeometry.Value)
                .Replace("-", string.Empty);

            return $"X'{hexString}'";
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override MethodInfo GetDataReaderMethod()
            => _getSingleStoreGeometry;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override string AsText(object value)
            => ((Geometry)value).AsText();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override int GetSrid(object value)
            => ((Geometry)value).SRID;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Type WKTReaderType
            => typeof(WKTReader);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            var mySqlParameter = (SingleStoreParameter)parameter;
            mySqlParameter.SingleStoreDbType = SingleStoreDbType.Geometry;
        }

        protected virtual bool CheckEmptyValue(Geometry geometry)
        {
            // Only `GeometryCollection.Empty` is currently supported by MySQL and MariaDB.
            if (geometry == Point.Empty ||
                geometry == LineString.Empty ||
                geometry == Polygon.Empty ||
                geometry == MultiPoint.Empty ||
                geometry == MultiLineString.Empty ||
                geometry == MultiPolygon.Empty)
            {
                throw new InvalidOperationException($@"An empty spatial geometry value has been used for a type of ""{geometry.GetType()}"". The only empty value currently supported by MySQL and MariaDB is ""GeometryCollection.Empty"".");
            }

            return geometry == GeometryCollection.Empty;
        }
    }
}*/
