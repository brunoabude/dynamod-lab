using System;
using System.Collections.Generic;
using System.Linq;

namespace DynaMod.Serialization.Attributes
{
    public class BinaryValue : AttributeValue
    {
        private byte[] _value;
        public byte[] B { get { return _value; } }

        public BinaryValue(IEnumerable<byte> value) : base(DataTypeDescriptor.B)
        {
            _value = value.ToArray();
        }

        public override object AsObject()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BinaryValue other))
            {
                return false;
            }

            ReadOnlySpan<byte> b1 = _value;
            ReadOnlySpan<byte> b2 = other._value;

            return b1.SequenceEqual(b2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(attributeType, _value);
        }

        public static bool operator ==(BinaryValue left, BinaryValue right)
        {
            return EqualityComparer<BinaryValue>.Default.Equals(left, right);
        }

        public static bool operator !=(BinaryValue left, BinaryValue right)
        {
            return !(left == right);
        }
    }
}
