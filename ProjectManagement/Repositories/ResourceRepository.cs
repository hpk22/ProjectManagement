using System;
using System.Collections.Generic;
using System.Configuration;
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
            using (var cmd = new SqlCommand("SELECT * FROM Resources WHERE (@type IS NULL OR ResourceType = @type)", con))
            {
                cmd.Parameters.AddWithValue("@type", (object)type ?? DBNull.Value);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new Resource
                    {
                        ResourceID = (int)rdr["ResourceID"],
                        ResourceName = rdr["ResourceName"].ToString(),
                        ResourceType = rdr["ResourceType"].ToString(),
                        Cost = rdr["Cost"] != DBNull.Value ? Convert.ToDecimal(rdr["Cost"]) : (decimal?)null,
                        Availability = Convert.ToDecimal(rdr["Availability"]),
                        UserID = rdr["UserID"] == DBNull.Value ? null : (int?)rdr["UserID"]
                    });
                }
            }
            return list;
        }

        public Resource GetResourceById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT * FROM Resources WHERE ResourceID = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
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
                return null;
            }
        }

        public Resource CreateResource(Resource r)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"INSERT INTO Resources 
                (ResourceName, ResourceType, Cost, Availability, UserID)
                OUTPUT INSERTED.ResourceID 
                VALUES (@name, @type, @cost, @avail, @user)", con))
            {
                cmd.Parameters.AddWithValue("@name", r.ResourceName);
                cmd.Parameters.AddWithValue("@type", r.ResourceType);
                cmd.Parameters.AddWithValue("@cost", (object)r.Cost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@avail", r.Availability);
                cmd.Parameters.AddWithValue("@user", (object)r.UserID ?? DBNull.Value);
                con.Open();
                int id = (int)cmd.ExecuteScalar();
                return GetResourceById(id);
            }
        }

        public Resource UpdateResource(int id, Resource r)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"UPDATE Resources SET 
                ResourceName=@name, ResourceType=@type, Cost=@cost, Availability=@avail, UserID=@user 
                WHERE ResourceID=@id", con))
            {
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
            using (var cmd = new SqlCommand("DELETE FROM Resources WHERE ResourceID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
