using System.Collections;
using System.Collections.Generic;

namespace DynaMod.Serialization.Attributes
{
    public class BinarySetValue : AttributeValue, IEnumerable<BinaryValue>
    {
        private HashSet<BinaryValue> _binarySet;

        public BinarySetValue() : base(DataTypeDescriptor.BS)
        {
            _binarySet = new HashSet<BinaryValue>();
        }

        public BinarySetValue(IEnumerable<IEnumerable<byte>> value) : base(DataTypeDescriptor.BS)
        {
            _binarySet = new HashSet<BinaryValue>();

            foreach (IEnumerable<byte> i in value)
            {
                _binarySet.Add(new BinaryValue(i));
            }
        }

        public void Add(BinaryValue value)
        {
            _binarySet.Add(value);
        }

        public bool Contains(BinaryValue binaryValue)
        {
            return _binarySet.Contains(binaryValue);
        }

        public override object AsObject()
        {
            return _binarySet;
        }

        public IEnumerator<BinaryValue> GetEnumerator()
        {
            return _binarySet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _binarySet.GetEnumerator();
        }
    }
}
