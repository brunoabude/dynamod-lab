using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class DescribeTableResponse
    {
        public static partial DescribeTableResponse Load(JsonReader json_reader)
        {
            DescribeTableResponse instance = new DescribeTableResponse();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "Table":
                        instance.Table = TableDescription.Load(json_reader);
                        break;
                }
            }

            return instance;
        }
    }
}