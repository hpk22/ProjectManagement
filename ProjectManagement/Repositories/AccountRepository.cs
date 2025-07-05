using System;
using System.Configuration;
using System.Data.SqlClient;
using ProjectManagement.Helpers;
using ProjectManagement.Utility;

namespace ProjectManagement.Repositories
{
    public class AccountRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public string ResetPassword(string email)
        {
            using (var con = new SqlConnection(cs))
            {
                con.Open();

                // ✅ Check if user exists
                using (var checkCmd = new SqlCommand("SELECT UserID FROM Users WHERE Email = @Email", con))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    var userId = checkCmd.ExecuteScalar();
                    if (userId == null)
                        return null;
                }

                // ✅ Generate & hash new easy password
                string newPassword = GenerateEasyPassword();
                string hashed = PasswordHelper.HashPassword(newPassword);

                // ✅ Update password in database
                using (var updateCmd = new SqlCommand("UPDATE Users SET Password = @Password WHERE Email = @Email", con))
                {
                    updateCmd.Parameters.AddWithValue("@Password", hashed);
                    updateCmd.Parameters.AddWithValue("@Email", email);
                    updateCmd.ExecuteNonQuery();
                }

                // ✅ Send password via email
                EmailSender.Send(email, "Your New Password - ProjectPro", $"Your new password is: {newPassword}");

                return newPassword;
            }
        }

        // ✅ Helper: Generate easy password like "Desk42"
        private string GenerateEasyPassword()
        {
            var rand = new Random();
            string[] words = { "Desk", "Star", "Fish", "Lamp", "Moon", "Note", "Lion", "Tree", "Cake", "Boat" };
            string word = words[rand.Next(words.Length)];
            int number = rand.Next(10, 99); // 2-digit number
            return word + number;
        }
    }
}
