using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ProvisionedThroughputOverride
    {
        public static partial ProvisionedThroughputOverride Load(JsonReader json_reader)
        {
            ProvisionedThroughputOverride instance = new ProvisionedThroughputOverride();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ReadCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ReadCapacityUnits = (float)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}