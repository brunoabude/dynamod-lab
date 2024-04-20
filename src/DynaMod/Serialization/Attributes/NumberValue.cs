using System;
using System.Collections.Generic;
using DynaMod.Serialization.Document;

namespace DynaMod.Serialization.Attributes
{
    public class NumberValue : AttributeValue
    {
        private readonly decimal _value;
        public decimal N { get { return _value; } }

        public NumberValue(decimal number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }
        public NumberValue(float number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(double number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(long number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(int number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(short number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(ulong number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(uint number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(ushort number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(byte number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(sbyte number) : base(DataTypeDescriptor.N)
        {
            _value = Convert.ToDecimal(number);
        }

        public NumberValue(string number) : base(DataTypeDescriptor.N)
        {
            DocumentSerializer serializer = DocumentSerializer.GetDefaultSerializer();
            _value = serializer.ParseDecimal(number);
        }

        public override bool Equals(object obj)
        {
            return obj is NumberValue number &&
                   attributeType == number.attributeType &&
                   _value == number._value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(attributeType, _value);
        }

        public override object AsObject()
        {
            return _value;
        }

        public static bool operator ==(NumberValue left, NumberValue right)
        {
            return EqualityComparer<NumberValue>.Default.Equals(left, right);
        }

        public static bool operator !=(NumberValue left, NumberValue right)
        {
            return !(left == right);
        }
    }
}
