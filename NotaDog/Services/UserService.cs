using System;
using System.Security.Cryptography;
using System.Text;

namespace NotaDog.Services
{
    public static class UserService
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin";

        public static bool IsUserRegistered()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.Username);
        }

        public static void RegisterUser(string username, string password)
        {
            // Générer un sel
            string salt = GenerateSalt();

            // Hacher le mot de passe avec le sel
            string hashedPassword = HashPassword(password, salt);

            // Stocker les informations dans les paramètres
            Properties.Settings.Default.Username = username;
            Properties.Settings.Default.PasswordHash = hashedPassword;
            Properties.Settings.Default.PasswordSalt = salt;
            Properties.Settings.Default.Save();
        }

        public static bool ValidateUser(string username, string password)
        {
            if (username == AdminUsername && password == AdminPassword)
            {
                return true; // Authentification réussie pour l'administrateur
            }

            string storedUsername = Properties.Settings.Default.Username;
            string storedPasswordHash = Properties.Settings.Default.PasswordHash;
            string storedSalt = Properties.Settings.Default.PasswordSalt;

            if (username == storedUsername)
            {
                string hashedPassword = HashPassword(password, storedSalt);
                return hashedPassword == storedPasswordHash;
            }

            return false;
        }

        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
