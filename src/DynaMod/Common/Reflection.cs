using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynaMod.Common
{
    internal enum CollectionType
    {
        None = 0,
        Set = 1,
        List = 2
    }

    internal class Reflection
    {
        internal static MethodInfo GetGetter(Type type, string propertyName)
        {
            Type _t = type;

            while (_t != null)
            {
                var prop = _t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _t = _t.BaseType;

                if (prop is null)
                    continue;

                var getter = prop.GetGetMethod(true);

                if (getter != null)
                {
                    return getter;
                }
            }

            return null;
        }

        internal static MethodInfo GetSetter(Type type, string propertyName)
        {
            Type _t = type;

            while (_t != null)
            {
                var prop = _t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _t = _t.BaseType;

                if (prop is null)
                    continue;

                var getter = prop.GetSetMethod(true);

                if (getter != null)
                {
                    return getter;
                }

            }

            return null;
        }

        internal static bool TryGetCollectionItemType(Type containerType, out CollectionType collectionType, out Type itemType)
        {
            collectionType = CollectionType.None;
            itemType = null;

            Type[] implementedInterfaces = containerType.GetInterfaces();

            IEnumerable<Type> typeChain = implementedInterfaces;

            if (containerType.IsInterface)
            {
                typeChain = typeChain.Append(containerType);
            }

            foreach (Type i in typeChain)
            {
                // This will be our fallback, but we need to specifically also check for sets
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    collectionType = CollectionType.List;
                    itemType = i.GetGenericArguments()[0];
                }

                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISet<>))
                {
                    collectionType = CollectionType.Set;
                    itemType = i.GetGenericArguments()[0];
                    return true;
                }

                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    collectionType = CollectionType.List;
                    itemType = i.GetGenericArguments()[0];
                    return true;
                }

                // We need to specifically check for this because dictionary also implementes ICollection
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                        i == typeof(IDictionary))
                {
                    collectionType = CollectionType.None;
                    itemType = null;
                    return false;
                }
            }


            return collectionType != CollectionType.None;
        }

        internal static bool TryGetDictionaryTKey(Type containerType, out Type tKey)
        {
            foreach (Type i in containerType.GetInterfaces())
            {
                // We need to specifically check for this because dictionary also implementes ICollection
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                     i == typeof(IDictionary))
                {
                    tKey = i.GetGenericArguments()[0];
                    return true;
                }
            }

            tKey = null;
            return false;
        }

        internal static bool TryGetDictionaryTKey(Type containerType, out Type tKey, out Type tvalue)
        {
            foreach (Type i in containerType.GetInterfaces())
            {
                // We need to specifically check for this because dictionary also implementes ICollection
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                     i == typeof(IDictionary))
                {
                    tKey = i.GetGenericArguments()[0];
                    tvalue = i.GetGenericArguments()[1];
                    return true;
                }
            }

            tKey = null;
            tvalue = null;
            return false;
        }
    }
}
