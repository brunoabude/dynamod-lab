using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class RestoreSummary
    {
        public static partial RestoreSummary Load(JsonReader json_reader)
        {
            RestoreSummary instance = new RestoreSummary();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "RestoreDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.RestoreDateTime = (float)json_reader.Value;
                        break;
                    case "RestoreInProgress":
                        throw new System.NotImplementedException();
                        break;
                    case "SourceBackupArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.SourceBackupArn = (string)json_reader.Value;
                        break;
                    case "SourceTableArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.SourceTableArn = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}