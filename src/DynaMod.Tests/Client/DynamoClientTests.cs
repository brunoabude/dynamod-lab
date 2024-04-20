using DynaMod.Client;
using DynaMod.Client.Contracts;
using DynaMod.Serialization.Attributes;
using Xunit;

namespace DynaMod.Tests.Client
{
    public class DynamoClientTests
    {

        [Fact]
        public async Task Test()
        {
            var client = new DynamoClient();

            var queryResponse = await client.QueryAsync(new QueryRequest
            {
                KeyConditionExpression = "PK = :pv",
                ExpressionAttributeValues = new MapValue
                {
                    [":pv"] = new StringValue("SKU#150001#5")
                },
                TableName = "Teste",
                Limit = 1,
                ReturnConsumedCapacity = "INDEXES"
            });
            ;
        }
    }
}
