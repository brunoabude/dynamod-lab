using DynaMod.ThirdParty.Json.LitJson;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class DescribeTableRequest
    {
        public string TableName { get; set; }
        public static partial DescribeTableRequest Load(JsonReader json_reader);
    }
}
