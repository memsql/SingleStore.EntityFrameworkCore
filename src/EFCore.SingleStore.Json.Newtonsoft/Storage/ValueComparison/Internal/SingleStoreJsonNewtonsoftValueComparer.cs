// Copyright (c) Pomelo Foundation. All rights reserved.
// Copyright (c) SingleStore Inc. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.ValueConversion.Internal;
using EntityFrameworkCore.SingleStore.Storage.Internal;
using EntityFrameworkCore.SingleStore.Storage.ValueComparison.Internal;

namespace EntityFrameworkCore.SingleStore.Json.Newtonsoft.Storage.ValueComparison.Internal
{
    public static class SingleStoreJsonNewtonsoftValueComparer
    {
        public static ValueComparer Create(Type t, SingleStoreJsonChangeTrackingOptions options = SingleStoreJsonChangeTrackingOptions.None)
            => (ValueComparer)Activator.CreateInstance(
                typeof(SingleStoreJsonNewtonsoftValueComparer<>).MakeGenericType(t),
                options);

        public static readonly JTokenEqualityComparer JTokenEqualityComparer = new JTokenEqualityComparer();
    }

    public class SingleStoreJsonNewtonsoftValueComparer<T> : ValueComparer<T>, ISingleStoreJsonValueComparer
    {
        public SingleStoreJsonNewtonsoftValueComparer(
            SingleStoreJsonChangeTrackingOptions options = SingleStoreJsonChangeTrackingOptions.None)
            : base(
                CreateEqualsExpression(options),
                CreateHashCodeExpression(options),
                CreateSnapshotExpression(options))
        {
        }

        public ValueComparer Clone(SingleStoreJsonChangeTrackingOptions options)
            => new SingleStoreJsonNewtonsoftValueComparer<T>(options);

        private static Expression<Func<T, T, bool>> CreateEqualsExpression(SingleStoreJsonChangeTrackingOptions options)
        {
            var type = typeof(T);
            var param1 = Expression.Parameter(type, "v1");
            var param2 = Expression.Parameter(type, "v2");

            if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.CompareRootPropertyOnly))
            {
                return CreateDefaultEqualsExpression();
            }

