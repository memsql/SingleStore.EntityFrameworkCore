// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SingleStoreConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.Internal
{
    public class SingleStoreJsonNewtonsoftTypeMapping<T> : SingleStoreJsonTypeMapping<T>
    {
        // Called via reflection.
        // ReSharper disable once UnusedMember.Global
        public SingleStoreJsonNewtonsoftTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            [NotNull] ISingleStoreOptions options)
            : base(
                storeType,
                valueConverter,
                valueComparer,
                options)
        {
        }

        protected SingleStoreJsonNewtonsoftTypeMapping(
            RelationalTypeMappingParameters parameters,
            SingleStoreDbType mySqlDbType,
            ISingleStoreOptions options)
            : base(parameters, mySqlDbType, options)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreJsonNewtonsoftTypeMapping<T>(parameters, SingleStoreDbType, Options);

        public override Expression GenerateCodeLiteral(object value)
            => value switch
            {
                JToken jToken => Expression.Call(
                    typeof(JToken).GetMethod(nameof(JToken.Parse), new[] {typeof(string), typeof(JsonLoadSettings)}),
                    Expression.Constant(jToken.ToString(Formatting.None)),
                    Expression.New(typeof(JsonLoadSettings))),
                string s => Expression.Constant(s),
                _ => throw new NotSupportedException("Cannot generate code literals for JSON POCOs.")
            };
    }
}
