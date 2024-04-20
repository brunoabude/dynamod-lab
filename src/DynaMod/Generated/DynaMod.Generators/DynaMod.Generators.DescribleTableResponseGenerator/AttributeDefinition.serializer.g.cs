using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class AttributeDefinition
    {
        public static partial AttributeDefinition Load(JsonReader json_reader)
        {
            AttributeDefinition instance = new AttributeDefinition();
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
                    case "AttributeType":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.AttributeType = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}