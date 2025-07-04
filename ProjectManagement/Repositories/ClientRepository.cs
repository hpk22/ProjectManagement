using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class ClientRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        // ✅ Get all active clients using stored procedure
        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetAllActiveClients", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        clients.Add(new Client
                        {
                            ClientID = Convert.ToInt32(rdr["ClientID"]),
                            ClientName = rdr["ClientName"].ToString()
                        });
                    }
                }
            }

            return clients;
        }

        // ✅ Get client by ID using stored procedure
        public Client GetClientById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetClientById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientID", id);
                con.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return new Client
                        {
                            ClientID = Convert.ToInt32(rdr["ClientID"]),
                            ClientName = rdr["ClientName"].ToString()
                        };
                    }
                }
            }

            return null;
        }
    }
}
