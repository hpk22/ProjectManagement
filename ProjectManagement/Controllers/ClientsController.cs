using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;
using ProjectManagement.Models;
using ProjectManagement.Repositories;
using System.Collections.Generic;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/clients")]
    public class ClientsController : ApiController
    {
        private readonly ClientRepository clientRepo = new ClientRepository();

        // ✅ GET: api/clients
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllClients()
        {
            List<Client> clients = clientRepo.GetAllClients();
            return Ok(clients);
        }

        // ✅ POST: api/clients/login
        [HttpPost]
        [Route("login")]
        public IHttpActionResult ClientLogin(ClientLoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and Password are required.");

            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 ClientID, ClientName
                                                      FROM Clients
                                                      WHERE Email = @Email AND Password = @Password AND Status = 'Active'", con))
            {
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);

                con.Open();
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var response = new
                    {
                        ClientID = rdr["ClientID"],
                        ClientName = rdr["ClientName"]
                    };
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }
    }

    public class ClientLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
