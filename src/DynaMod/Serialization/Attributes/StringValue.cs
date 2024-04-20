using System;

namespace DynaMod.Serialization.Attributes
{
    public class StringValue : AttributeValue
    {
        private readonly string _value;
        public string S { get { return _value; } }

        public StringValue(string value) : base(DataTypeDescriptor.S)
        {
            _value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is StringValue @string &&
                   attributeType == @string.attributeType &&
                   _value == @string._value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(attributeType, _value);
        }

        public override object AsObject()
        {
            return _value;
        }
    }
}
