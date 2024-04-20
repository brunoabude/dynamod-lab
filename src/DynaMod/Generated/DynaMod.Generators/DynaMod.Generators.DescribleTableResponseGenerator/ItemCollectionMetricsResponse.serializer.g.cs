using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ItemCollectionMetricsResponse
    {
        public static partial ItemCollectionMetricsResponse Load(JsonReader json_reader)
        {
            ItemCollectionMetricsResponse instance = new ItemCollectionMetricsResponse();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ItemCollectionKey":
                        instance.ItemCollectionKey = MapValue.Load(json_reader);
                        break;
                    case "SizeEstimateRangeGB":
                        instance.SizeEstimateRangeGB = new List<int>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.SizeEstimateRangeGB.Add((int)json_reader.Value);
                            Assert(json_reader.Read());
                        }

                        break;
                }
            }

            return instance;
        }
    }
}