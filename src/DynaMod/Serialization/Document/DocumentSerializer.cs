using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using DynaMod.Serialization.Document;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DynaMod.Mapping.Fluent.Fields;

namespace DynaMod.Serialization.Document
{
    public class DocumentSerializer
    {
        private static IFormatProvider _defaultProvider = null;

        internal static IFormatProvider GetDefaultNumberFormatProvider()
        {
            if (_defaultProvider != null)
                return _defaultProvider;

            CultureInfo best = null;
            // TODO USE a custom format. For now, lets select the one that suits dynamos in theb est way
            foreach (var c in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (c.NumberFormat.NumberDecimalSeparator == "." &&
                    c.NumberFormat.NegativeSign == "-")
                {
                    if (best != null)
                    {
                        if (c.NumberFormat.NumberDecimalDigits > best.NumberFormat.NumberDecimalDigits)
                        {
                            best = c;
                        }
                    }
                    else
                    {
                        best = c;
                    }

                }
            }

            if (best is null)
                throw new InvalidOperationException($"Could not find any suitable format providers");
            _defaultProvider = best;
            return best;
        }


        internal AttributeValue ToAttribute(DataTypeDescriptor attrType, object value)
        {
            switch (attrType)
            {
                case DataTypeDescriptor.N:
                    decimal dValue = Convert.ToDecimal(value);

                    return new NumberValue(dValue);
                case DataTypeDescriptor.S:
                    return new StringValue(value?.ToString());
                default:
                    throw new NotImplementedException($"Attribute type {attrType} is not implemented");
            }
        }

        internal AttributeValue ToAttribute(DataTypeDescriptor attrType, FluentExpression mapper, object entity)
        {
            return ToAttribute(attrType, mapper.Apply(entity));
        }

        internal object ToValue(Type targetType, object value)
        {
            if (targetType == typeof(string))
            {
                if (value is null)
                    return null;

                return value.ToString();
            }

            if (targetType == typeof(ulong))
            {
                if (value is null)
                    return 0;

                return Convert.ToUInt64(value);
            }

            if (targetType == typeof(uint))
            {
                if (value is null)
                    return 0;

                return Convert.ToUInt32(value);
            }

            if (targetType == typeof(ushort))
            {
                if (value is null)
                    return 0;

                return Convert.ToUInt16(value);
            }

            if (targetType == typeof(long))
            {
                if (value is null)
                    return 0;

                return Convert.ToInt64(value);
            }

            if (targetType == typeof(int))
            {
                if (value is null)
                    return 0;

                return Convert.ToInt32(value);
            }

            if (targetType == typeof(short))
            {
                if (value is null)
                    return 0;

                return Convert.ToInt16(value);
            }

            if (targetType == typeof(decimal))
            {
                if (value is null)
                    return 0.0M;

                return Convert.ToDecimal(value);
            }

            if (targetType == typeof(float))
            {
                if (value is null)
                    return 0.0f;

                return Convert.ToDouble(value);
            }

            if (targetType == typeof(double))
            {
                if (value is null)
                    return 0.0;

                return Convert.ToDouble(value);
            }

            throw new NotImplementedException($"Type {targetType.Name} not implemented");
        }

        internal readonly struct Container
        {
            // Map Reference, List reference. Reference is important to kee
            internal readonly AttributeValue u;

            internal readonly IEnumerator<KeyValuePair<string, AttributeValue>> map_enumerator;
            internal readonly IEnumerator<AttributeValue> list_enumerator;

            internal Container(AttributeValue value)
            {
                u = value;

                switch (u.attributeType)
                {
                    case DataTypeDescriptor.M:
                        map_enumerator = ((MapValue)value).GetEnumerator();
                        list_enumerator = null;
                        break;
                    case DataTypeDescriptor.L:
                        list_enumerator = ((ListValue)value).GetEnumerator();
                        map_enumerator = null;
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected container type");
                }
            }
        }

