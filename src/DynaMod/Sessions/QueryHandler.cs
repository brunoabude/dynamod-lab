using DynaMod.Querying;
using System;

using static DynaMod.Common.Assertions;

namespace DynaMod.Sessions
{
    internal class QueryHandler
    {
        private readonly DynamoSession _session;

        internal QueryHandler(DynamoSession session)
        {
            _session = session;
        }

        internal ResultSet Handle(Query query)
        {
            Assert(query.TargetType != null, "Query entity type invalid");


            throw new NotImplementedException();
        }
    }
}
