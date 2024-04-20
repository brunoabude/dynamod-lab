using DynaMod.Serialization.Attributes;
using System;

namespace DynaMod.Sessions
{
    public class TableSchema
    {
        public readonly string TableName;
        public readonly string PartitionKeyFieldName;
        public readonly DataTypeDescriptor PartitionKeyAttributeType;

        public readonly string SortKeyFieldName;
        public readonly DataTypeDescriptor SortKeyAttributeType;

        public readonly bool HasSortKey;

        public TableSchema(string tableName, string partitionKeyFieldName, DataTypeDescriptor partitionKeyAttributeType)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException($"'{nameof(tableName)}' cannot be null or whitespace.", nameof(tableName));
            }

            if (string.IsNullOrWhiteSpace(partitionKeyFieldName))
            {
                throw new ArgumentException($"'{nameof(partitionKeyFieldName)}' cannot be null or whitespace.", nameof(partitionKeyFieldName));
            }

            TableName = tableName;
            PartitionKeyFieldName = partitionKeyFieldName;
            PartitionKeyAttributeType = partitionKeyAttributeType;
            HasSortKey = false;
        }

        public TableSchema(string tableName, string partitionKeyFieldName, DataTypeDescriptor partitionKeyAttributeType, string sortKeyFieldName, DataTypeDescriptor sortKeyAttributeType)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException($"'{nameof(tableName)}' cannot be null or whitespace.", nameof(tableName));
            }

            if (string.IsNullOrWhiteSpace(partitionKeyFieldName))
            {
                throw new ArgumentException($"'{nameof(partitionKeyFieldName)}' cannot be null or whitespace.", nameof(partitionKeyFieldName));
            }

            TableName = tableName;
            PartitionKeyFieldName = partitionKeyFieldName;
            PartitionKeyAttributeType = partitionKeyAttributeType;
            SortKeyFieldName = sortKeyFieldName;
            SortKeyAttributeType = sortKeyAttributeType;
            HasSortKey = true;
        }
    }
}
