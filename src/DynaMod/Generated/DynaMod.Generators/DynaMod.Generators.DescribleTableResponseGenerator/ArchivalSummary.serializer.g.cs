using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ArchivalSummary
    {
        public static partial ArchivalSummary Load(JsonReader json_reader)
        {
            ArchivalSummary instance = new ArchivalSummary();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ArchivalBackupArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ArchivalBackupArn = (string)json_reader.Value;
                        break;
                    case "ArchivalDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ArchivalDateTime = (float)json_reader.Value;
                        break;
                    case "ArchivalReason":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ArchivalReason = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}