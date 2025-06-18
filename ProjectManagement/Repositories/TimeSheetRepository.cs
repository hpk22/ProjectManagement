using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class TimeSheetRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<TimeSheet> GetAllTimesheets(int? userId, int? projectId, DateTime? startDate, DateTime? endDate)
        {
            var list = new List<TimeSheet>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"SELECT * FROM TimeSheets
                WHERE (@user IS NULL OR UserID = @user)
                  AND (@project IS NULL OR ProjectID = @project)
                  AND (@start IS NULL OR WorkDate >= @start)
                  AND (@end IS NULL OR WorkDate <= @end)", con))
            {
                cmd.Parameters.AddWithValue("@user", (object)userId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@project", (object)projectId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@start", (object)startDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@end", (object)endDate ?? DBNull.Value);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TimeSheet
                    {
                        TimeSheetID = (int)rdr["TimeSheetID"],
                        UserID = (int)rdr["UserID"],
                        ProjectID = (int)rdr["ProjectID"],
                        TaskID = (int)rdr["TaskID"],
                        WorkDate = (DateTime)rdr["WorkDate"],
                        HoursWorked = (decimal)rdr["HoursWorked"],
                        Description = rdr["Description"]?.ToString(),
                        Status = rdr["Status"].ToString(),
                        SubmittedDate = (DateTime)rdr["SubmittedDate"],
                        ApprovedBy = rdr["ApprovedBy"] == DBNull.Value ? null : (int?)rdr["ApprovedBy"],
                        ApprovalDate = rdr["ApprovalDate"] == DBNull.Value ? null : (DateTime?)rdr["ApprovalDate"]
                    });
                }
            }
            return list;
        }

        public TimeSheet SubmitTimesheet(TimeSheet t)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"INSERT INTO TimeSheets
                (UserID, ProjectID, TaskID, WorkDate, HoursWorked, Description, Status, SubmittedDate)
                OUTPUT INSERTED.TimeSheetID
                VALUES (@uid, @pid, @tid, @date, @hours, @desc, 'Submitted', GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@uid", t.UserID);
                cmd.Parameters.AddWithValue("@pid", t.ProjectID);
                cmd.Parameters.AddWithValue("@tid", t.TaskID);
                cmd.Parameters.AddWithValue("@date", t.WorkDate);
                cmd.Parameters.AddWithValue("@hours", t.HoursWorked);
                cmd.Parameters.AddWithValue("@desc", (object)t.Description ?? DBNull.Value);
                con.Open();
                int id = (int)cmd.ExecuteScalar();
                return GetAllTimesheets(t.UserID, t.ProjectID, t.WorkDate, t.WorkDate).Find(x => x.TimeSheetID == id);
            }
        }

        public bool ApproveTimesheet(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("UPDATE TimeSheets SET Status='Approved', ApprovalDate=GETDATE() WHERE TimeSheetID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool RejectTimesheet(int id, string reason)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("UPDATE TimeSheets SET Status='Rejected', Description=@reason WHERE TimeSheetID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@reason", reason);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
