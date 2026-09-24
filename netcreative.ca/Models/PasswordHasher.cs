using System;
using System.Security.Cryptography;

namespace netcreative.ca
{
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = DeriveHash(password, salt, Iterations);

            return string.Join("$", Prefix, Iterations.ToString(), Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool Verify(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }

            string[] parts = hashedPassword.Split('$');

            if (parts.Length != 4 || parts[0] != Prefix)
            {
                return false;
            }

            int iterations;

            if (!int.TryParse(parts[1], out iterations))
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);
            byte[] actualHash = DeriveHash(password, salt, iterations);

            return FixedTimeEquals(expectedHash, actualHash);
        }

        private static byte[] DeriveHash(string password, byte[] salt, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;

            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}
