using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class StreamSpecification
    {
        public static partial StreamSpecification Load(JsonReader json_reader)
        {
            StreamSpecification instance = new StreamSpecification();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "StreamEnabled":
                        throw new System.NotImplementedException();
                        break;
                    case "StreamViewType":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.StreamViewType = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}