            var jTokenType = typeof(JToken);
            if (jTokenType.IsAssignableFrom(type))
            {
                if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.CompareDomSemantically))
                {
                    var jsonComparerEqualsMethod = jTokenType.GetRuntimeMethods()
                        .First(
                            m => m.ReturnType == typeof(bool) &&
                                 m.IsStatic &&
                                 nameof(JToken.DeepEquals).Equals(m.Name, StringComparison.Ordinal) &&
                                 m.GetParameters().Length == 2 &&
                                 m.GetParameters()[0].ParameterType == jTokenType &&
                                 m.GetParameters()[1].ParameterType == jTokenType);

                    return Expression.Lambda<Func<T, T, bool>>(
                        Expression.Call(
                            jsonComparerEqualsMethod,
                            param1,
                            param2),
                        param1, param2);
                }

                if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.CompareDomRootPropertyByEquals))
                {
                    return CreateDefaultEqualsExpression();
                }
            }

            if (type == typeof(string))
            {
                if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.CompareStringRootPropertyByEquals))
                {
                    return CreateDefaultEqualsExpression();
                }

                // Force the string to be parsed, so that e.g. whitespaces are normalized.
                return (left, right) => SingleStoreJsonNewtonsoftStringValueConverter.ProcessJsonString(left as string) == SingleStoreJsonNewtonsoftStringValueConverter.ProcessJsonString(right as string);
            }

            var unwrappedType = type.UnwrapNullableType();
            if (unwrappedType.IsInteger() ||
                unwrappedType == typeof(Guid) ||
                unwrappedType == typeof(bool) ||
                unwrappedType == typeof(decimal) ||
                unwrappedType == typeof(double) ||
                unwrappedType == typeof(float) ||
                unwrappedType == typeof(object))
            {
                return Expression.Lambda<Func<T, T, bool>>(
                    Expression.Equal(param1, param2),
                    param1, param2);
            }

            // The following includes IEquatable<T> implementations.
            var typedEquals = type.GetRuntimeMethods()
                .FirstOrDefault(
                    m => m.ReturnType == typeof(bool) &&
                         !m.IsStatic &&
                         nameof(object.Equals).Equals(m.Name, StringComparison.Ordinal) &&
                         m.GetParameters().Length == 1 &&
                         m.GetParameters()[0].ParameterType == type);

            while (typedEquals == null &&
                   type != null)
            {
                var declaredMethods = type.GetTypeInfo()
                    .DeclaredMethods;
                typedEquals = declaredMethods.FirstOrDefault(
                    m => m.IsStatic &&
                         m.ReturnType == typeof(bool) &&
                         "op_Equality".Equals(m.Name, StringComparison.Ordinal) &&
                         m.GetParameters().Length == 2 &&
                         m.GetParameters()[0].ParameterType == type &&
                         m.GetParameters()[1].ParameterType == type);

                type = type.BaseType;
            }

            if (typedEquals != null)
            {
                return Expression.Lambda<Func<T, T, bool>>(
                    typedEquals.IsStatic
                        ? Expression.Call(typedEquals, param1, param2)
                        : Expression.Call(param1, typedEquals, param2),
                    param1, param2);
            }

            return (left, right) => object.Equals(JsonConvert.SerializeObject(left), JsonConvert.SerializeObject(right));
        }

        private static Expression<Func<T, int>> CreateHashCodeExpression(SingleStoreJsonChangeTrackingOptions options)
        {
            var type = typeof(T);
            var jTokenType = typeof(JToken);

            if (jTokenType.IsAssignableFrom(type))
            {
                if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.HashDomSemantiallyOptimized) ||
                    options.HasFlag(SingleStoreJsonChangeTrackingOptions.HashDomSemantially))
                {
                    var param1 = Expression.Parameter(type, "v1");
                    var jTokenEqualityComparerType = typeof(JTokenEqualityComparer);

                    var jsonComparerGetHashCodeMethod = jTokenEqualityComparerType.GetRuntimeMethods()
                        .First(
                            m => m.ReturnType == typeof(int) &&
                                 !m.IsStatic &&
                                 nameof(JTokenEqualityComparer.GetHashCode).Equals(m.Name, StringComparison.Ordinal) &&
                                 m.GetParameters().Length == 1 &&
                                 m.GetParameters()[0].ParameterType == jTokenType);

                    return Expression.Lambda<Func<T, int>>(
                        Expression.Call(
                            Expression.Constant(SingleStoreJsonNewtonsoftValueComparer.JTokenEqualityComparer, jTokenEqualityComparerType),
                            jsonComparerGetHashCodeMethod,
                            param1),
                        param1);
                }
            }

            return CreateDefaultHashCodeExpression(false);
        }

        private static Expression<Func<T, T>> CreateSnapshotExpression(SingleStoreJsonChangeTrackingOptions options)
        {
            var type = typeof(T);

            var unwrappedType = type.UnwrapNullableType();
            if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.CompareRootPropertyOnly) ||
                type == typeof(string) ||
                unwrappedType.IsInteger() ||
                unwrappedType == typeof(Guid) ||
                unwrappedType == typeof(bool) ||
                unwrappedType == typeof(decimal) ||
                unwrappedType == typeof(double) ||
                unwrappedType == typeof(float) ||
                unwrappedType == typeof(object))
            {
                return v => v;
            }

            if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.SnapshotCallsDeepClone))
            {
                var deepCloneMethod = type.GetRuntimeMethod("DeepClone", Type.EmptyTypes);
                if (deepCloneMethod != null &&
                    (deepCloneMethod.ReturnType == type || deepCloneMethod.ReturnType == typeof(object)))
                {
                    var param1 = Expression.Parameter(type, "v");

                    return Expression.Lambda<Func<T, T>>(
                        deepCloneMethod.ReturnType == typeof(object)
                            ? (Expression)Expression.Convert(
                                Expression.Call(
                                    param1,
                                    deepCloneMethod),
                                type)
                            : Expression.Call(
                                param1,
                                deepCloneMethod),
                        param1);
                }
            }

            if (options.HasFlag(SingleStoreJsonChangeTrackingOptions.SnapshotCallsClone))
            {
                // The following includes, but is not limited to, ICloneable implementations.
                // This also includes Array.Clone().
                var cloneMethod = type.GetRuntimeMethod(nameof(ICloneable.Clone), Type.EmptyTypes);
                if (cloneMethod != null &&
                    (cloneMethod.ReturnType == type || cloneMethod.ReturnType == typeof(object)))
                {
                    var param1 = Expression.Parameter(type, "v");

                    return Expression.Lambda<Func<T, T>>(
                        cloneMethod.ReturnType == typeof(object)
                            ? (Expression)Expression.Convert(
                                Expression.Call(
                                    param1,
                                    cloneMethod),
                                type)
                            : Expression.Call(
                                param1,
                                cloneMethod),
                        param1);
                }
            }

            return v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v));
        }
    }
}
