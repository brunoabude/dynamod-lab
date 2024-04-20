using DynaMod.Client;
using DynaMod.Sessions;

namespace DynaMod.Tests.SampleApplication
{
    public class SampleApplicationTestCase
    {
        protected readonly DynamoClient _dynamo_client;
        protected readonly DynamoSession _session;

        protected SampleApplicationTestCase()
        {
            _dynamo_client = new DynamoClient();
            _session = new DynamoSession(_dynamo_client);
        }
    }
}
