using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class UserRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<User> GetAllUsers(string role, string status)
        {
            var list = new List<User>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetAllUsers", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@role", string.IsNullOrEmpty(role) ? (object)DBNull.Value : role);
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new User
                    {
                        UserID = (int)rdr["UserID"],
                        Username = rdr["Username"].ToString(),
                        FirstName = rdr["FirstName"].ToString(),
                        LastName = rdr["LastName"].ToString(),
                        Email = rdr["Email"].ToString(),
                        RoleID = (int)rdr["RoleID"],
                        Status = rdr["Status"].ToString()
                    });
                }
            }
            return list;
        }

        public User GetUserById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetUserById", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    return new User
                    {
                        UserID = id,
                        Username = rdr["Username"].ToString(),
                        FirstName = rdr["FirstName"].ToString(),
                        LastName = rdr["LastName"].ToString(),
                        Email = rdr["Email"].ToString(),
                        RoleID = (int)rdr["RoleID"],
                        Status = rdr["Status"].ToString()
                    };
                }
            }
            return null;
        }

        public User UpdateUser(int id, User user)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_UpdateUser", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", id);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0) return null;
                return GetUserById(id);
            }
        }

        public bool DeleteUser(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_DeleteUser", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ChangeUserRole(int id, int newRole)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_ChangeUserRole", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", id);
                cmd.Parameters.AddWithValue("@RoleID", newRole);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
