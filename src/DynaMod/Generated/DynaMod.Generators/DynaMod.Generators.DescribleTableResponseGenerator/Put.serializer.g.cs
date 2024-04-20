using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class Put
    {
        public static partial Put Load(JsonReader json_reader)
        {
            Put instance = new Put();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ConditionExpression":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ConditionExpression = (string)json_reader.Value;
                        break;
                    case "ExpressionAttributeNames":
                        instance.ExpressionAttributeNames = new Dictionary<string, string>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
                        while (json_reader.Token != JsonToken.ObjectEnd)
                        {
                            Assert(json_reader.Read() && json_reader.Token == JsonToken.PropertyName);
                            string property_name;
                            property_name = json_reader.Value.ToString();
                            Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                            instance.ExpressionAttributeNames[json_reader.Value.ToString()] = (string)json_reader.Value;
                            Assert(json_reader.Read());
                        }

                        break;
                    case "ExpressionAttributeValues":
                        instance.ExpressionAttributeValues = MapValue.Load(json_reader);
                        break;
                    case "Item":
                        instance.Item = MapValue.Load(json_reader);
                        break;
                    case "ReturnValuesOnConditionCheckFailure":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReturnValuesOnConditionCheckFailure = (string)json_reader.Value;
                        break;
                    case "TableName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableName = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}