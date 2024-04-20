using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using static DynaMod.Common.Assertions;

namespace DynaMod.Serialization.Document
{
    [Serializable]
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException() { }
        public InvalidSyntaxException(string message) : base(message) { }
        public InvalidSyntaxException(string message, Exception inner) : base(message, inner) { }
        protected InvalidSyntaxException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }

    internal enum DynamoTokenType
    {
        KEY,
        TYPE,
        SCALAR
    }

    internal struct DynamoToken
    {
        internal DynamoTokenType tokenType;
        internal string value;

        public DynamoToken(DynamoTokenType tokenType, string value)
        {
            this.tokenType = tokenType;
            this.value = value;
        }
    }

    internal enum State
    {
        START,
        READING_MAP,
        READING_LIST,
        DONE,
        FAILED
    }

    internal class DocumentParser
    {
        private readonly Stack<DynamoToken> token_stack;
        private readonly Stack<AttributeValue> attribute_stack;
        private readonly JsonReader json_reader;

        private State state;

        internal DocumentParser(string json)
        {
            state = State.START;
            token_stack = new Stack<DynamoToken>();
            attribute_stack = new Stack<AttributeValue>();
            json_reader = new JsonReader(json);
        }

        internal DocumentParser(JsonReader json_reader)
        {
            state = State.START;
            token_stack = new Stack<DynamoToken>();
            attribute_stack = new Stack<AttributeValue>();
            this.json_reader = json_reader;
        }

        internal MapValue Parse()
        {
            switch (state)
            {
                case State.DONE: return (MapValue)attribute_stack.Peek();
                case State.FAILED: return null;
                case State.START: break;
                default: throw new InvalidOperationException();
            }
            Assert(json_reader.Token == JsonToken.ObjectStart || read_token() == JsonToken.ObjectStart);

            attribute_stack.Push(new MapValue());

            state = State.READING_MAP;

            while (!json_reader.EndOfJson)
            {
                switch (state)
                {
                    case State.READING_MAP: read_map(); break;
                    case State.READING_LIST: read_list(); break;
                    case State.DONE: return (MapValue)attribute_stack.Peek();
                    case State.FAILED: return null;
                    default: throw new InvalidOperationException();
                }
            }

            AssertSyntax(attribute_stack.Count == 1 && attribute_stack.Peek().attributeType == DataTypeDescriptor.M, "invalid document");
            return (MapValue)attribute_stack.Peek();
        }

        internal void read_map()
        {
            switch (read_token())
            {
                case JsonToken.PropertyName:
                    {
                        token_stack.Push(new DynamoToken(DynamoTokenType.KEY, json_reader.Value.ToString()));
                        read_token(JsonToken.ObjectStart);
                        read_token(JsonToken.PropertyName);
                        token_stack.Push(new DynamoToken(DynamoTokenType.TYPE, json_reader.Value.ToString()));
                        parse_attribute();
                        return;
                    }
                case JsonToken.ObjectEnd:
                    { 
                        if (attribute_stack.Count == 1)
                        {
                            state = State.DONE;
                            return;
                        }
          
                        read_token(JsonToken.ObjectEnd);
                        pop_attribute();
                        return;
                    }
                default:
                    throw new InvalidSyntaxException($"Unexpected token {json_reader.Token}");
            }
        }

        internal void read_scalar()
        {
            string attribute_type = token_stack.Peek().value.ToString();

            switch (attribute_type)
            {
                case "NULL":
                    {
                        read_token(JsonToken.Boolean);
                        Assert((bool)json_reader.Value == true, "Invalid null value");
                        attribute_stack.Push(new NullValue());
                        break;
                    }
                case "BOOL":
                    {
                        read_token(JsonToken.Boolean);
                        attribute_stack.Push(new BoolValue((bool)json_reader.Value));
                        break;
                    }
                case "N":
                    {
                        read_token(JsonToken.String);
                        AssertSyntax(TryParseDecimal(json_reader.Value.ToString(), out decimal decimalValue), "The number is not valid");
                        attribute_stack.Push(new NumberValue(decimalValue));
                        break;
                    }
                case "S":
                    read_token(JsonToken.String);
                    attribute_stack.Push(new StringValue(json_reader.Value.ToString()));
                    break;
                case "B":
                    read_token(JsonToken.String);
                    string base64str = json_reader.Value.ToString();

                    if (base64str.Length == 0)
                    {
                        attribute_stack.Push(new BinaryValue(new byte[0]));
                    }
                    else
                    {
                        try
                        {
                            byte[] b = Convert.FromBase64String(base64str);
                            attribute_stack.Push(new BinaryValue(b));
                        }
                        catch (FormatException)
                        {
                            throw new InvalidSyntaxException($"The binary field does not contain valid base64 data");
                        }
                    }
                    break;
                default:
                    throw new InvalidSyntaxException($"Invalid type {attribute_type}");
            }

            read_token(JsonToken.ObjectEnd);
            pop_attribute();
            return;
        }

        internal void read_set()
        {
            string set_type = token_stack.Peek().value;

            while (read_token() != JsonToken.ArrayEnd)
            {
                Assert(json_reader.Token == JsonToken.String);
                string item_value = json_reader.Value.ToString();

                switch (set_type)
                {
                    case "SS":
                        {
                            var string_element = new StringValue(item_value);
                            ((StringSetValue)attribute_stack.Peek()).Add(string_element);
                            break;
                        }
                    case "NS":
                        {
                            AssertSyntax(TryParseDecimal(item_value, out decimal decimalValue), "The number is not valid");
                            ((NumberSetValue)attribute_stack.Peek()).Add(new NumberValue(decimalValue));
                            break;
                        }
                    case "BS":
                        {
                            if (item_value.Length == 0)
                            {
                                ((BinarySetValue)attribute_stack.Peek()).Add(new BinaryValue(new byte[0]));
                            }
                            else
                            {
                                try
                                {
                                    byte[] b = Convert.FromBase64String(item_value);
                                    ((BinarySetValue)attribute_stack.Peek()).Add(new BinaryValue(b));
                                }
                                catch (FormatException)
                                {
                                    throw new InvalidSyntaxException($"The binary field does not contain valid base64 data");
                                }
                            }
                            break;
                        }
                    default:
                        throw new InvalidSyntaxException();
                }
            }

            read_token(JsonToken.ObjectEnd);
            pop_attribute();
            return;
        }

        internal void read_list()
        {
            switch (read_token())
            {
                case JsonToken.ObjectStart:
                    read_token(JsonToken.PropertyName);
                    token_stack.Push(new DynamoToken(DynamoTokenType.TYPE, json_reader.Value.ToString()));
                    parse_attribute();
                    return;
                case JsonToken.ArrayEnd:
                    read_token(JsonToken.ObjectEnd);
                    pop_attribute();
                    return;
                default:
                    throw new InvalidSyntaxException("Invalid list");
            }

        }

        private void pop_attribute()
        {
            DynamoToken attribute_type_token = token_stack.Pop();
            Assert(attribute_type_token.tokenType == DynamoTokenType.TYPE, "invalid syntax");

            string parent_token_t = token_stack.Peek().value;

            var attribute = attribute_stack.Pop();

            switch (token_stack.Peek().tokenType)
            {
                case DynamoTokenType.KEY:
                    token_stack.Pop();
                    AssertSyntax(attribute_stack.Peek().attributeType == DataTypeDescriptor.M, "Invalid structure");
                    ((MapValue)attribute_stack.Peek())[parent_token_t] = attribute;
                    state = State.READING_MAP;
                    break;

                case DynamoTokenType.TYPE:
                    switch (parent_token_t)
                    {
                        case "L":
                            AssertSyntax(attribute_stack.Peek().attributeType == DataTypeDescriptor.L, "Invalid structure");
                            ((ListValue)attribute_stack.Peek()).Add(attribute);
                            state = State.READING_LIST;
                            break;
                        default:
                            throw new InvalidSyntaxException("Invalid structure");
                    }
                    break;
                default:
                    throw new InvalidSyntaxException($"Invalid structure");
            }
        }

        private void parse_attribute()
        {
            string attribute_type = token_stack.Peek().value;

            switch (attribute_type)
            {
                case "N":
                case "S":
                case "B":
                case "NULL":
                case "BOOL":
                    read_scalar();
                    break;
                case "M":
                    read_token(JsonToken.ObjectStart);
                    attribute_stack.Push(new MapValue());
                    state = State.READING_MAP;
                    break;
                case "L":
                    read_token(JsonToken.ArrayStart);
                    attribute_stack.Push(new ListValue());
                    state = State.READING_LIST;
                    break;
                case "NS":
                    read_token(JsonToken.ArrayStart);
                    attribute_stack.Push(new NumberSetValue());
                    read_set();
                    break;
                case "SS":
                    read_token(JsonToken.ArrayStart);
                    attribute_stack.Push(new StringSetValue());
                    read_set();
                    break;
                case "BS":
                    read_token(JsonToken.ArrayStart);
                    attribute_stack.Push(new BinarySetValue());
                    read_set();
                    break;
                default:
                    throw new InvalidSyntaxException($"Invalid type '{attribute_type}'");
            }
        }

        internal bool TryParseDecimal(string value, out decimal result)
        {
            IFormatProvider formatter = DocumentSerializer.GetDefaultNumberFormatProvider();
            bool parseResult = decimal.TryParse(value, NumberStyles.Any, formatter, out decimal r);
            result = r;
            return parseResult;
        }

        private JsonToken read_token(JsonToken? expected_token = null)
        {
            Assert(json_reader.Read());

            if (expected_token.HasValue)
            {
                Assert(expected_token == json_reader.Token);
            }

            return json_reader.Token;
        }
    }
}
