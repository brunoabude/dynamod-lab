using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class QueryRequest
    {
        public static partial QueryRequest Load(JsonReader json_reader)
        {
            QueryRequest instance = new QueryRequest();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "AttributesToGet":
                        throw new System.NotImplementedException();
                        break;
                    case "ConditionalOperator":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ConditionalOperator = (string)json_reader.Value;
                        break;
                    case "ConsistentRead":
                        throw new System.NotImplementedException();
                        break;
                    case "ExclusiveStartKey":
                        instance.ExclusiveStartKey = MapValue.Load(json_reader);
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
                    case "FilterExpression":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.FilterExpression = (string)json_reader.Value;
                        break;
                    case "IndexName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.IndexName = (string)json_reader.Value;
                        break;
                    case "KeyConditionExpression":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.KeyConditionExpression = (string)json_reader.Value;
                        break;
                    case "Limit":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Int);
                        instance.Limit = (int)json_reader.Value;
                        break;
                    case "ProjectionExpression":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ProjectionExpression = (string)json_reader.Value;
                        break;
                    case "ReturnConsumedCapacity":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReturnConsumedCapacity = (string)json_reader.Value;
                        break;
                    case "ScanIndexForward":
                        throw new System.NotImplementedException();
                        break;
                    case "Select":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.Select = (string)json_reader.Value;
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