using System.Collections;
using System.Collections.Generic;

namespace DynaMod.Serialization.Attributes
{
    public class NumberSetValue : AttributeValue, IEnumerable<NumberValue>
    {
        private HashSet<NumberValue> _numberSet;

        public NumberSetValue() : base(DataTypeDescriptor.NS)
        {
            _numberSet = new HashSet<NumberValue>();
        }

        public NumberSetValue(HashSet<NumberValue> value) : base(DataTypeDescriptor.NS)
        {
            _numberSet = new HashSet<NumberValue>();

            foreach (var i in value)
            {
                _numberSet.Add(i);
            }
        }

        public void Add(NumberValue value)
        {
            _numberSet.Add(value);
        }

        public bool Contains(NumberValue value)
        {
            return _numberSet.Contains(value);
        }

        public override object AsObject()
        {
            return _numberSet;
        }

        public IEnumerator<NumberValue> GetEnumerator()
        {
            return _numberSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _numberSet.GetEnumerator();
        }
    }
}
