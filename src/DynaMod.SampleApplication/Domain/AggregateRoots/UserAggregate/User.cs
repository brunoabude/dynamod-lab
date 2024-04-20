using DynaMod.SampleApplication.Domain.AggregateRoots.CommentAggregate;
using DynaMod.SampleApplication.Domain.AggregateRoots.PostAggregate;

namespace DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate
{
    public class User
    {
        public string         Id { get; private set; }
        public byte[]         PasswordHash { get; private set; }
        public byte[]         PasswordSalt { get; private set; }
        public string         DisplayName { get; private set; }
        public string         Email { get; private set; }
        public List<Comment>  Comments { get; private set; }
        public List<Post>     Posts { get; private set; }

        public User(string id, string display_name, string email, byte[] password_hash, byte[] password_salt)
        {
            Id           = id;
            PasswordHash = password_hash;
            PasswordSalt = password_salt;
            DisplayName  = display_name;
            Email        = email;
            Comments     = new List<Comment>();
            Posts        = new List<Post>();
        }

        public void ChangeDisplayName(string new_name)
        {
            DisplayName = new_name;
        }
    }
}