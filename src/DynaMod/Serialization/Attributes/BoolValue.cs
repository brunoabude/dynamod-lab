using System;
using System.Collections.Generic;

namespace DynaMod.Serialization.Attributes
{
    public class BoolValue : AttributeValue
    {
        private bool _value;
        public bool BOOL { get { return _value; } }

        public BoolValue(bool value) : base(DataTypeDescriptor.BOOL)
        {
            _value = value;
        }

        public override object AsObject()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            return obj is BoolValue value &&
                   attributeType == value.attributeType &&
                   _value == value._value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(attributeType, _value);
        }

        public static bool operator ==(BoolValue left, BoolValue right)
        {
            return EqualityComparer<BoolValue>.Default.Equals(left, right);
        }

        public static bool operator !=(BoolValue left, BoolValue right)
        {
            return !(left == right);
        }
    }
}
