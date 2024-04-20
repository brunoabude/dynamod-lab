using DynaMod.Client.Contracts;
using DynaMod.Serialization.Attributes;
using DynaMod.Sessions;
using DynaMod.ThirdParty.Json.LitJson;
using DynaMod.Serialization.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DynaMod.Client
{
    public struct Profile
    {
        public string profile_name;
        public string aws_access_key_id;
        public string aws_secret_access_key;
    }

    public class DynamoClient
    {
        private readonly HttpClient _httpClient;
        private Profile default_credentials;

        static bool json_mapper_setup = false;

        private static void SetupJsonMapper()
        {
            if (!json_mapper_setup)
            {
                JsonMapper.RegisterExporter<MapValue>((map_value, json_writer) =>
                {
                    DocumentSerializer.WriteMapValue(json_writer, map_value);
                });

                JsonMapper.RegisterExporter<NumberValue>((number_value, json_writer) =>
                {
                    DocumentSerializer.WriteNumberValue(json_writer, number_value);
                });

                JsonMapper.RegisterExporter<BinaryValue>((binary_value, json_writer) =>
                {
                    DocumentSerializer.WriteBinaryValue(json_writer, binary_value);
                });

                JsonMapper.RegisterExporter<StringValue>((string_value, json_writer) =>
                {
                    DocumentSerializer.WriteStringValue(json_writer, string_value);
                });

                json_mapper_setup = true;
            }
        }

        public DynamoClient()
        {
            _httpClient = new HttpClient();
            default_credentials = GetDefaultProfile();
            SetupJsonMapper();
        }

        public DynamoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            default_credentials = GetDefaultProfile();
            SetupJsonMapper();
        }

        public async Task<TableSchema> GetTableDefinition(string tableName)
        {
            DescribeTableRequest describe_table_request = new DescribeTableRequest
            {
                TableName = tableName
            };

            DescribeTableResponse describe_table_response = await Call<DescribeTableResponse>(DynamoAction.DescribeTable, describe_table_request);

            TableDescription tableDescription = describe_table_response.Table;

            KeySchemaElement hashKey = tableDescription.KeySchema.Single(e => e.KeyType == "HASH");
            AttributeDefinition hashAttribute = tableDescription.AttributeDefinitions.Single(attr => attr.AttributeName == hashKey.AttributeName);
            DataTypeDescriptor pkType = (DataTypeDescriptor)Enum.Parse(typeof(DataTypeDescriptor), hashAttribute.AttributeType);

            if (tableDescription.KeySchema.Count == 1)
            {
                return new TableSchema(tableName, hashKey.AttributeName, pkType);
            }
            else
            {
                KeySchemaElement rangeKey = tableDescription.KeySchema.Single(e => e.KeyType == "RANGE");
                AttributeDefinition rangeAttribute = tableDescription.AttributeDefinitions.Single(attr => attr.AttributeName == rangeKey.AttributeName);
                DataTypeDescriptor skType = (DataTypeDescriptor)Enum.Parse(typeof(DataTypeDescriptor), rangeAttribute.AttributeType);
                return new TableSchema(tableName, hashKey.AttributeName, pkType, rangeKey.AttributeName, skType);
            }
        }

        internal async Task CommitTransaction(TransactWriteItemRequest transaction)
        {
            TransactWriteItemsResponse response = await Call<TransactWriteItemsResponse>(DynamoAction.TransactWriteItems, transaction);
        }

        internal async Task<QueryResponse> QueryAsync(QueryRequest queryRequest)
        {
            // TODO LastEvaluatedToken
            var response = await Call(DynamoAction.Query, queryRequest);

            var json_reader = new JsonReader(response);

            return QueryResponse.Load(json_reader);
        }

        private Profile GetDefaultProfile()
        {
            string path = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\.aws\\credentials");

            using FileStream f = File.Open(path, FileMode.Open);
            using TextReader r = new StreamReader(f);

            List<Profile> profiles = new List<Profile>();

            Profile profile = new Profile();

            while (true)
            {
                string line = r.ReadLine()?.Trim();

                if (line == null)
                    break;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    profiles.Add(profile);
                    profile = new Profile();
                    profile.profile_name = line.Substring(1, line.Length-2);
                    continue;
                }

                var kv = line.Split('=');

                if (kv.Length != 2)
                    continue;

                string key = kv[0].Trim();
                string value = kv[1].Trim();

                switch (key)
                {
                    case "aws_access_key_id": profile.aws_access_key_id = value; break;
                    case "aws_secret_access_key": profile.aws_secret_access_key = value; break;
                    default: break;
                }
            }

            profiles.Add(profile);

            return profiles.First(p => p.profile_name == "default");
        }

        private async Task<TResponse> Call<TResponse>(DynamoAction action, object payload)
        {
            DynamoRequest request = new DynamoRequest(action, JsonMapper.ToJson(payload));

            var response = await _httpClient.SendAsync(request.AsSignedHttpRequest(GetDefaultProfile()));

            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();
            }

            return JsonMapper.ToObject<TResponse>(await response.Content.ReadAsStringAsync());
        }

        private async Task<string> Call(DynamoAction action, object payload)
        {
            DynamoRequest request = new DynamoRequest(action, JsonMapper.ToJson(payload));

            var response = await _httpClient.SendAsync(request.AsSignedHttpRequest(GetDefaultProfile()));

            if (!response.IsSuccessStatusCode)
            {
                string err = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
