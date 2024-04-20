using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class TransactWriteItemsResponse
    {
        public static partial TransactWriteItemsResponse Load(JsonReader json_reader)
        {
            TransactWriteItemsResponse instance = new TransactWriteItemsResponse();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ConsumedCapacity":
                        break;
                    case "ItemCollectionMetrics":
                        instance.ItemCollectionMetrics = new Dictionary<string, ItemCollectionMetricsResponse>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
                        while (json_reader.Token != JsonToken.ObjectEnd)
                        {
                            Assert(json_reader.Read() && json_reader.Token == JsonToken.PropertyName);
                            string property_name;
                            property_name = json_reader.Value.ToString();
                            instance.ItemCollectionMetrics[json_reader.Value.ToString()] = ItemCollectionMetricsResponse.Load(json_reader);
                            Assert(json_reader.Read());
                        }

                        break;
                }
            }

            return instance;
        }
    }
}