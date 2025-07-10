using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            using (var cmd = new SqlCommand("sp_GetAllTimesheets", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", (object)userId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectID", (object)projectId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", (object)endDate ?? DBNull.Value);

                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TimeSheet
                    {
                        TimeSheetID = (int)rdr["TimeSheetID"],
                        UserID = (int)rdr["UserID"],
                        ProjectID = (int)rdr["ProjectID"],
                        TaskID = rdr["TaskID"] == DBNull.Value ? null : (int?)rdr["TaskID"],
                        TaskName = rdr["TaskName"] == DBNull.Value ? null : rdr["TaskName"].ToString(),
                        WorkDate = (DateTime)rdr["WorkDate"],
                        HoursWorked = (decimal)rdr["HoursWorked"],
                        Description = rdr["Description"]?.ToString(),
                        Status = rdr["Status"].ToString(),
                        SubmittedDate = (DateTime)rdr["SubmittedDate"],
                        ApprovedBy = rdr["ApprovedBy"] == DBNull.Value ? null : (int?)rdr["ApprovedBy"],
                        ApprovalDate = rdr["ApprovalDate"] == DBNull.Value ? null : (DateTime?)rdr["ApprovalDate"],
                        ProjectName = rdr["ProjectName"].ToString(),
                        UserName = rdr["Username"].ToString()
                    });
                }
            }
            return list;
        }

        public TimeSheet SubmitTimesheet(TimeSheet t)
        {
            int newId;
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_SubmitTimesheet", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", t.UserID);
                cmd.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmd.Parameters.AddWithValue("@TaskID",t.TaskID);
                cmd.Parameters.AddWithValue("@WorkDate", t.WorkDate);
                cmd.Parameters.AddWithValue("@HoursWorked", t.HoursWorked);
                cmd.Parameters.AddWithValue("@Description", (object)t.Description ?? DBNull.Value);

                var outParam = new SqlParameter("@NewID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outParam);

                con.Open();
                cmd.ExecuteNonQuery();
                newId = (int)outParam.Value;
            }

            return GetAllTimesheets(t.UserID, t.ProjectID, t.WorkDate, t.WorkDate).Find(x => x.TimeSheetID == newId);
        }

        public bool ApproveTimesheet(int id, int approverId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_ApproveTimesheet", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TimeSheetID", id);
                cmd.Parameters.AddWithValue("@ApproverID", approverId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool RejectTimesheet(int id, string reason, int approverId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_RejectTimesheet", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TimeSheetID", id);
                cmd.Parameters.AddWithValue("@Reason", reason);
                cmd.Parameters.AddWithValue("@ApproverID", approverId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}