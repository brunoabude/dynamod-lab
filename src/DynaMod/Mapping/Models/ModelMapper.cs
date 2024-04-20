using DynaMod.Common;
using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.DataMapping;
using System;
using System.Collections;
using System.Collections.Generic;


namespace DynaMod.Mapping.Models
{
    internal struct Collection
    {
        internal object instance;
        internal DataTypeDescriptor dataType;
        internal AttributeValue attr;
        internal IEnumerator<MapperCollectionMember> membersEnumerator;
    }

    internal struct MapperCollectionMember
    {
        internal DataTypeDescriptor dataType;
        internal string key;
        internal object value;
    }

    internal class ModelMapper
    {
        private Stack<Collection> stack = new Stack<Collection>();
        private HashSet<object> visited = new HashSet<object>();

        public MapValue Map(object instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            stack.Clear();
            visited.Clear();

            MapValue root = new MapValue();

            stack.Push(new Collection
            {
                instance = instance,
                dataType = DataTypeDescriptor.M,
                attr = root,
                membersEnumerator = GetEnumerator(instance)
            });

            while (stack.Count > 0)
            {
                Collection col = stack.Peek();
                visited.Add(col.instance);

                bool moveNext;

                do
                {
                    moveNext = col.membersEnumerator.MoveNext();

                    if (!moveNext)
                        break;

                    MapperCollectionMember member = col.membersEnumerator.Current;
                    AttributeValue attr;

                    switch (member.dataType)
                    {
                        case DataTypeDescriptor.S:
                            attr = new StringValue((string)member.value);
                            break;
                        case DataTypeDescriptor.N:
                            attr = new NumberValue(Convert.ToDecimal(member.value));
                            break;
                        case DataTypeDescriptor.B:
                            if (member.value.GetType() == typeof(byte))
                                attr = new BinaryValue(new byte[] { (byte)member.value });
                            else if (member.value.GetType() == typeof(byte[]))
                                attr = new BinaryValue((byte[])member.value);
                            else
                                throw new InvalidOperationException();
                            break;
                        case DataTypeDescriptor.BOOL:
                            attr = new BoolValue((bool)member.value);
                            break;
                        case DataTypeDescriptor.NULL:
                            attr = new NullValue();
                            break;
                        case DataTypeDescriptor.SS:
                            attr = new StringSetValue();
                            break;
                        case DataTypeDescriptor.BS:
                            attr = new BinarySetValue();
                            break;
                        case DataTypeDescriptor.L:
                            attr = new ListValue();
                            break;
                        case DataTypeDescriptor.M:
                            attr = new MapValue();
                            break;
                        case DataTypeDescriptor.NS:
                            attr = new NumberSetValue();
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    if (col.dataType == DataTypeDescriptor.M)
                    {
                        if (visited.Contains(member.value))
                            throw new InvalidOperationException("Circular reference detected.");

                        ((MapValue)col.attr)[member.key] = attr;
                    }
                    else if (col.dataType == DataTypeDescriptor.L)
                    {
                        if (visited.Contains(member.value))
                            throw new InvalidOperationException("Circular reference detected.");

                        ((ListValue)col.attr).Add(attr);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    switch (member.dataType)
                    {
                        case DataTypeDescriptor.BS:
                        case DataTypeDescriptor.SS:
                        case DataTypeDescriptor.NS:
                        case DataTypeDescriptor.M:
                        case DataTypeDescriptor.L:
                            stack.Push(new Collection
                            {
                                instance = member.value,
                                dataType = member.dataType,
                                attr = attr,
                                membersEnumerator = GetEnumerator(member.value)
                            });
                            goto BREAK;
                        default:
                            break;
                    }
                } while (moveNext);

            BREAK:

                if (!moveNext)
                {
                    stack.Pop();
                    visited.Remove(col.instance);
                }
            }

            return root;
        }

        private IEnumerator<MapperCollectionMember> GetEnumerator(object instance)
        {
            if (Reflection.TryGetCollectionItemType(instance.GetType(), out CollectionType collectionType, out Type itemType))
            {
                List<MapperCollectionMember> members = new List<MapperCollectionMember>();

                foreach (object e in (ICollection)instance)
                {
                    members.Add(new MapperCollectionMember
                    {
                        dataType = e == null ? DataTypeDescriptor.NULL : DataMapper.GetDataTypeDescriptor(e.GetType()),
                        value = e
                    });
                }

                return members.GetEnumerator();
            }
            else if (Reflection.TryGetDictionaryTKey(instance.GetType(), out Type tKey))
            {
                if (tKey != typeof(string))
                    throw new InvalidOperationException($"Dictionary members must have string as key");

                List<MapperCollectionMember> members = new List<MapperCollectionMember>();

                foreach (DictionaryEntry e in (IDictionary)instance)
                {
                    members.Add(new MapperCollectionMember
                    {
                        dataType = e.Value == null ? DataTypeDescriptor.NULL : DataMapper.GetDataTypeDescriptor(e.Value.GetType()),
                        value = e.Value,
                        key = (string)e.Key
                    });
                }

                return members.GetEnumerator();
            }
            else
            {
                DataMapper dataMapper = DataMapper.GetMapper(instance.GetType());
                Dictionary<string, MemberDataDefinition> memberDataDefinitions = dataMapper.GetAllMemberDataDefinitions();

                List<MapperCollectionMember> members = new List<MapperCollectionMember>();

                foreach ((string key, MemberDataDefinition memberDef) in memberDataDefinitions)
                {
                    if ((memberDef.Flags & MemberDataInfoFlags.CAN_READ) == 0)
                        continue;

                    object value = memberDef.GetMethod.Invoke(instance, null);

                    members.Add(new MapperCollectionMember
                    {
                        dataType = value == null ? DataTypeDescriptor.NULL : DataMapper.GetDataTypeDescriptor(value.GetType()),
                        value = value,
                        key = key
                    });
                }

                return members.GetEnumerator();
            }
        }
    }
}