        public static void WriteMapValue(JsonWriter writer, MapValue document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var format_provider = GetDefaultNumberFormatProvider();

            Stack<Container> containers = new Stack<Container>();
            HashSet<object> visited = new HashSet<object>();

            containers.Push(new Container(document));

            writer.WriteObjectStart();

            while (containers.Count > 0)
            {
                Container c = containers.Peek();
                visited.Add(c.u);

                bool has_advanced;

                do
                {
                    string key = null;
                    AttributeValue attr = null;

                    if (c.u.attributeType == DataTypeDescriptor.M)
                    {
                        has_advanced = c.map_enumerator.MoveNext();

                        if (has_advanced)
                        {
                            key = c.map_enumerator.Current.Key;
                            attr = c.map_enumerator.Current.Value;

                            writer.WritePropertyName(key);
                        }

                    }
                    else if (c.u.attributeType == DataTypeDescriptor.L)
                    {
                        has_advanced = c.list_enumerator.MoveNext();

                        if (has_advanced)
                            attr = c.list_enumerator.Current;
                    }
                    else
                    {
                        throw new Exception();
                    }

                    if (!has_advanced)
                    {
                        visited.Remove(c.u);
                        containers.Pop();

                        if (c.u.attributeType == DataTypeDescriptor.M)
                        {
                            writer.WriteObjectEnd();

                            // Prevents the root object from writing an additional } when popping from stack
                            if (containers.Count > 0)
                                writer.WriteObjectEnd();
                        }

                        if (c.u.attributeType == DataTypeDescriptor.L)
                        {
                            writer.WriteArrayEnd();
                            writer.WriteObjectEnd();
                        }

                        break;
                    }

                    switch (attr.attributeType)
                    {
                        case DataTypeDescriptor.N:
                            {
                                WriteNumberValue(writer, (NumberValue)attr, format_provider);
                                break;
                            }
                        case DataTypeDescriptor.S:
                            {
                                WriteStringValue(writer, (StringValue)attr);
                                break;
                            }
                        case DataTypeDescriptor.NULL:
                            {
                                WriteNullValue(writer);
                                break;
                            }
                        case DataTypeDescriptor.BOOL:
                            {
                                WriteBoolValue(writer, (BoolValue)attr);
                                break;
                            }
                        case DataTypeDescriptor.B:
                            {
                                WriteBinaryValue(writer, (BinaryValue)attr);
                                break;
                            }
                        case DataTypeDescriptor.SS:
                            {
                                WriteStringSetValue(writer, (IEnumerable<string>)(StringSetValue)attr);
                                break;
                            }
                        case DataTypeDescriptor.NS:
                            {
                                WriteNumberSet(writer, (IEnumerable<decimal>)(NumberSetValue)attr, format_provider);
                                break;
                            }
                        case DataTypeDescriptor.BS:
                            {
                                WriteBinarySetValue(writer, (IEnumerable<byte[]>)(BinarySetValue)attr);
                                break;
                            }
                        case DataTypeDescriptor.M:
                            {
                                if (visited.Contains(attr))
                                    throw new InvalidOperationException("Circular reference detected");

                                containers.Push(new Container(attr));
                                writer.WriteObjectStart();
                                writer.WritePropertyName("M");
                                writer.WriteObjectStart();
                                has_advanced = false;
                                break;
                            }
                        case DataTypeDescriptor.L:
                            {
                                if (visited.Contains(attr))
                                    throw new InvalidOperationException("Circular reference detected");

                                containers.Push(new Container(attr));
                                writer.WriteObjectStart();
                                writer.WritePropertyName("L");
                                writer.WriteArrayStart();
                                has_advanced = false;
                                break;
                            }
                        default:
                            throw new NotImplementedException($"Type {attr.attributeType} is not implemented.");
                    }
                } while (has_advanced);
            }
        }

        private static void WriteBinarySetValue(JsonWriter writer, IEnumerable<byte[]> bytes)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("BS");
            writer.WriteArrayStart();

            foreach (var b in bytes)
            {
                var base64 = Convert.ToBase64String(b);
                writer.Write(base64);
            }


            writer.WriteArrayEnd();
            writer.WriteObjectEnd();
        }

        private static void WriteNumberSet(JsonWriter writer, IEnumerable<decimal> numbers, IFormatProvider format_provider=null)
        {
            format_provider ??= GetDefaultNumberFormatProvider();

            writer.WriteObjectStart();
            writer.WritePropertyName("NS");
            writer.WriteArrayStart();
            foreach (var number in numbers)
            {
                writer.Write(number.ToString(format_provider));
            }

            writer.WriteArrayEnd();
            writer.WriteObjectEnd();
        }

        private static void WriteStringSetValue(JsonWriter writer, IEnumerable<string> strings)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("SS");
            writer.WriteArrayStart();

            foreach (var str in strings)
            {
                writer.Write(str);
            }

            writer.WriteArrayEnd();
            writer.WriteObjectEnd();
        }

        public static void WriteBinaryValue(JsonWriter writer, BinaryValue b)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("B");
            var base64 = Convert.ToBase64String(b.B);
            writer.Write(base64);
            writer.WriteObjectEnd();
        }

        public static void WriteBoolValue(JsonWriter writer, BoolValue value)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("BOOL");
            writer.Write(value.BOOL);
            writer.WriteObjectEnd();
        }

        public static void WriteNullValue(JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("NULL");
            writer.Write(true);
            writer.WriteObjectEnd();
        }

        public static void WriteStringValue(JsonWriter writer, StringValue attr)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("S");
            string value = ((StringValue)attr).S;
            writer.Write(value);
            writer.WriteObjectEnd();
        }

        public static void WriteNumberValue(JsonWriter writer, NumberValue attr, IFormatProvider format_provider=null)
        {
            if (format_provider == null)
            {
                format_provider = GetDefaultNumberFormatProvider();
            }

            writer.WriteObjectStart();
            writer.WritePropertyName("N");
            decimal value = attr.N;
            writer.Write(value.ToString(format_provider));
            writer.WriteObjectEnd();
        }

        public MapValue FromDynamoJson(string json)
        {
            return new DocumentParser(json).Parse();
        }

        internal static DocumentSerializer GetDefaultSerializer()
        {
            return new DocumentSerializer();
        }

        internal decimal ParseDecimal(string value)
        {
            IFormatProvider formatter = GetDefaultNumberFormatProvider();
            return Convert.ToDecimal(value, formatter);
        }

        internal bool TryParseDecimal(string value, out decimal result)
        {
            IFormatProvider formatter = GetDefaultNumberFormatProvider();
            bool parseResult = decimal.TryParse(value, NumberStyles.Any, formatter, out decimal r);
            result = r;
            return parseResult;
        }
    }
}
