using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class ProjectRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<Project> GetAllProjects(string status, int? managerId, int? clientId)
        {
            var list = new List<Project>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetAllProjects", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@managerId", (object)managerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@clientId", (object)clientId ?? DBNull.Value);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(MapProject(rdr));
                }
            }
            return list;
        }

        public Project GetProjectById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetProjectById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                return rdr.Read() ? MapProject(rdr) : null;
            }
        }

        public Project CreateProject(Project p)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_CreateProject", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", p.ProjectName);
                cmd.Parameters.AddWithValue("@Description", p.Description);
                cmd.Parameters.AddWithValue("@StartDate", p.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object)p.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActualEndDate", (object)p.ActualEndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", p.Status);
                cmd.Parameters.AddWithValue("@Priority", p.Priority);
                cmd.Parameters.AddWithValue("@Budget", p.Budget);
                cmd.Parameters.AddWithValue("@ClientID", p.ClientID);
                cmd.Parameters.AddWithValue("@ManagerID", p.ManagerID);
                cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);
                con.Open();
                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                return GetProjectById(newId);
            }
        }

        public Project UpdateProject(int id, Project p)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_UpdateProject", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectID", id);
                cmd.Parameters.AddWithValue("@ProjectName", p.ProjectName);
                cmd.Parameters.AddWithValue("@Description", p.Description);
                cmd.Parameters.AddWithValue("@StartDate", p.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object)p.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ActualEndDate", (object)p.ActualEndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", p.Status);
                cmd.Parameters.AddWithValue("@Priority", p.Priority);
                cmd.Parameters.AddWithValue("@Budget", p.Budget);
                cmd.Parameters.AddWithValue("@ClientID", p.ClientID);
                cmd.Parameters.AddWithValue("@ManagerID", p.ManagerID);
                cmd.Parameters.AddWithValue("@ModifiedBy", p.ModifiedBy ?? (object)DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
                return GetProjectById(id);
            }
        }

        public bool DeleteProject(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_DeleteProject", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectID", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool AddTeamMember(int projectId, int userId, string role)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_AddTeamMember", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectID", projectId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Role", role);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<ProjectTeamMember> GetProjectTeam(int projectId)
        {
            var list = new List<ProjectTeamMember>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetProjectTeam", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectID", projectId);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new ProjectTeamMember
                    {
                        UserID = (int)rdr["UserID"],
                        Role = rdr["Role"].ToString(),
                        JoinDate = Convert.ToDateTime(rdr["JoinDate"]),
                        ExitDate = rdr["ExitDate"] == DBNull.Value ? null : (DateTime?)rdr["ExitDate"]
                    });
                }
            }
            return list;
        }

        private Project MapProject(SqlDataReader rdr)
        {
            return new Project
            {
                ProjectID = (int)rdr["ProjectID"],
                ProjectName = rdr["ProjectName"].ToString(),
                Description = rdr["Description"].ToString(),
                StartDate = rdr["StartDate"] != DBNull.Value ? Convert.ToDateTime(rdr["StartDate"]) : (DateTime?)null,
                EndDate = rdr["EndDate"] != DBNull.Value ? Convert.ToDateTime(rdr["EndDate"]) : (DateTime?)null,
                ActualEndDate = rdr["ActualEndDate"] != DBNull.Value ? Convert.ToDateTime(rdr["ActualEndDate"]) : (DateTime?)null,
                Status = rdr["Status"].ToString(),
                Priority = rdr["Priority"].ToString(),
                Budget = rdr["Budget"] != DBNull.Value ? Convert.ToDecimal(rdr["Budget"]) : 0,
                ClientID = rdr["ClientID"] != DBNull.Value ? Convert.ToInt32(rdr["ClientID"]) : 0,
                ManagerID = rdr["ManagerID"] != DBNull.Value ? Convert.ToInt32(rdr["ManagerID"]) : 0
            };
        }
    }
}
