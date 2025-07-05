using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class TaskRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<Task> GetAll()
        {
            var list = new List<Task>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetAllTasks", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var task = MapTask(rdr);
                    task.ProjectName = rdr["ProjectName"].ToString();
                    task.AssigneeName = rdr["AssigneeName"].ToString();
                    list.Add(task);
                }
            }
            return list;
        }

        public Task GetById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetTaskById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                return rdr.Read() ? MapTask(rdr) : null;
            }
        }

        public Task Create(Task t)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_CreateTask", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmd.Parameters.AddWithValue("@TaskName", t.TaskName);
                cmd.Parameters.AddWithValue("@Description", t.Description);
                cmd.Parameters.AddWithValue("@StartDate", t.StartDate);
                cmd.Parameters.AddWithValue("@DueDate", t.DueDate);
                cmd.Parameters.AddWithValue("@CompletionDate", ToDbValue(t.CompletionDate));
                cmd.Parameters.AddWithValue("@Status", t.Status);
                cmd.Parameters.AddWithValue("@Priority", t.Priority);
                cmd.Parameters.AddWithValue("@EstimatedHours", t.EstimatedHours);
                cmd.Parameters.AddWithValue("@ActualHours", (object)t.ActualHours ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AssignedTo", t.AssignedTo);
                cmd.Parameters.AddWithValue("@CreatedBy", t.CreatedBy);
                con.Open();
                int newId = (int)cmd.ExecuteScalar();
                return GetById(newId);
            }
        }

        public Task Update(int id, Task t)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_UpdateTask", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskID", id);
                cmd.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmd.Parameters.AddWithValue("@TaskName", (object)t.TaskName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)t.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", ToDbValue(t.StartDate));
                cmd.Parameters.AddWithValue("@DueDate", ToDbValue(t.DueDate));
                cmd.Parameters.AddWithValue("@CompletionDate", ToDbValue(t.CompletionDate));
                cmd.Parameters.AddWithValue("@Status", (object)t.Status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Priority", (object)t.Priority ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EstimatedHours", t.EstimatedHours);
                cmd.Parameters.AddWithValue("@ActualHours", (object)t.ActualHours ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AssignedTo", t.AssignedTo);
                cmd.Parameters.AddWithValue("@ModifiedBy", (object)t.ModifiedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedDate", ToDbValue(t.ModifiedDate));
                con.Open();
                cmd.ExecuteNonQuery();
                return GetById(id);
            }
        }

        public bool Delete(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_DeleteTask", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskID", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Task> GetTasksByUserId(int userId)
        {
            var list = new List<Task>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetTasksByUserId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(MapTask(rdr));
                }
            }
            return list;
        }

        

        public List<Task> GetTasksByProjectId(int projectId)
        {
            var list = new List<Task>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_GetTasksByProjectID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectID", projectId);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(MapTask(rdr));
                }
            }
            return list;
        }

        // ✅ NEW: Update only status using sp_UpdateTaskStatus
        public bool UpdateTaskStatus(int taskId, string status)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_UpdateTaskStatus", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskID", taskId);
                cmd.Parameters.AddWithValue("@Status", status);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private object ToDbValue(DateTime? date)
        {
            return (!date.HasValue || date.Value < new DateTime(1753, 1, 1)) ? DBNull.Value : (object)date.Value;
        }

        private Task MapTask(SqlDataReader rdr)
        {
            return new Task
            {
                TaskID = (int)rdr["TaskID"],
                ProjectID = (int)rdr["ProjectID"],
                TaskName = rdr["TaskName"].ToString(),
                Description = rdr["Description"].ToString(),
                StartDate = (DateTime)rdr["StartDate"],
                DueDate = (DateTime)rdr["DueDate"],
                CompletionDate = rdr["CompletionDate"] == DBNull.Value ? (DateTime?)null : (DateTime)rdr["CompletionDate"],
                Status = rdr["Status"].ToString(),
                Priority = rdr["Priority"].ToString(),
                EstimatedHours = Convert.ToDouble(rdr["EstimatedHours"]),
                ActualHours = rdr["ActualHours"] == DBNull.Value ? (double?)null : Convert.ToDouble(rdr["ActualHours"]),
                AssignedTo = (int)rdr["AssignedTo"],
                CreatedBy = (int)rdr["CreatedBy"],
                CreatedDate = (DateTime)rdr["CreatedDate"],
                ModifiedBy = rdr["ModifiedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(rdr["ModifiedBy"]),
                ModifiedDate = rdr["ModifiedDate"] == DBNull.Value ? (DateTime?)null : (DateTime)rdr["ModifiedDate"]
            };
        }
    }
}
