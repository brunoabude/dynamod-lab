using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ProvisionedThroughput
    {
        public static partial ProvisionedThroughput Load(JsonReader json_reader)
        {
            ProvisionedThroughput instance = new ProvisionedThroughput();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "LastDecreaseDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.LastDecreaseDateTime = (float)json_reader.Value;
                        break;
                    case "LastIncreaseDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.LastIncreaseDateTime = (float)json_reader.Value;
                        break;
                    case "_1000OfDecreasesToday":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance._1000OfDecreasesToday = (float)json_reader.Value;
                        break;
                    case "ReadCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ReadCapacityUnits = (float)json_reader.Value;
                        break;
                    case "WriteCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.WriteCapacityUnits = (float)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}