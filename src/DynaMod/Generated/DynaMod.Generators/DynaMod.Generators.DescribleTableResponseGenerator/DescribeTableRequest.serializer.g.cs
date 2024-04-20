using DynaMod.ThirdParty.Json.LitJson;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class DescribeTableRequest
    {
        public static partial DescribeTableRequest Load(JsonReader json_reader)
        {
            DescribeTableRequest instance = new DescribeTableRequest();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "TableName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableName = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}