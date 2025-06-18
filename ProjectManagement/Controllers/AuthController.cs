using System;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ProjectManagement.Models;
using Microsoft.AspNetCore.Cors;
using System.Web.Http.Cors;  // ✅ Correct for Web API 2


namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/auth")]
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]

    public class AuthController : ApiController
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // POST: api/auth/register
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
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@RoleID", model.RoleID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return Ok("User registered successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/auth/login
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
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        return Ok(new AuthResponse
                        {
                            UserID = Convert.ToInt32(rdr["UserID"]),
                            Username = rdr["Username"].ToString(),
                            Email = rdr["Email"].ToString(),
                            RoleID = Convert.ToInt32(rdr["RoleID"]),
                            Token = Guid.NewGuid().ToString()
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

        // POST: api/auth/reset-password
        [HttpPost]
        [Route("reset-password")]
        public IHttpActionResult ResetPassword(UserReset model)
        {
            
            return Ok("Reset password link sent if account exists.");
        }
    }
}
