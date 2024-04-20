using DynaMod.Client;
using System.Collections.Generic;

namespace DynaMod.Sessions
{
    internal class TableSchemaCache
    {
        private Dictionary<string, TableSchema> _tables;
        private readonly DynamoClient           _client;

        internal TableSchemaCache(DynamoClient client)
        {
            _tables = new Dictionary<string, TableSchema>();
            _client = client;
        }

        internal TableSchema this[string table_name]
        {
            get
            {
                if (!_tables.ContainsKey(table_name))
                {
                    TableSchema table = _client.GetTableDefinition(table_name).GetAwaiter().GetResult();
                    _tables[table_name] = table;
                }

                return _tables[table_name];
            }    
        }
    }
}
