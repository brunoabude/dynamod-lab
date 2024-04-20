using DynaMod.ThirdParty.Json.LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace DynaMod.Client
{
    internal enum DynamoAction
    {
        DescribeTable,
        TransactWriteItems,
        Query
    }

    internal class DynamoRequest
    {
        internal readonly DynamoAction Action;
        internal readonly string Body;

        internal const string ContentType = "application/x-amz-json-1.0";

        internal DynamoRequest(DynamoAction action, string body)
        {
            Action = action;
            Body = body;
        }

        internal HttpRequestMessage AsSignedHttpRequest(Profile profile)
        {
            StringContent http_content = new StringContent(Body, Encoding.UTF8, ContentType);

            string body_sha256 = to_sha256hex(Body);

            DateTime request_datetime = DateTime.UtcNow;

            Dictionary<string, string> http_headers = new Dictionary<string, string>
            {
                ["X-Amz-Target"] = $"DynamoDB_20120810.{Enum.GetName(typeof(DynamoAction), Action)}",
                ["User-Agent"] = "dynamod",
                ["Host"] = "dynamodb.us-east-1.amazonaws.com",
                ["X-Amz-Date"] = request_datetime.ToString("yyyyMMddTHHmmssZ")
            };

            var sorted_headers = new SortedDictionary<string, string>();

            foreach (KeyValuePair<string, string> h in http_headers)
            {
                sorted_headers.Add(h.Key, h.Value);
            }

            sorted_headers.Add("content-type", ContentType.Trim());

            const string http_method = "POST";
            const string canonical_uri = "https://dynamodb.us-east-1.amazonaws.com/";
            const string canonical_query_string = "";
            const string hash_algorithm = "AWS4-HMAC-SHA256";

            StringBuilder canonical_request_builder = new StringBuilder();

            canonical_request_builder.Append(http_method + "\n");
            canonical_request_builder.Append("/" + "\n");
            canonical_request_builder.Append(canonical_query_string + "\n");

            foreach (KeyValuePair<string, string> h in sorted_headers)
            {
                canonical_request_builder.Append(h.Key.ToLower() + ":" + h.Value.Trim() + "\n");
            }

            canonical_request_builder.Append("\n");
            canonical_request_builder.Append(string.Join(';', sorted_headers.Keys.Select(i => i.ToLower())) + "\n");
            canonical_request_builder.Append(body_sha256);

            string canonical_request = canonical_request_builder.ToString();

            string canonical_request_hash = to_sha256hex(canonical_request);

            StringBuilder string_to_sign_builder = new StringBuilder();

            string_to_sign_builder.Append(hash_algorithm + "\n");
            string_to_sign_builder.Append(request_datetime.ToString("yyyyMMddTHHmmssZ") + "\n");
            string_to_sign_builder.Append(request_datetime.ToString("yyyyMMdd") + "/" + "us-east-1" + "/" + "dynamodb" + "/" + "aws4_request" + "\n");
            string_to_sign_builder.Append(canonical_request_hash);

            string string_to_sign = string_to_sign_builder.ToString();

            byte[] date_key = sign(Encoding.UTF8.GetBytes("AWS4" + profile.aws_secret_access_key), request_datetime.ToString("yyyyMMdd"));
            byte[] date_region_key = sign(date_key, "us-east-1");
            byte[] date_region_service_key = sign(date_region_key, "dynamodb");
            byte[] signing_key = sign(date_region_service_key, "aws4_request");

            byte[] signature = sign(signing_key, string_to_sign);

            StringBuilder authorization_header_builder = new StringBuilder();

            authorization_header_builder.Append("AWS4-HMAC-SHA256 Credential=" + profile.aws_access_key_id + "/" + request_datetime.ToString("yyyyMMdd") + "/" + "us-east-1" + "/" + "dynamodb" + "/" + "aws4_request");
            authorization_header_builder.Append(", SignedHeaders=");
            authorization_header_builder.Append(string.Join(';', sorted_headers.Keys.Select(i => i)));
            authorization_header_builder.Append(", Signature=" + BitConverter.ToString(signature).Replace("-", "").ToLower());

            string authorization_header = authorization_header_builder.ToString();

            //httpRequest.Headers.Add("X-Amz-Content-SHA256", s_hash);

            var http_request = new HttpRequestMessage(HttpMethod.Post, new Uri(canonical_uri));
            http_request.Content = http_content;

            foreach (var (h_key, h_value) in http_headers)
            {
                http_request.Headers.TryAddWithoutValidation(h_key, h_value);
            }

            http_request.Headers.TryAddWithoutValidation("Authorization", authorization_header);
            http_content.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);

            return http_request;
        }

        private static string to_sha256hex(string data)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private byte[] sign(byte[] key, string message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(Encoding.UTF8.GetBytes(message));
        }
    }
}
