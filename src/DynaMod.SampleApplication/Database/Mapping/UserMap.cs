using DynaMod.Mapping.Fluent.Declarative;
using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;

namespace DynaMod.SampleApplication.Database.Mapping
{
    public class UserMap : FluentEntityMap<User>
    {
        public UserMap()
        {
            Table("users");
            PartitionKey(e => e.Id);
            SortKey(u => "user-entity");
            Unique(e => e.Email);
        }
    }
}
