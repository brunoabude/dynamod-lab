using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ItemCollectionMetrics
    {
        public static partial ItemCollectionMetrics Load(JsonReader json_reader)
        {
            ItemCollectionMetrics instance = new ItemCollectionMetrics();
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