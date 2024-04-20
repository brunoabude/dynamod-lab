using System;
using System.Collections.Generic;

namespace DynaMod.Querying
{
    public class Query
    {
        internal readonly Type TargetType;
        internal readonly IList<Condition> Conditions;

        internal Query(Type entityType)
        {
            Conditions = new List<Condition>();
            TargetType = entityType;
        }
    }
}
