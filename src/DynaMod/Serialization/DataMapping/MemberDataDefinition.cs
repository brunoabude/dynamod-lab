using DynaMod.Serialization.Attributes;
using System;
using System.Reflection;

namespace DynaMod.Serialization.DataMapping
{
    [Flags]
    internal enum MemberDataInfoFlags
    {
        DEFAULT = 0,
        FIELD = 1 << 1,
        PROPERTY = 1 << 2,
        CAN_READ = 1 << 3,
        CAN_WRITE = 1 << 4
    }

    internal struct MemberDataDefinition
    {
        internal MemberDataInfoFlags Flags;
        internal Type MemberType;
        internal MethodInfo GetMethod;
        internal MethodInfo SetMethod;
        internal DataTypeDescriptor TargetDataType;
    }
}
