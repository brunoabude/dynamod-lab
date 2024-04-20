using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.Document;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class QueryResponse
    {
        public static partial QueryResponse Load(JsonReader json_reader)
        {
            QueryResponse instance = new QueryResponse();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ConsumedCapacity":
                        instance.ConsumedCapacity = ConsumedCapacity.Load(json_reader);
                        break;
                    case "Count":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Int);
                        instance.Count = (int)json_reader.Value;
                        break;
                    case "Items":
                        instance.Items = new List<MapValue>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.Items.Add(MapValue.Load(json_reader));
                            Assert(json_reader.Read());
                        }

                        break;
                    case "LastEvaluatedKey":
                        instance.LastEvaluatedKey = MapValue.Load(json_reader);
                        break;
                    case "ScannedCount":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Int);
                        instance.ScannedCount = (int)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}