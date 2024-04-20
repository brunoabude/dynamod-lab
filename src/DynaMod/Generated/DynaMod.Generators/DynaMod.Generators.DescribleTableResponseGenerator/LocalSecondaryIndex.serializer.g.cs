using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class LocalSecondaryIndex
    {
        public static partial LocalSecondaryIndex Load(JsonReader json_reader)
        {
            LocalSecondaryIndex instance = new LocalSecondaryIndex();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "IndexArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.IndexArn = (string)json_reader.Value;
                        break;
                    case "IndexName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.IndexName = (string)json_reader.Value;
                        break;
                    case "IndexSizeBytes":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.IndexSizeBytes = (float)json_reader.Value;
                        break;
                    case "ItemCount":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ItemCount = (float)json_reader.Value;
                        break;
                    case "KeySchema":
                        instance.KeySchema = new List<KeySchemaElement>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.KeySchema.Add(KeySchemaElement.Load(json_reader));
                            Assert(json_reader.Read());
                        }

                        break;
                    case "Projection":
                        instance.Projection = Projection.Load(json_reader);
                        break;
                }
            }

            return instance;
        }
    }
}