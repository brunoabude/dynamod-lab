using DynaMod.Common;
using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.DataMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using static DynaMod.Common.Assertions;

namespace DynaMod.Mapping.Models
{
    internal struct CollectionMember
    {
        internal object container;
        internal DataTypeDescriptor containerDataType;
        internal AttributeValue attribute;
    }

    internal class ModelLoader
    {
        private List<CollectionMember> dfsList = new List<CollectionMember>();

        private static object CreateCollection(Type t)
        {
            Assert(Reflection.TryGetCollectionItemType(t, out var collectionType, out var itemType), "Invalid collection type");

            if (!t.IsInterface && !t.IsAbstract)
            {
                return Activator.CreateInstance(t);
            }
            else if (!t.IsInterface && t.IsAbstract)
            {
                throw new NotImplementedException();
            }
            else if (t.IsInterface)
            {
                switch (collectionType)
                {
                    case CollectionType.List:
                        return Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                    case CollectionType.Set:
                        return Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(itemType));
                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static MethodInfo GetCollectionAddMethod(Type t)
        {
            Assert(Reflection.TryGetCollectionItemType(t, out var collectionType, out var itemType), "Invalid collection type");

            if (collectionType == CollectionType.Set)
            {
                return t.GetMethod(nameof(ISet<object>.Add), new Type[] { itemType });
            }
            else if (collectionType == CollectionType.List)
            {
                return t.GetMethod(nameof(ICollection<object>.Add), new Type[] { itemType });
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public object Load(Type t, MapValue document)
        {
            object instance = FormatterServices.GetUninitializedObject(t);

            dfsList.Clear();

            dfsList.Add(new CollectionMember
            {
                container = instance,
                attribute = document,
                containerDataType = DataTypeDescriptor.M
            });

            List<CollectionMember> newMembers = new List<CollectionMember>();

            while (dfsList.Count > 0)
            {
                var element = dfsList[dfsList.Count - 1];
                dfsList.RemoveAt(dfsList.Count - 1);

                if (element.containerDataType == DataTypeDescriptor.M && !Reflection.TryGetDictionaryTKey(element.container.GetType(), out var _))
                {
                    DataMapper dataMapper = DataMapper.GetMapper(element.container.GetType());
                    Dictionary<string, MemberDataDefinition> memberDefinitions = dataMapper.GetAllMemberDataDefinitions();

                    newMembers.Clear();

                    foreach (var (memberName, memberDefinition) in memberDefinitions)
                    {
                        MapValue map = (MapValue)element.attribute;

                        if (!map.ContainsKey(memberName))
                            continue;

                        AttributeValue attribute = map[memberName];

                        Assert(attribute.attributeType == memberDefinition.TargetDataType, "Invalid document");

                        switch (memberDefinition.TargetDataType)
                        {
                            case DataTypeDescriptor.S:
                                memberDefinition.SetMethod.Invoke(element.container, new object[] { ((StringValue)attribute).S });
                                break;
                            case DataTypeDescriptor.N:
                                var n = ((NumberValue)attribute).N;
                                if (memberDefinition.MemberType == typeof(long))
                                {
                                    memberDefinition.SetMethod.Invoke(element.container, new object[] { (long)n });
                                }
                                else if (memberDefinition.MemberType == typeof(int))
                                {
                                    memberDefinition.SetMethod.Invoke(element.container, new object[] { (int)n });
                                }
                                else if (memberDefinition.MemberType == typeof(short))
                                {
                                    memberDefinition.SetMethod.Invoke(element.container, new object[] { (int)n });
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }

                                break;
                            case DataTypeDescriptor.B:
                                memberDefinition.SetMethod.Invoke(element.container, new object[] { ((BinaryValue)attribute).B });
                                break;
                            case DataTypeDescriptor.BOOL:
                                memberDefinition.SetMethod.Invoke(element.container, new object[] { ((BoolValue)attribute).BOOL });
                                break;
                            case DataTypeDescriptor.NULL:
                                memberDefinition.SetMethod.Invoke(element.container, new object[] { null });
                                break;
                            case DataTypeDescriptor.SS:
                            case DataTypeDescriptor.BS:
                            case DataTypeDescriptor.NS:
                            case DataTypeDescriptor.L:
                                {
                                    object mInstance = CreateCollection(memberDefinition.MemberType);

                                    newMembers.Add(new CollectionMember
                                    {
                                        container = mInstance,
                                        attribute = attribute,
                                        containerDataType = DataTypeDescriptor.L
                                    });

                                    memberDefinition.SetMethod.Invoke(element.container, new object[] { mInstance });

                                    break;
                                }
                            case DataTypeDescriptor.M:
                                {
                                    object mInstance = FormatterServices.GetUninitializedObject(memberDefinition.MemberType);

                                    newMembers.Add(new CollectionMember
                                    {
                                        container = mInstance,
                                        attribute = attribute,
                                        containerDataType = DataTypeDescriptor.M
                                    });

                                    memberDefinition.SetMethod.Invoke(element.container, new object[] { mInstance });
                                    break;
                                }
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                }
                else if (element.containerDataType == DataTypeDescriptor.L)
                {
                    Assert(Reflection.TryGetCollectionItemType(element.container.GetType(), out var collectionType, out var itemType), "Invalid collection type");
                    MethodInfo collectionAddMethod = GetCollectionAddMethod(element.container.GetType());
                    DataTypeDescriptor expectedType = DataMapper.GetDataTypeDescriptor(itemType);
                    ListValue list = (ListValue)element.attribute;

                    newMembers.Clear();

                    foreach (AttributeValue attribute in list)
                    {
                        Assert(attribute.attributeType == expectedType, "Invalid document");

                        switch (attribute.attributeType)
                        {
                            case DataTypeDescriptor.S:
                                collectionAddMethod.Invoke(element.container, new object[] { ((StringValue)attribute).S });
                                break;
                            case DataTypeDescriptor.N:
                                var n = ((NumberValue)attribute).N;
                                if (itemType == typeof(long))
                                {
                                    collectionAddMethod.Invoke(element.container, new object[] { (long)n });
                                }
                                else if (itemType == typeof(int))
                                {
                                    collectionAddMethod.Invoke(element.container, new object[] { (int)n });
                                }
                                else if (itemType == typeof(short))
                                {
                                    collectionAddMethod.Invoke(element.container, new object[] { (int)n });
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }

                                break;
                            case DataTypeDescriptor.B:
                                collectionAddMethod.Invoke(element.container, new object[] { ((BinaryValue)attribute).B });
                                break;
                            case DataTypeDescriptor.BOOL:
                                collectionAddMethod.Invoke(element.container, new object[] { ((BoolValue)attribute).BOOL });
                                break;
                            case DataTypeDescriptor.NULL:
                                collectionAddMethod.Invoke(element.container, new object[] { null });
                                break;
                            case DataTypeDescriptor.L:
                                {
                                    object mInstance = CreateCollection(itemType);

                                    newMembers.Add(new CollectionMember
                                    {
                                        container = mInstance,
                                        attribute = attribute,
                                        containerDataType = DataTypeDescriptor.L
                                    });

                                    collectionAddMethod.Invoke(element.container, new object[] { mInstance });

                                    break;
                                }
                            case DataTypeDescriptor.SS:
                            case DataTypeDescriptor.BS:
                            case DataTypeDescriptor.NS:
                                continue;
                            case DataTypeDescriptor.M:
                                {
                                    object mInstance = FormatterServices.GetUninitializedObject(itemType);

                                    newMembers.Add(new CollectionMember
                                    {
                                        container = mInstance,
                                        attribute = attribute,
                                        containerDataType = DataTypeDescriptor.M
                                    });

                                    collectionAddMethod.Invoke(element.container, new object[] { mInstance });
                                    break;
                                }
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                }
                else if (element.containerDataType == DataTypeDescriptor.M && Reflection.TryGetDictionaryTKey(element.container.GetType(), out var tKey, out var tvalue))
                {
                    DataMapper dataMapper = DataMapper.GetMapper(element.container.GetType());
                    Dictionary<string, MemberDataDefinition> memberDefinitions = dataMapper.GetAllMemberDataDefinitions();
                    var expectedType = DataMapper.GetDataTypeDescriptor(tvalue);

                    newMembers.Clear();

                    Assert(tKey == typeof(string), "Only dicts with string key..");

                    MapValue map = (MapValue)element.attribute;

                    var itemPropertyInfo = element.container.GetType().GetProperty("Item");

                    foreach (var (memberName, attribute) in map)
                    {
                        Assert(expectedType == attribute.attributeType, "Invalid element in map");

                        switch (expectedType)
                        {
                            case DataTypeDescriptor.S:
                                itemPropertyInfo.SetValue(element.container, ((StringValue)attribute).S, new[] { memberName });
                                break;
                            case DataTypeDescriptor.N:
                                var n = ((NumberValue)attribute).N;
                                if (tvalue == typeof(long))
                                {
                                    itemPropertyInfo.SetValue(element.container, (long)n, new[] { memberName });
                                }
                                else if (tvalue == typeof(int))
                                {
                                    itemPropertyInfo.SetValue(element.container, (int)n, new[] { memberName });
                                }
                                else if (tvalue == typeof(short))
                                {
                                    itemPropertyInfo.SetValue(element.container, (short)n, new[] { memberName });
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }

                                break;
                            case DataTypeDescriptor.B:
                                itemPropertyInfo.SetValue(element.container, ((BinaryValue)attribute).B, new[] { memberName });
                                break;
                            case DataTypeDescriptor.BOOL:
                                itemPropertyInfo.SetValue(element.container, ((BoolValue)attribute).BOOL, new[] { memberName });
                                break;
                            case DataTypeDescriptor.NULL:
                                itemPropertyInfo.SetValue(element.container, null, new[] { memberName });
                                break;
                            case DataTypeDescriptor.L:
                                {
                                    object mInstance = CreateCollection(tvalue);

                                    newMembers.Add(new CollectionMember
                                    {
                                        container = mInstance,
                                        attribute = attribute,
                                        containerDataType = DataTypeDescriptor.L
                                    });

                                    itemPropertyInfo.SetValue(element.container, mInstance, new[] { memberName });

                                    break;
                                }
                            case DataTypeDescriptor.SS:
                            case DataTypeDescriptor.BS:
                            case DataTypeDescriptor.NS:
                                continue;
                            case DataTypeDescriptor.M:
                                {
                                    object mInstance = FormatterServices.GetUninitializedObject(tvalue);

                                    newMembers.Add(new CollectionMember
                                    {
                                        container = mInstance,
                                        attribute = attribute,
                                        containerDataType = DataTypeDescriptor.M
                                    });

                                    itemPropertyInfo.SetValue(element.container, mInstance, new[] { memberName });
                                    break;
                                }
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                }

                if (newMembers.Count > 0)
                {
                    dfsList.AddRange(newMembers.Reverse<CollectionMember>());
                    newMembers.Clear();
                }
            }

            return instance;
        }
    }
}
