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
        public IList<string> AttributesToGet { get; set; }
        public string ConditionalOperator { get; set; }
        public bool ConsistentRead { get; set; }
        public MapValue ExclusiveStartKey { get; set; }
        public Dictionary<string, string> ExpressionAttributeNames { get; set; }
        public MapValue ExpressionAttributeValues { get; set; }
        public string FilterExpression { get; set; }
        public string IndexName { get; set; }
        public string KeyConditionExpression { get; set; }
        public int Limit { get; set; }
        public string ProjectionExpression { get; set; }
        public string ReturnConsumedCapacity { get; set; }
        public bool ScanIndexForward { get; set; }
        public string Select { get; set; }
        public string TableName { get; set; }
        public static partial QueryRequest Load(JsonReader json_reader);
    }
}
