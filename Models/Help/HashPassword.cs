using System.Security.Cryptography;
using System.Text;

namespace VFM.Models.Help
{
    public class HashPassword
    {
        public static string Hash(string password)
        {
            string Salt = generateSalt();
            password += Salt;

            password = generateHash(password);

            password += Salt;

            return password;
        }

        public static bool ComparePasswords(string HashPassword, string Password)
        {
            string Salt = getSalt(HashPassword);

            Password += Salt;
            Password = generateHash(Password);
            Password += Salt;

            bool a = HashPassword == Password;
            return a;
        }

        private static string generateHash(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            string hashString = BitConverter.ToString(hash).Replace("-", "");
            return hashString;
        }

        private static string generateSalt()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();
            int length = 57;

            string Salt = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            Salt = generateHash(Salt);
            return Salt.Substring(0, 20);
        }
        private static string getSalt(string hash)
        {
            int first = hash.Length - 20;

            string Salt = hash.Substring(first);

            return Salt;
        }
    }
}
