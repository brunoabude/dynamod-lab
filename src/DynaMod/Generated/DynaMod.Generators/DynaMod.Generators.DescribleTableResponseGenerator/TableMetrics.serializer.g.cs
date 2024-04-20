using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class TableMetrics
    {
        public static partial TableMetrics Load(JsonReader json_reader)
        {
            TableMetrics instance = new TableMetrics();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "CapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.CapacityUnits = (double)json_reader.Value;
                        break;
                    case "ReadCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ReadCapacityUnits = (double)json_reader.Value;
                        break;
                    case "WriteCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.WriteCapacityUnits = (double)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}