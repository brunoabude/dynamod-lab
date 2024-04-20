using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{

    public partial class TransactWriteItemRequest
    {
        public string ClientRequestToken { get; set; }
        public string ReturnConsumedCapacity { get; set; }
        public string ReturnItemCollectionMetrics { get; set; }
        public IList<Transactitem> TransactItems { get; set; }
        public static partial TransactWriteItemRequest Load(JsonReader json_reader);
    }

    public partial class Transactitem
    {
        public Conditioncheck ConditionCheck { get; set; }
        public Delete Delete { get; set; }
        public Put Put { get; set; }
        public Update Update { get; set; }
        public static partial Transactitem Load(JsonReader json_reader);
    }

    public partial class Conditioncheck
    {
        public string ConditionExpression { get; set; }
        public Dictionary<string, string> ExpressionAttributeNames { get; set; }
        public MapValue ExpressionAttributeValues { get; set; }
        public MapValue Key { get; set; }
        public string ReturnValuesOnConditionCheckFailure { get; set; }
        public string TableName { get; set; }
        public static partial Conditioncheck Load(JsonReader json_reader);
    }

    public partial class Delete
    {
        public string ConditionExpression { get; set; }
        public Dictionary<string, string> ExpressionAttributeNames { get; set; }
        public MapValue ExpressionAttributeValues { get; set; }
        public MapValue Key { get; set; }
        public string ReturnValuesOnConditionCheckFailure { get; set; }
        public string TableName { get; set; }
        public static partial Delete Load(JsonReader json_reader);
    }


    public partial class Put
    {
        public string ConditionExpression { get; set; }
        public Dictionary<string, string> ExpressionAttributeNames { get; set; }
        public MapValue ExpressionAttributeValues { get; set; }
        public MapValue Item { get; set; }
        public string ReturnValuesOnConditionCheckFailure { get; set; }
        public string TableName { get; set; }
        public static partial Put Load(JsonReader json_reader);
    }


    public partial class Update
    {
        public string ConditionExpression { get; set; }
        public Dictionary<string, string> ExpressionAttributeNames { get; set; }
        public MapValue ExpressionAttributeValues { get; set; }
        public MapValue Key { get; set; }
        public string ReturnValuesOnConditionCheckFailure { get; set; }
        public string TableName { get; set; }
        public string UpdateExpression { get; set; }
        public static partial Update Load(JsonReader json_reader);
    }
}
