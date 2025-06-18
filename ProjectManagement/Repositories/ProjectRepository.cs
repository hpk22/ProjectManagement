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
            using (var cmd = new SqlCommand(@"SELECT * FROM Projects 
                WHERE (@status IS NULL OR Status = @status)
                  AND (@manager IS NULL OR ManagerID = @manager)
                  AND (@client IS NULL OR ClientID = @client)", con))
            {
                cmd.Parameters.AddWithValue("@status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@manager", (object)managerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@client", (object)clientId ?? DBNull.Value);
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
            using (var cmd = new SqlCommand("SELECT * FROM Projects WHERE ProjectID = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                return rdr.Read() ? MapProject(rdr) : null;
            }
        }

        public Project CreateProject(Project p)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                INSERT INTO Projects 
                (ProjectName, Description, StartDate, EndDate, ActualEndDate, Status, Priority, Budget, ClientID, ManagerID, CreatedBy, CreatedDate) 
                OUTPUT INSERTED.ProjectID 
                VALUES 
                (@name, @desc, @start, @end, @actual, @status, @priority, @budget, @client, @manager, @createdBy, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@name", p.ProjectName);
                cmd.Parameters.AddWithValue("@desc", p.Description);
                cmd.Parameters.AddWithValue("@start", p.StartDate);
                cmd.Parameters.AddWithValue("@end", (object)p.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@actual", (object)p.ActualEndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@status", p.Status);
                cmd.Parameters.AddWithValue("@priority", p.Priority);
                cmd.Parameters.AddWithValue("@budget", p.Budget);
                cmd.Parameters.AddWithValue("@client", p.ClientID);
                cmd.Parameters.AddWithValue("@manager", p.ManagerID);
                cmd.Parameters.AddWithValue("@createdBy", p.CreatedBy);
                con.Open();
                int newId = (int)cmd.ExecuteScalar();
                return GetProjectById(newId);
            }
        }

        public Project UpdateProject(int id, Project p)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                UPDATE Projects SET 
                    ProjectName=@name,
                    Description=@desc,
                    StartDate=@start,
                    EndDate=@end,
                    ActualEndDate=@actual,
                    Status=@status,
                    Priority=@priority,
                    Budget=@budget,
                    ClientID=@client,
                    ManagerID=@manager,
                    ModifiedBy=@modifiedBy,
                    ModifiedDate=GETDATE()
                WHERE ProjectID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", p.ProjectName);
                cmd.Parameters.AddWithValue("@desc", p.Description);
                cmd.Parameters.AddWithValue("@start", p.StartDate);
                cmd.Parameters.AddWithValue("@end", (object)p.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@actual", (object)p.ActualEndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@status", p.Status);
                cmd.Parameters.AddWithValue("@priority", p.Priority);
                cmd.Parameters.AddWithValue("@budget", p.Budget);
                cmd.Parameters.AddWithValue("@client", p.ClientID);
                cmd.Parameters.AddWithValue("@manager", p.ManagerID);
                cmd.Parameters.AddWithValue("@modifiedBy", p.ModifiedBy ?? (object)DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
                return GetProjectById(id);
            }
        }

        public bool DeleteProject(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("DELETE FROM Projects WHERE ProjectID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Project MapProject(SqlDataReader rdr)
        {
            return new Project
            {
                ProjectID = (int)rdr["ProjectID"],
                ProjectName = rdr["ProjectName"].ToString(),
                Description = rdr["Description"].ToString(),
                StartDate = (DateTime)(rdr["StartDate"] != DBNull.Value ? Convert.ToDateTime(rdr["StartDate"]) : (DateTime?)null),
                EndDate = rdr["EndDate"] != DBNull.Value ? Convert.ToDateTime(rdr["EndDate"]) : (DateTime?)null,
                ActualEndDate = rdr["ActualEndDate"] != DBNull.Value ? Convert.ToDateTime(rdr["ActualEndDate"]) : (DateTime?)null,
                Status = rdr["Status"].ToString(),
                Priority = rdr["Priority"].ToString(),
                Budget = rdr["Budget"] != DBNull.Value ? Convert.ToDecimal(rdr["Budget"]) : 0,
                ClientID = rdr["ClientID"] != DBNull.Value ? Convert.ToInt32(rdr["ClientID"]) : 0,
                ManagerID = rdr["ManagerID"] != DBNull.Value ? Convert.ToInt32(rdr["ManagerID"]) : 0
            };
        }


        public bool AddTeamMember(int projectId, int userId, string role)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
        INSERT INTO ProjectTeam (ProjectID, UserID, Role, JoinDate) 
        VALUES (@pid, @uid, @role, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@pid", projectId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@role", role);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<ProjectTeamMember> GetProjectTeam(int projectId)
        {
            var list = new List<ProjectTeamMember>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT UserID, Role, JoinDate, ExitDate FROM ProjectTeam WHERE ProjectID = @pid", con))
            {
                cmd.Parameters.AddWithValue("@pid", projectId);
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


    }
}
