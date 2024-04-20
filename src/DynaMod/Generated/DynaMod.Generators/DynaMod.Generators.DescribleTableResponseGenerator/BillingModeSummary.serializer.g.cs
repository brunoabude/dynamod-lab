using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class BillingModeSummary
    {
        public static partial BillingModeSummary Load(JsonReader json_reader)
        {
            BillingModeSummary instance = new BillingModeSummary();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "BillingMode":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.BillingMode = (string)json_reader.Value;
                        break;
                    case "LastUpdateToPayPerRequestDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.LastUpdateToPayPerRequestDateTime = (float)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}