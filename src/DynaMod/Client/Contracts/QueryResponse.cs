using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.Document;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class QueryResponse 
    {
        public ConsumedCapacity ConsumedCapacity { get; set; }
        public int Count { get; set; }
        public List<MapValue> Items { get; set; }
        public MapValue LastEvaluatedKey { get; set; }
        public int ScannedCount { get; set; }

        public static partial QueryResponse Load(JsonReader json_reader);
    }
}
