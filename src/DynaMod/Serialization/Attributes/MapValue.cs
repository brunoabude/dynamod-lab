using DynaMod.Serialization.Document;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections;
using System.Collections.Generic;

namespace DynaMod.Serialization.Attributes
{
    public class MapValue : AttributeValue, IEnumerable<KeyValuePair<string, AttributeValue>>
    {
        private readonly Dictionary<string, AttributeValue> _attributes;

        public MapValue() : base(DataTypeDescriptor.M)
        {
            _attributes = new Dictionary<string, AttributeValue>();
        }

        public AttributeValue this[string key]
        {
            get { return _attributes[key]; }
            set { _attributes[key] = value; }
        }

        public bool ContainsKey(string key)
        {
            return _attributes.ContainsKey(key);
        }

        public override object AsObject()
        {
            return _attributes;
        }

        public IEnumerator<KeyValuePair<string, AttributeValue>> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        public static MapValue Load(JsonReader json_reader)
        {
            return new DocumentParser(json_reader).Parse();
        }
    }
}
