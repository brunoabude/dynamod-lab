using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;
using DynaMod.SampleApplication.Domain.Services;
using DynaMod.Serialization.Attributes;
using DynaMod.Sessions;

namespace DynaMod.SampleApplication.Domain.Repository
{
    public class UserRepository
    {
        private readonly DynamoSession _session;

        public UserRepository(DynamoSession session)
        {
            _session = session;
        }

        public User CreateUser(string user_id, string display_name, string email, string plain_text_password)
        {
            byte[] salt = SecurityService.GenerateSalt();
            byte[] password_hash = SecurityService.PasswordHash(plain_text_password, salt);

            User user = new User(user_id, display_name, email, password_hash, salt);

            _session.Add(user);
            return user;
        }

        public async Task<User> GetUser(string user_id)
        {
            var user = await _session.GetAsync<User>(new StringValue(user_id));
            return user;
        }
    }
}
