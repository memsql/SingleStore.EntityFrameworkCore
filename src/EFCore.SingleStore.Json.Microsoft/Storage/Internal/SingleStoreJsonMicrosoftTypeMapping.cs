// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SingleStoreConnector;
using EntityFrameworkCore.SingleStore.Infrastructure.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Microsoft.Storage.Internal
{
    public class SingleStoreJsonMicrosoftTypeMapping<T> : SingleStoreJsonTypeMapping<T>
    {
        // Called via reflection.
        // ReSharper disable once UnusedMember.Global
        public SingleStoreJsonMicrosoftTypeMapping(
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

        protected SingleStoreJsonMicrosoftTypeMapping(
            RelationalTypeMappingParameters parameters,
            SingleStoreDbType mySqlDbType,
            ISingleStoreOptions options)
            : base(parameters, mySqlDbType, options)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SingleStoreJsonMicrosoftTypeMapping<T>(parameters, SingleStoreDbType, Options);

        public override Expression GenerateCodeLiteral(object value)
            => value switch
            {
                JsonDocument document => Expression.Call(
                    typeof(JsonDocument).GetMethod(nameof(JsonDocument.Parse), new[] {typeof(string), typeof(JsonDocumentOptions)}),
                    Expression.Constant(document.RootElement.ToString()),
                    Expression.New(typeof(JsonDocumentOptions))),
                JsonElement element => Expression.Property(
                    Expression.Call(
                        typeof(JsonDocument).GetMethod(nameof(JsonDocument.Parse), new[] {typeof(string), typeof(JsonDocumentOptions)}),
                        Expression.Constant(element.ToString()),
                        Expression.New(typeof(JsonDocumentOptions))),
                    nameof(JsonDocument.RootElement)),
                string s => Expression.Constant(s),
                _ => throw new NotSupportedException("Cannot generate code literals for JSON POCOs.")
            };
    }
}
