using System.Security.Cryptography;

namespace Common.Libraries.Services.Authentication
{
    public class PasswordHashing : IPasswordHashing
    {
        public string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA3_256))
            {
                var hash = deriveBytes.GetBytes(32); //256 bit hash
                var saltAndHash = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, saltAndHash, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, saltAndHash, salt.Length, hash.Length);
                return Convert.ToBase64String(saltAndHash);

            }
        }
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var saltAndHash = Convert.FromBase64String(hashedPassword);
            var salt = new byte[16];
            Buffer.BlockCopy(saltAndHash, 0, salt, 0, salt.Length);
            var hash = new byte[32];
            Buffer.BlockCopy(saltAndHash, salt.Length, hash, 0, hash.Length);
            using (var derivedBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA3_256))
            {

                var computedHash = derivedBytes.GetBytes(32);
                return CryptographicOperations.FixedTimeEquals(computedHash, hash);
            }
        }
    }
}
