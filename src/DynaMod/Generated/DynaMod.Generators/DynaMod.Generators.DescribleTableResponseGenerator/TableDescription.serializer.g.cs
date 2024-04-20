using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class TableDescription
    {
        public static partial TableDescription Load(JsonReader json_reader)
        {
            TableDescription instance = new TableDescription();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ArchivalSummary":
                        instance.ArchivalSummary = ArchivalSummary.Load(json_reader);
                        break;
                    case "AttributeDefinitions":
                        instance.AttributeDefinitions = new List<AttributeDefinition>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.AttributeDefinitions.Add(AttributeDefinition.Load(json_reader));
                            Assert(json_reader.Read());
                        }

                        break;
                    case "BillingModeSummary":
                        instance.BillingModeSummary = BillingModeSummary.Load(json_reader);
                        break;
                    case "CreationDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.CreationDateTime = (float)json_reader.Value;
                        break;
                    case "DeletionProtectionEnabled":
                        throw new System.NotImplementedException();
                        break;
                    case "GlobalSecondaryIndexes":
                        instance.GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.GlobalSecondaryIndexes.Add(GlobalSecondaryIndex.Load(json_reader));
                            Assert(json_reader.Read());
                        }

                        break;
                    case "GlobalTableVersion":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.GlobalTableVersion = (string)json_reader.Value;
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
                    case "LatestStreamArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.LatestStreamArn = (string)json_reader.Value;
                        break;
                    case "LatestStreamLabel":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.LatestStreamLabel = (string)json_reader.Value;
                        break;
                    case "LocalSecondaryIndexes":
                        instance.LocalSecondaryIndexes = new List<LocalSecondaryIndex>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.LocalSecondaryIndexes.Add(LocalSecondaryIndex.Load(json_reader));
                            Assert(json_reader.Read());
                        }

                        break;
                    case "ProvisionedThroughput":
                        instance.ProvisionedThroughput = ProvisionedThroughput.Load(json_reader);
                        break;
                    case "Replicas":
                        instance.Replicas = new List<Replica>();
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.ArrayStart);
                        json_reader.Read();
                        while (json_reader.Token != JsonToken.ArrayEnd)
                        {
                            instance.Replicas.Add(Replica.Load(json_reader));
                            Assert(json_reader.Read());
                        }

                        break;
                    case "RestoreSummary":
                        instance.RestoreSummary = RestoreSummary.Load(json_reader);
                        break;
                    case "SSEDescription":
                        instance.SSEDescription = SSEDescription.Load(json_reader);
                        break;
                    case "StreamSpecification":
                        instance.StreamSpecification = StreamSpecification.Load(json_reader);
                        break;
                    case "TableArn":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableArn = (string)json_reader.Value;
                        break;
                    case "TableClassSummary":
                        instance.TableClassSummary = TableClassSummary.Load(json_reader);
                        break;
                    case "TableId":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableId = (string)json_reader.Value;
                        break;
                    case "TableName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableName = (string)json_reader.Value;
                        break;
                    case "TableSizeBytes":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.TableSizeBytes = (float)json_reader.Value;
                        break;
                    case "TableStatus":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableStatus = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}