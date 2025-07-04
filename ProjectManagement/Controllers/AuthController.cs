using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Security;
using System.Web.Http.Cors;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/auth")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // ✅ POST: api/auth/register (Client-side hashed password)
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(UserRegister model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("usp_CreateUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password); // Already hashed by client
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@RoleID", model.RoleID);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    // If role is client, insert into Clients table too
                    if (model.RoleID == 4 && !string.IsNullOrEmpty(model.ClientName))
                    {
                        using (SqlCommand clientCmd = new SqlCommand("usp_CreateClient", con))
                        {
                            clientCmd.CommandType = CommandType.StoredProcedure;
                            clientCmd.Parameters.AddWithValue("@ClientName", model.ClientName);
                            clientCmd.Parameters.AddWithValue("@ContactPerson", model.ContactPerson);
                            clientCmd.Parameters.AddWithValue("@Email", model.Email);
                            clientCmd.Parameters.AddWithValue("@Phone", model.PhoneNumber);
                            clientCmd.Parameters.AddWithValue("@Address", model.Address ?? "");
                            clientCmd.Parameters.AddWithValue("@Password", model.Password); // Store hashed password
                            clientCmd.ExecuteNonQuery();
                        }
                    }

                    return Ok("✅ User registered successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ POST: api/auth/login
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(UserLogin model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("usp_AuthenticateUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password); // Already hashed on frontend

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        int userId = Convert.ToInt32(rdr["UserID"]);
                        string username = rdr["Username"].ToString();
                        string email = rdr["Email"].ToString();
                        int roleId = Convert.ToInt32(rdr["RoleID"]);

                        string token = JwtManager.GenerateToken(username, userId, roleId);

                        return Ok(new
                        {
                            UserID = userId,
                            Username = username,
                            Email = email,
                            RoleID = roleId,
                            Token = token
                        });
                    }

                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/auth/reset-password (optional stub)
        [HttpPost]
        [Route("reset-password")]
        public IHttpActionResult ResetPassword(UserReset model)
        {
            return Ok("📧 If account exists, reset password link has been sent.");
        }
    }
}
