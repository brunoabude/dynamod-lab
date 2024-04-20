using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class IndexConsumeCapacity
    {
        public static partial IndexConsumeCapacity Load(JsonReader json_reader)
        {
            IndexConsumeCapacity instance = new IndexConsumeCapacity();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "CapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Int);
                        instance.CapacityUnits = (int)json_reader.Value;
                        break;
                    case "ReadCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Int);
                        instance.ReadCapacityUnits = (int)json_reader.Value;
                        break;
                    case "WriteCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Int);
                        instance.WriteCapacityUnits = (int)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}