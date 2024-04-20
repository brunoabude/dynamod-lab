using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;

using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ConsumedCapacity
    {
        public double CapacityUnits { get; set; }
        public Dictionary<string, IndexConsumeCapacity> GlobalSecondaryIndexes { get; set; }
        public Dictionary<string, IndexConsumeCapacity> LocalSecondaryIndexes { get; set; }
        public double ReadCapacityUnits { get; set; }
        public TableMetrics Table { get; set; }
        public string TableName { get; set; }
        public double WriteCapacityUnits { get; set; }

        public static partial ConsumedCapacity Load(JsonReader json_reader);
    }

    public partial class IndexConsumeCapacity
    {
        public int CapacityUnits { get; set; }
        public int ReadCapacityUnits { get; set; }
        public int WriteCapacityUnits { get; set; }

        public static partial IndexConsumeCapacity Load(JsonReader json_reader);
    }
}
