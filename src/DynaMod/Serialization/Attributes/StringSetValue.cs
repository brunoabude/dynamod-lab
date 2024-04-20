using System.Collections;
using System.Collections.Generic;

namespace DynaMod.Serialization.Attributes
{
    public class StringSetValue : AttributeValue, IEnumerable<StringValue>
    {
        private HashSet<StringValue> _stringSet;

        public StringSetValue() : base(DataTypeDescriptor.SS)
        {
            _stringSet = new HashSet<StringValue>();
        }

        public StringSetValue(IEnumerable<string> value) : base(DataTypeDescriptor.SS)
        {
            _stringSet = new HashSet<StringValue>();

            foreach (string i in value)
            {
                _stringSet.Add(new StringValue(i));
            }
        }

        public void Add(StringValue v)
        {
            _stringSet.Add(v);
        }

        public bool Contains(StringValue v)
        {
            return _stringSet.Contains(v);
        }

        public override object AsObject()
        {
            return _stringSet;
        }

        public IEnumerator<StringValue> GetEnumerator()
        {
            return _stringSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _stringSet.GetEnumerator();
        }
    }
}
