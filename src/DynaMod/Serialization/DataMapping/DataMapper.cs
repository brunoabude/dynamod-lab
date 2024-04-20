using DynaMod.Common;
using DynaMod.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using static DynaMod.Common.Assertions;

namespace DynaMod.Serialization.DataMapping
{
    internal class DataMapper
    {
        private static Dictionary<Type, DataMapper> _mappers;

        internal static DataMapper GetMapper(Type type)
        {
            _mappers ??= new Dictionary<Type, DataMapper>();

            if (!_mappers.ContainsKey(type))
            {
                _mappers[type] = new DataMapper(type);
            }

            return _mappers[type];
        }

        internal static readonly Dictionary<Type, DataTypeDescriptor> PrimitiveMap = new Dictionary<Type, DataTypeDescriptor>()
        {
            [typeof(long)] = DataTypeDescriptor.N,
            [typeof(int)] = DataTypeDescriptor.N,
            [typeof(short)] = DataTypeDescriptor.N,
            [typeof(ulong)] = DataTypeDescriptor.N,
            [typeof(uint)] = DataTypeDescriptor.N,
            [typeof(ushort)] = DataTypeDescriptor.N,

            [typeof(float)] = DataTypeDescriptor.N,
            [typeof(double)] = DataTypeDescriptor.N,
            [typeof(decimal)] = DataTypeDescriptor.N,

            [typeof(string)] = DataTypeDescriptor.S,
            [typeof(bool)] = DataTypeDescriptor.BOOL,
            [typeof(byte)] = DataTypeDescriptor.B,
            [typeof(byte[])] = DataTypeDescriptor.B,
        };

        internal static readonly HashSet<Type> NumberPrimitives = new HashSet<Type>
        {
            typeof(long),
            typeof(int),
            typeof(short),
            typeof(ulong),
            typeof(uint),
            typeof(ushort),
            typeof(float),
            typeof(decimal),
            typeof(double),
            typeof(byte),
            typeof(sbyte)
        };

        internal static readonly BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Type _type;
        private readonly Dictionary<string, MemberDataDefinition> _memberDatas;

        private DataMapper(Type type)
        {
            _type = type;
            _memberDatas = new Dictionary<string, MemberDataDefinition>();
        }

        internal static DataTypeDescriptor GetDataTypeDescriptor(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
            {
                return PrimitiveMap[type];
            }
            else if (Reflection.TryGetCollectionItemType(type, out CollectionType collectionType, out Type itemType))
            {
                if (itemType == typeof(byte))
                {
                    return DataTypeDescriptor.B;
                }
                else if (collectionType == CollectionType.Set && NumberPrimitives.Contains(itemType))
                {
                    return DataTypeDescriptor.NS;
                }
                else if (collectionType == CollectionType.Set && itemType == typeof(string))
                {
                    return DataTypeDescriptor.SS;
                }
                else if (collectionType == CollectionType.Set && (itemType == typeof(byte[]) || itemType == typeof(sbyte[])))
                {
                    return DataTypeDescriptor.BS;
                }
                else
                {
                    return DataTypeDescriptor.L;
                }
            }
            else
            {
                return DataTypeDescriptor.M;
            }
        }

        internal static AttributeValue Primitive(object value)
        {
            return GetDataTypeDescriptor(value.GetType()) switch
            {
                DataTypeDescriptor.S => new StringValue((string)value),
                DataTypeDescriptor.N => new NumberValue(Convert.ToDecimal(value)),
                DataTypeDescriptor.B => new BinaryValue((byte[])value),
                DataTypeDescriptor.BOOL => new BoolValue((bool)value),
                DataTypeDescriptor.NULL => new NullValue(),
                _ => throw new InvalidOperationException(),
            };
        }

        internal MemberDataDefinition? GetMemberData(string memberName)
        {
            if (_memberDatas.ContainsKey(memberName))
                return _memberDatas[memberName];

            MemberInfo[] member_info = _type.GetMember(memberName);

            if (member_info.Length == 0)
                return null;

            Assert(member_info.Length == 1, "Only one member is expected.");

            switch (member_info[0].MemberType)
            {
                case MemberTypes.Property:
                    {
                        PropertyInfo property_info = member_info[0]
                            .DeclaringType
                            .GetProperty(member_info[0].Name,
                                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        Assert(property_info != null, $"Could not find property{member_info[0].Name} at the declaring type {member_info[0].DeclaringType.Name}");

                        MemberDataDefinition member_data = new MemberDataDefinition { MemberType = property_info.PropertyType, Flags = 0 };

                        if (property_info.CanRead)
                        {
                            member_data.Flags |= MemberDataInfoFlags.CAN_READ;
                            member_data.GetMethod = property_info.GetMethod;
                        }

                        if (property_info.CanWrite)
                        {
                            member_data.Flags |= MemberDataInfoFlags.CAN_WRITE;
                            member_data.SetMethod = property_info.SetMethod;
                        }

                        Type property_type = property_info.PropertyType;


                        member_data.TargetDataType = GetDataTypeDescriptor(property_type);

                        _memberDatas[memberName] = member_data;
                        return member_data;
                    }
                default:
                    throw new InvalidOperationException($"Member type not supported {member_info[0].MemberType}");
            }
        }

        internal Dictionary<string, MemberDataDefinition> GetAllMemberDataDefinitions()
        {
            foreach (var property_info in _type.GetProperties(PropertyFlags))
            {
                _ = GetMemberData(property_info.Name);
            }

            return new Dictionary<string, MemberDataDefinition>(_memberDatas.ToList());
        }
    }
}
