using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class Replica
    {
        public static partial Replica Load(JsonReader json_reader)
        {
            Replica instance = new Replica();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "GlobalSecondaryIndexes":
                        break;
                    case "KMSMasterKeyId":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.KMSMasterKeyId = (string)json_reader.Value;
                        break;
                    case "ProvisionedThroughputOverride":
                        instance.ProvisionedThroughputOverride = ProvisionedThroughputOverride.Load(json_reader);
                        break;
                    case "RegionName":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.RegionName = (string)json_reader.Value;
                        break;
                    case "ReplicaInaccessibleDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.ReplicaInaccessibleDateTime = (float)json_reader.Value;
                        break;
                    case "ReplicaStatus":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReplicaStatus = (string)json_reader.Value;
                        break;
                    case "ReplicaStatusDescription":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReplicaStatusDescription = (string)json_reader.Value;
                        break;
                    case "ReplicaStatusPercentProgress":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.ReplicaStatusPercentProgress = (string)json_reader.Value;
                        break;
                    case "ReplicaTableClassSummary":
                        instance.ReplicaTableClassSummary = ReplicaTableClassSummary.Load(json_reader);
                        break;
                }
            }

            return instance;
        }
    }
}