using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Honse.Global.Extensions
{
    public static class ObjectCopyExtensions
    {
        private static readonly ConcurrentDictionary<(Type Source, Type Target), Delegate> _copyDelegatesCache = new();

        // Thread-local storage for circular reference tracking
        private static readonly ThreadLocal<HashSet<object>> _currentCopyChain = new ThreadLocal<HashSet<object>>(
            () => new HashSet<object>(ReferenceEqualityComparer.Instance)
        );

        public static T DeepCopyTo<T>(this object source)
        {
            if (source == null) return default(T);

            // Reset the circular reference tracking for this copy operation
            _currentCopyChain.Value.Clear();

            try
            {
                return (T)DeepCopyInternal(source, source.GetType(), typeof(T));
            }
            finally
            {
                // Clean up
                _currentCopyChain.Value.Clear();
            }
        }

        private static object DeepCopyInternal(object source, Type sourceType, Type targetType)
        {
            if (source == null) return null;

            // Handle value types and strings
            if (sourceType.IsValueType || sourceType == typeof(string))
                return source;

            // Check for circular references
            if (_currentCopyChain.Value.Contains(source))
            {
                throw new InvalidOperationException("Circular reference detected during deep copy");
            }

            // Add current object to the copy chain
            _currentCopyChain.Value.Add(source);

            try
            {
                // Handle arrays
                if (sourceType.IsArray)
                {
                    return CopyArray((Array)source, targetType);
                }

                // Handle collections
                if (typeof(IEnumerable).IsAssignableFrom(sourceType) && sourceType != typeof(string))
                {
                    return CopyCollection(source, sourceType, targetType);
                }

                // Handle objects - get or create copy delegate
                var key = (sourceType, targetType);
                var copyDelegate = _copyDelegatesCache.GetOrAdd(key, k => CreateCopyDelegate(k.Source, k.Target));

                return copyDelegate.DynamicInvoke(source);
            }
            finally
            {
                // Remove current object from copy chain when we're done
                _currentCopyChain.Value.Remove(source);
            }
        }

        private static Delegate CreateCopyDelegate(Type sourceType, Type targetType)
        {
            var sourceParam = Expression.Parameter(typeof(object), "source");
            var sourceVariable = Expression.Variable(sourceType, "typedSource");
            var targetVariable = Expression.Variable(targetType, "target");

            var expressions = new List<Expression>
        {
            // Convert source to typed variable
            Expression.Assign(sourceVariable, Expression.Convert(sourceParam, sourceType)),
            // Create target instance with proper type conversion
            Expression.Assign(targetVariable, Expression.Convert(CreateInstance(targetType), targetType))
        };

            // Copy properties - use more flexible matching for different but compatible types
            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);
            var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var sourceProp in sourceProperties)
            {
                var targetProp = targetProperties.FirstOrDefault(p => p.Name == sourceProp.Name);

                if (targetProp != null)
                {
                    var sourceValue = Expression.Property(sourceVariable, sourceProp);
                    expressions.Add(CreatePropertyCopyExpression(sourceValue, sourceProp.PropertyType, targetProp, targetVariable));
                }
            }

            // Copy fields - include non-public fields
            var sourceFields = sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var targetFields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var sourceField in sourceFields)
            {
                // Skip readonly fields and backing fields for properties
                if (sourceField.IsInitOnly || sourceField.Name.Contains("<"))
                    continue;

                var targetField = targetFields.FirstOrDefault(f =>
                    f.Name == sourceField.Name && !f.IsInitOnly);

                if (targetField != null)
                {
                    var sourceValue = Expression.Field(sourceVariable, sourceField);
                    expressions.Add(CreateFieldCopyExpression(sourceValue, sourceField.FieldType, targetField, targetVariable));
                }
            }

            // Return the target
            expressions.Add(targetVariable);

            var block = Expression.Block(
                new[] { sourceVariable, targetVariable },
                expressions
            );

            // Create lambda with explicit type conversion
            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(block, typeof(object)),
                sourceParam
            );
            return lambda.Compile();
        }

        private static Expression CreatePropertyCopyExpression(Expression sourceValue, Type sourceType, PropertyInfo targetProp, ParameterExpression targetVariable)
        {
            // For value types and strings, try direct assignment with conversion
            if (sourceType.IsValueType || sourceType == typeof(string))
            {
                // If types are directly compatible
                if (targetProp.PropertyType.IsAssignableFrom(sourceType))
                {
                    return Expression.Assign(
                        Expression.Property(targetVariable, targetProp),
                        Expression.Convert(sourceValue, targetProp.PropertyType)
                    );
                }
                // If target is nullable and source is non-nullable value type
                else if (IsNullableCompatible(sourceType, targetProp.PropertyType))
                {
                    return Expression.Assign(
                        Expression.Property(targetVariable, targetProp),
                        Expression.Convert(sourceValue, targetProp.PropertyType)
                    );
                }
            }

            // For all other cases (including different reference types), use DeepCopyInternal
            var copyCall = Expression.Call(
                typeof(ObjectCopyExtensions),
                nameof(DeepCopyInternal),
                null,
                Expression.Convert(sourceValue, typeof(object)),
                Expression.Constant(sourceType),
                Expression.Constant(targetProp.PropertyType)
            );

            return Expression.Assign(
                Expression.Property(targetVariable, targetProp),
                Expression.Convert(copyCall, targetProp.PropertyType)
            );
        }

        private static Expression CreateFieldCopyExpression(Expression sourceValue, Type sourceType, FieldInfo targetField, ParameterExpression targetVariable)
        {
            // For value types and strings, try direct assignment with conversion
            if (sourceType.IsValueType || sourceType == typeof(string))
            {
                // If types are directly compatible
                if (targetField.FieldType.IsAssignableFrom(sourceType))
                {
                    return Expression.Assign(
                        Expression.Field(targetVariable, targetField),
                        Expression.Convert(sourceValue, targetField.FieldType)
                    );
                }
                // If target is nullable and source is non-nullable value type
                else if (IsNullableCompatible(sourceType, targetField.FieldType))
                {
                    return Expression.Assign(
                        Expression.Field(targetVariable, targetField),
                        Expression.Convert(sourceValue, targetField.FieldType)
                    );
                }
            }

            // For all other cases (including different reference types), use DeepCopyInternal
            var copyCall = Expression.Call(
                typeof(ObjectCopyExtensions),
                nameof(DeepCopyInternal),
                null,
                Expression.Convert(sourceValue, typeof(object)),
                Expression.Constant(sourceType),
                Expression.Constant(targetField.FieldType)
            );

            return Expression.Assign(
                Expression.Field(targetVariable, targetField),
                Expression.Convert(copyCall, targetField.FieldType)
            );
        }

        private static bool IsNullableCompatible(Type sourceType, Type targetType)
        {
            var underlyingTarget = Nullable.GetUnderlyingType(targetType);
            return underlyingTarget != null && underlyingTarget == sourceType;
        }

        private static Expression CreateInstance(Type type)
        {
            if (type == typeof(string))
                return Expression.Constant(string.Empty);

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return Expression.NewArrayBounds(elementType, Expression.Constant(0));
            }

            // Try to find a parameterless constructor
            var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
            if (parameterlessConstructor != null)
                return Expression.New(parameterlessConstructor);

            // For types without parameterless constructors, use the simplest constructor
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var simplestConstructor = constructors.OrderBy(c => c.GetParameters().Length).FirstOrDefault();

            if (simplestConstructor != null)
            {
                var parameters = simplestConstructor.GetParameters();
                var parameterExpressions = parameters.Select(p =>
                    p.HasDefaultValue ?
                        Expression.Constant(p.DefaultValue, p.ParameterType) :
                        p.ParameterType.IsValueType ?
                            Expression.Constant(Activator.CreateInstance(p.ParameterType), p.ParameterType) :
                            Expression.Constant(null, p.ParameterType)
                ).ToArray();

                return Expression.New(simplestConstructor, parameterExpressions);
            }

            // Last resort: use Activator.CreateInstance
            var createInstanceMethod = typeof(Activator).GetMethod(
                nameof(Activator.CreateInstance),
                new[] { typeof(Type) }
            );

            return Expression.Call(createInstanceMethod, Expression.Constant(type));
        }

        private static object CopyArray(Array sourceArray, Type targetArrayType)
        {
            var elementType = targetArrayType.GetElementType();
            var length = sourceArray.Length;
            var targetArray = Array.CreateInstance(elementType, length);

            for (int i = 0; i < length; i++)
            {
                var sourceValue = sourceArray.GetValue(i);
                if (sourceValue != null)
                {
                    var targetValue = DeepCopyInternal(sourceValue, sourceValue.GetType(), elementType);
                    targetArray.SetValue(targetValue, i);
                }
                else
                {
                    targetArray.SetValue(null, i);
                }
            }

            return targetArray;
        }

        private static object CopyCollection(object source, Type sourceType, Type targetType)
        {
            var sourceCollection = (IEnumerable)source;

            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                var list = CreateList(sourceCollection, elementType);

                // Convert IList to array manually
                var array = Array.CreateInstance(elementType, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(list[i], i);
                }
                return array;
            }

            if (targetType.IsGenericType)
            {
                var genericType = targetType.GetGenericTypeDefinition();
                var elementType = targetType.GetGenericArguments()[0];

                if (genericType == typeof(List<>) || genericType == typeof(IList<>) || genericType == typeof(ICollection<>))
                {
                    return CreateList(sourceCollection, elementType);
                }

                // Handle HashSet<T>
                if (genericType == typeof(HashSet<>))
                {
                    var hashSetType = typeof(HashSet<>).MakeGenericType(elementType);
                    var hashSet = Activator.CreateInstance(hashSetType);
                    var addMethod = hashSetType.GetMethod("Add");

                    foreach (var item in sourceCollection)
                    {
                        if (item != null)
                        {
                            var copiedItem = DeepCopyInternal(item, item.GetType(), elementType);
                            addMethod.Invoke(hashSet, new[] { copiedItem });
                        }
                    }
                    return hashSet;
                }
            }

            // Default: try to create using Activator and add items
            try
            {
                var collection = Activator.CreateInstance(targetType);
                var addMethod = targetType.GetMethod("Add");

                if (addMethod != null)
                {
                    var elementType = addMethod.GetParameters()[0].ParameterType;
                    foreach (var item in sourceCollection)
                    {
                        if (item != null)
                        {
                            var copiedItem = DeepCopyInternal(item, item.GetType(), elementType);
                            addMethod.Invoke(collection, new[] { copiedItem });
                        }
                    }
                    return collection;
                }
            }
            catch
            {
                // If we can't create the specific collection type, fall back to List<object>
            }

            return CreateList(sourceCollection, typeof(object));
        }

        private static IList CreateList(IEnumerable source, Type elementType)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var item in source)
            {
                if (item != null)
                {
                    var copiedItem = DeepCopyInternal(item, item.GetType(), elementType);
                    list.Add(copiedItem);
                }
            }

            return list;
        }
    }
}