using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    // Simple auth and file encryption helper using AES
    public static class AuthService
    {
        private static readonly string ConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StudentGrader", "CSharpCLI", "auth.cfg");

        public static void EnsureConfigDirectory()
        {
            var dir = Path.GetDirectoryName(ConfigFile);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        }

        public static bool HasPassword()
        {
            EnsureConfigDirectory();
            return File.Exists(ConfigFile);
        }

        public static void SetPassword(string password)
        {
            EnsureConfigDirectory();
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            File.WriteAllBytes(ConfigFile, bytes);
        }

        public static bool ValidatePassword(string password)
        {
            if (!HasPassword()) return false;
            var stored = File.ReadAllBytes(ConfigFile);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(stored, bytes);
        }

        public static void ResetAuth()
        {
            try
            {
                if (File.Exists(ConfigFile)) File.Delete(ConfigFile);
            }
            catch { }
        }

        // Helpers for user records: hash and verify using salted PBKDF2
        public static string HashPasswordForUser(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            using var derive = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var hash = derive.GetBytes(32);
            // store salt + hash as base64
            var data = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, data, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, data, salt.Length, hash.Length);
            return Convert.ToBase64String(data);
        }

        public static bool VerifyPasswordForUser(string password, string stored)
        {
            try
            {
                var data = Convert.FromBase64String(stored);
                var salt = new byte[16];
                Buffer.BlockCopy(data, 0, salt, 0, 16);
                var hash = new byte[data.Length - 16];
                Buffer.BlockCopy(data, 16, hash, 0, hash.Length);
                using var derive = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
                var candidate = derive.GetBytes(hash.Length);
                return CryptographicOperations.FixedTimeEquals(candidate, hash);
            }
            catch
            {
                return false;
            }
        }

        // Encryption helpers moved to Services/_AuthEncryption.cs and disabled here.
        // The methods below intentionally throw to prevent accidental use.
        public static void EncryptFileTo(string sourcePath, string destinationPath, string password)
        {
            throw new NotSupportedException("File encryption has been disabled. See Services/_AuthEncryption.cs if you need the original implementation.");
        }

        public static void DecryptFileTo(string sourcePath, string destinationPath, string password)
        {
            throw new NotSupportedException("File decryption has been disabled. See Services/_AuthEncryption.cs if you need the original implementation.");
        }
    }
}
