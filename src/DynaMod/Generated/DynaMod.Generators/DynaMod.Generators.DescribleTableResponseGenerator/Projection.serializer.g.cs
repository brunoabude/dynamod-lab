using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class Projection
    {
        public static partial Projection Load(JsonReader json_reader)
        {
            Projection instance = new Projection();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "NonKeyAttributes":
                        instance.NonKeyAttributes = new List<string>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.NonKeyAttributes.Add((string)json_reader.Value);
                            Assert(json_reader.Read());
                        }

                        break;
                    case "ProjectionType":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ProjectionType = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}