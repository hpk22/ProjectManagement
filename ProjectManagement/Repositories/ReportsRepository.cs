using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class ReportRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<ProjectStatusReport> GetProjectStatus(int projectId)
        {
            var list = new List<ProjectStatusReport>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT Status, COUNT(*) AS Total FROM Projects WHERE ProjectID = @pid GROUP BY Status", con))
            {
                cmd.Parameters.AddWithValue("@pid", projectId);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new ProjectStatusReport
                    {
                        Status = rdr["Status"].ToString(),
                        Total = Convert.ToInt32(rdr["Total"])
                    });
                }
            }
            return list;
        }

        public List<ResourceUtilizationReport> GetResourceUtilization(DateTime start, DateTime end)
        {
            var list = new List<ResourceUtilizationReport>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"SELECT R.ResourceName, RA.AllocationPercentage, RA.AllocationStartDate, RA.AllocationEndDate
                                               FROM Resources R
                                               JOIN ResourceAllocations RA ON R.ResourceID = RA.ResourceID
                                               WHERE RA.AllocationStartDate >= @start AND RA.AllocationEndDate <= @end", con))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new ResourceUtilizationReport
                    {
                        ResourceName = rdr["ResourceName"].ToString(),
                        AllocationPercentage = Convert.ToDecimal(rdr["AllocationPercentage"]),
                        AllocationStartDate = Convert.ToDateTime(rdr["AllocationStartDate"]),
                        AllocationEndDate = Convert.ToDateTime(rdr["AllocationEndDate"])
                    });
                }
            }
            return list;
        }

        public List<TaskStatusReport> GetTaskStatus(int projectId)
        {
            var list = new List<TaskStatusReport>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT Status, COUNT(*) AS Count FROM Tasks WHERE ProjectID = @pid GROUP BY Status", con))
            {
                cmd.Parameters.AddWithValue("@pid", projectId);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TaskStatusReport
                    {
                        Status = rdr["Status"].ToString(),
                        Count = Convert.ToInt32(rdr["Count"])
                    });
                }
            }
            return list;
        }

        public List<TimesheetSummaryReport> GetTimesheetSummary(DateTime start, DateTime end)
        {
            var list = new List<TimesheetSummaryReport>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT UserID, SUM(HoursWorked) AS TotalHours FROM TimeSheets WHERE WorkDate BETWEEN @start AND @end GROUP BY UserID", con))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TimesheetSummaryReport
                    {
                        UserID = Convert.ToInt32(rdr["UserID"]),
                        TotalHours = Convert.ToDecimal(rdr["TotalHours"])
                    });
                }
            }
            return list;
        }
    }
}
