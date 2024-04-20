using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;

using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class TransactWriteItemsResponse
    {
        public ConsumedCapacity[] ConsumedCapacity { get; set; }
        public Dictionary<string, ItemCollectionMetricsResponse> ItemCollectionMetrics { get; set; }
        public static partial TransactWriteItemsResponse Load(JsonReader json_reader);
    }

    public partial class ItemCollectionMetricsResponse
    {
        public MapValue ItemCollectionKey { get; set; }
        public List<int> SizeEstimateRangeGB { get; set; }

        public static partial ItemCollectionMetricsResponse Load(JsonReader json_reader);
    }


    public partial class TableMetrics
    {
        public double CapacityUnits { get; set; }
        public double ReadCapacityUnits { get; set; }
        public double WriteCapacityUnits { get; set; }
        public static partial TableMetrics Load(JsonReader json_reader);
    }
}
