using System.Collections;
using System.Collections.Generic;

namespace DynaMod.Serialization.Attributes
{
    public class ListValue : AttributeValue, IEnumerable<AttributeValue>
    {
        private readonly List<AttributeValue> _list;

        public ListValue() : base(DataTypeDescriptor.L)
        {
            _list = new List<AttributeValue>();
        }

        public ListValue(List<AttributeValue> list) : base(DataTypeDescriptor.L)
        {
            _list = new List<AttributeValue>(list);

            foreach (var i in list)
            {
                _list.Add(i);
            }
        }

        public void Add(AttributeValue value)
        {
            _list.Add(value);
        }

        public override object AsObject()
        {
            return _list;
        }

        public IEnumerator<AttributeValue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
