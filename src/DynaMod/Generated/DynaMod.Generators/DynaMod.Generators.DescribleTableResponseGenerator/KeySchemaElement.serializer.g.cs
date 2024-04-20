using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class KeySchemaElement
    {
        public static partial KeySchemaElement Load(JsonReader json_reader)
        {
            KeySchemaElement instance = new KeySchemaElement();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "AttributeName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.AttributeName = (string)json_reader.Value;
                        break;
                    case "KeyType":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.KeyType = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}