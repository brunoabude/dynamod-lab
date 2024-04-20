using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{

    public partial class DescribeTableResponse
    {
        public TableDescription Table { get; set; }
        public static partial DescribeTableResponse Load(JsonReader json_reader);
    }

    public partial class TableDescription
    {
        public ArchivalSummary ArchivalSummary { get; set; }
        public List<AttributeDefinition> AttributeDefinitions { get; set; }
        public BillingModeSummary BillingModeSummary { get; set; }
        public float CreationDateTime { get; set; }
        public bool DeletionProtectionEnabled { get; set; }
        public List<GlobalSecondaryIndex> GlobalSecondaryIndexes { get; set; }
        public string GlobalTableVersion { get; set; }
        public float ItemCount { get; set; }
        public List<KeySchemaElement> KeySchema { get; set; }
        public string LatestStreamArn { get; set; }
        public string LatestStreamLabel { get; set; }
        public List<LocalSecondaryIndex> LocalSecondaryIndexes { get; set; }
        public ProvisionedThroughput ProvisionedThroughput { get; set; }
        public List<Replica> Replicas { get; set; }
        public RestoreSummary RestoreSummary { get; set; }
        public SSEDescription SSEDescription { get; set; }
        public StreamSpecification StreamSpecification { get; set; }
        public string TableArn { get; set; }
        public TableClassSummary TableClassSummary { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public float TableSizeBytes { get; set; }
        public string TableStatus { get; set; }

        public static partial TableDescription Load(JsonReader json_reader);
    }

    public partial class ArchivalSummary
    {
        public string ArchivalBackupArn { get; set; }
        public float ArchivalDateTime { get; set; }
        public string ArchivalReason { get; set; }
        public static partial ArchivalSummary Load(JsonReader json_reader);
    }

    public partial class BillingModeSummary
    {
        public string BillingMode { get; set; }
        public float LastUpdateToPayPerRequestDateTime { get; set; }
        public static partial BillingModeSummary Load(JsonReader json_reader);
    }

    public partial class ProvisionedThroughput
    {
        public float LastDecreaseDateTime { get; set; }
        public float LastIncreaseDateTime { get; set; }
        public float _1000OfDecreasesToday { get; set; }
        public float ReadCapacityUnits { get; set; }
        public float WriteCapacityUnits { get; set; }
        public static partial ProvisionedThroughput Load(JsonReader json_reader);
    }

    public partial class RestoreSummary
    {
        public float RestoreDateTime { get; set; }
        public bool RestoreInProgress { get; set; }
        public string SourceBackupArn { get; set; }
        public string SourceTableArn { get; set; }
        public static partial RestoreSummary Load(JsonReader json_reader);
    }

    public partial class SSEDescription
    {
        public float InaccessibleEncryptionDateTime { get; set; }
        public string KMSMasterKeyArn { get; set; }
        public string SSEType { get; set; }
        public string Status { get; set; }
        public static partial SSEDescription Load(JsonReader json_reader);
    }

    public partial class StreamSpecification
    {
        public bool StreamEnabled { get; set; }
        public string StreamViewType { get; set; }
        public static partial StreamSpecification Load(JsonReader json_reader);
    }

    public partial class TableClassSummary
    {
        public float LastUpdateDateTime { get; set; }
        public string TableClass { get; set; }
        public static partial TableClassSummary Load(JsonReader json_reader);
    }

    public partial class AttributeDefinition
    {
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public static partial AttributeDefinition Load(JsonReader json_reader);
    }

    public partial class GlobalSecondaryIndex
    {
        public bool Backfilling { get; set; }
        public string IndexArn { get; set; }
        public string IndexName { get; set; }
        public float IndexSizeBytes { get; set; }
        public string IndexStatus { get; set; }
        public float ItemCount { get; set; }
        public List<KeySchemaElement> KeySchema { get; set; }
        public Projection Projection { get; set; }
        public ProvisionedThroughput ProvisionedThroughput { get; set; }
        public static partial GlobalSecondaryIndex Load(JsonReader json_reader);
    }

    public partial class Projection
    {
        public List<string> NonKeyAttributes { get; set; }
        public string ProjectionType { get; set; }
        public static partial Projection Load(JsonReader json_reader);
    }

    public partial class KeySchemaElement
    {
        public string AttributeName { get; set; }
        public string KeyType { get; set; }
        public static partial KeySchemaElement Load(JsonReader json_reader);
    }

    public partial class LocalSecondaryIndex
    {
        public string IndexArn { get; set; }
        public string IndexName { get; set; }
        public float IndexSizeBytes { get; set; }
        public float ItemCount { get; set; }
        public List<KeySchemaElement> KeySchema { get; set; }
        public Projection Projection { get; set; }
        public static partial LocalSecondaryIndex Load(JsonReader json_reader);
    }


    public partial class Replica
    {
        public GlobalSecondaryIndex[] GlobalSecondaryIndexes { get; set; }
        public string KMSMasterKeyId { get; set; }
        public ProvisionedThroughputOverride ProvisionedThroughputOverride { get; set; }
        public string RegionName { get; set; }
        public float ReplicaInaccessibleDateTime { get; set; }
        public string ReplicaStatus { get; set; }
        public string ReplicaStatusDescription { get; set; }
        public string ReplicaStatusPercentProgress { get; set; }
        public ReplicaTableClassSummary ReplicaTableClassSummary { get; set; }
        public static partial Replica Load(JsonReader json_reader);
    }

    public partial class ProvisionedThroughputOverride
    {
        public float ReadCapacityUnits { get; set; }
        public static partial ProvisionedThroughputOverride Load(JsonReader json_reader);
    }

    public partial class ReplicaTableClassSummary
    {
        public float LastUpdateDateTime { get; set; }
        public string TableClass { get; set; }
        public static partial ReplicaTableClassSummary Load(JsonReader json_reader);
}
}
