using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class ResourceRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<Resource> GetAllResources(string type = null)
        {
            var list = new List<Resource>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetAllResources", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", (object)type ?? DBNull.Value);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(MapResource(rdr));
                }
            }
            return list;
        }

        public Resource GetResourceById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetResourceById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                return rdr.Read() ? MapResource(rdr) : null;
            }
        }

        public Resource CreateResource(Resource r)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_CreateResource", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", r.ResourceName);
                cmd.Parameters.AddWithValue("@type", r.ResourceType);
                cmd.Parameters.AddWithValue("@cost", (object)r.Cost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@avail", r.Availability);
                cmd.Parameters.AddWithValue("@user", (object)r.UserID ?? DBNull.Value);

                con.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                return GetResourceById(id);
            }
        }

        public Resource UpdateResource(int id, Resource r)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_UpdateResource", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", r.ResourceName);
                cmd.Parameters.AddWithValue("@type", r.ResourceType);
                cmd.Parameters.AddWithValue("@cost", (object)r.Cost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@avail", r.Availability);
                cmd.Parameters.AddWithValue("@user", (object)r.UserID ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                return GetResourceById(id);
            }
        }

        public bool DeleteResource(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_DeleteResource", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Resource MapResource(SqlDataReader rdr)
        {
            return new Resource
            {
                ResourceID = (int)rdr["ResourceID"],
                ResourceName = rdr["ResourceName"].ToString(),
                ResourceType = rdr["ResourceType"].ToString(),
                Cost = rdr["Cost"] != DBNull.Value ? Convert.ToDecimal(rdr["Cost"]) : (decimal?)null,
                Availability = Convert.ToDecimal(rdr["Availability"]),
                UserID = rdr["UserID"] == DBNull.Value ? null : (int?)rdr["UserID"]
            };
        }
    }
}
