using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class TransactWriteItemRequest
    {
        public static partial TransactWriteItemRequest Load(JsonReader json_reader)
        {
            TransactWriteItemRequest instance = new TransactWriteItemRequest();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ClientRequestToken":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ClientRequestToken = (string)json_reader.Value;
                        break;
                    case "ReturnConsumedCapacity":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReturnConsumedCapacity = (string)json_reader.Value;
                        break;
                    case "ReturnItemCollectionMetrics":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReturnItemCollectionMetrics = (string)json_reader.Value;
                        break;
                    case "TransactItems":
                        throw new System.NotImplementedException();
                        break;
                }
            }

            return instance;
        }
    }
}