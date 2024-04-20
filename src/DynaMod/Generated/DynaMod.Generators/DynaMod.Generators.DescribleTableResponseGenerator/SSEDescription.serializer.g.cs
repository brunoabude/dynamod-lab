using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class SSEDescription
    {
        public static partial SSEDescription Load(JsonReader json_reader)
        {
            SSEDescription instance = new SSEDescription();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "InaccessibleEncryptionDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.InaccessibleEncryptionDateTime = (float)json_reader.Value;
                        break;
                    case "KMSMasterKeyArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.KMSMasterKeyArn = (string)json_reader.Value;
                        break;
                    case "SSEType":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.SSEType = (string)json_reader.Value;
                        break;
                    case "Status":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.Status = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}