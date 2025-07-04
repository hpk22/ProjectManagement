using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class DashboardRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public DashboardStats GetDashboardStats(int roleId)
        {
            var stats = new DashboardStats();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Total Projects
                using (SqlCommand cmd = new SqlCommand("sp_GetTotalProjects", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    stats.TotalProjects = (int)cmd.ExecuteScalar();
                }

                // Total Team Members
                using (SqlCommand cmd = new SqlCommand("sp_GetTotalTeamMembers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    stats.TotalTeamMembers = (int)cmd.ExecuteScalar();
                }

                // Pending Tasks
                using (SqlCommand cmd = new SqlCommand("sp_GetPendingTasks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    stats.PendingTasks = (int)cmd.ExecuteScalar();
                }

                // Submitted Timesheets
                if (roleId == 2 || roleId == 3)
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetSubmittedTimesheets", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        stats.SubmittedTimesheets = (int)cmd.ExecuteScalar();
                    }
                }

                // Documents
                using (SqlCommand cmd = new SqlCommand("sp_GetTotalDocuments", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    stats.TotalDocuments = (int)cmd.ExecuteScalar();
                }

                // Clients
                using (SqlCommand cmd = new SqlCommand("sp_GetTotalClients", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    stats.TotalClients = (int)cmd.ExecuteScalar();
                }
            }

            return stats;
        }
    }
}
