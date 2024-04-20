using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class ConsumedCapacity
    {
        public static partial ConsumedCapacity Load(JsonReader json_reader)
        {
            ConsumedCapacity instance = new ConsumedCapacity();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "CapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.CapacityUnits = (double)json_reader.Value;
                        break;
                    case "GlobalSecondaryIndexes":
                        instance.GlobalSecondaryIndexes = new Dictionary<string, IndexConsumeCapacity>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
                        while (json_reader.Token != JsonToken.ObjectEnd)
                        {
                            Assert(json_reader.Read() && json_reader.Token == JsonToken.PropertyName);
                            string property_name;
                            property_name = json_reader.Value.ToString();
                            instance.GlobalSecondaryIndexes[json_reader.Value.ToString()] = IndexConsumeCapacity.Load(json_reader);
                            Assert(json_reader.Read());
                        }

                        break;
                    case "LocalSecondaryIndexes":
                        instance.LocalSecondaryIndexes = new Dictionary<string, IndexConsumeCapacity>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
                        while (json_reader.Token != JsonToken.ObjectEnd)
                        {
                            Assert(json_reader.Read() && json_reader.Token == JsonToken.PropertyName);
                            string property_name;
                            property_name = json_reader.Value.ToString();
                            instance.LocalSecondaryIndexes[json_reader.Value.ToString()] = IndexConsumeCapacity.Load(json_reader);
                            Assert(json_reader.Read());
                        }

                        break;
                    case "ReadCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ReadCapacityUnits = (double)json_reader.Value;
                        break;
                    case "Table":
                        instance.Table = TableMetrics.Load(json_reader);
                        break;
                    case "TableName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableName = (string)json_reader.Value;
                        break;
                    case "WriteCapacityUnits":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.WriteCapacityUnits = (double)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}