using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjectManagement.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            string hashedEntered = HashPassword(enteredPassword);
            return string.Equals(hashedEntered, storedHashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
