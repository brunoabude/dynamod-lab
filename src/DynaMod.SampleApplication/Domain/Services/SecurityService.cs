using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;
using System.Security.Cryptography;
using System.Text;

namespace DynaMod.SampleApplication.Domain.Services
{
    public class SecurityService
    {
        public static byte[] GenerateSalt()
        {
            byte[] password_salt = new byte[10];
            new Random().NextBytes(password_salt);

            return password_salt;
        }

        public static byte[] PasswordHash(string plain_text_password, byte[] salt)
        {
            byte[] password_bytes = Encoding.ASCII.GetBytes(plain_text_password);
            
            var string_to_hash = new byte[password_bytes.Length + salt.Length];

            password_bytes.CopyTo(string_to_hash, 0);
            salt.CopyTo(string_to_hash, password_bytes.Length);

            using (SHA256 hasher = SHA256.Create())
            {
                return hasher.ComputeHash(string_to_hash);
            }
        }

        public static bool CheckPassword(User user, string plain_text_password)
        {
            var hash = PasswordHash(plain_text_password, user.PasswordSalt);

            ReadOnlySpan<byte> b1 = hash;
            ReadOnlySpan<byte> b2 = user.PasswordHash;

            return b1.SequenceEqual(b2);
        }
    }
}
