using System.Security.Cryptography;
using System.Text;

namespace WorldKartIdentity.Helpers
{
    public class TokenHelper
    {
        public static (string plain, string hash) GenerateRefreshToken()
        {
            // 32 bytes random → base64 string
            var bytes = RandomNumberGenerator.GetBytes(32);
            var plain = Convert.ToBase64String(bytes);
            var hash = Sha256(plain);
            return (plain, hash);
        }

        public static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // hex string, easier to index
        }

        public class TokenPairResponse
        {
            public string AccessToken { get; set; } = "";
            public string RefreshToken { get; set; } = "";
        }
    }
}
