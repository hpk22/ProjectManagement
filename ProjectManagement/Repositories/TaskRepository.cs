using System;
using System.Collections.Generic;
using System.Configuration;
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
            using (var cmd = new SqlCommand("SELECT * FROM Tasks", con))
            {
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(MapTask(rdr));
                }
            }
            return list;
        }

        public Task GetById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT * FROM Tasks WHERE TaskID = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                return rdr.Read() ? MapTask(rdr) : null;
            }
        }

        public Task Create(Task t)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                INSERT INTO Tasks 
                (ProjectID, TaskName, Description, StartDate, DueDate, CompletionDate, Status, Priority, EstimatedHours, ActualHours, AssignedTo, CreatedBy, CreatedDate)
                OUTPUT INSERTED.TaskID
                VALUES 
                (@ProjectID, @TaskName, @Description, @StartDate, @DueDate, @CompletionDate, @Status, @Priority, @EstimatedHours, @ActualHours, @AssignedTo, @CreatedBy, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmd.Parameters.AddWithValue("@TaskName", t.TaskName);
                cmd.Parameters.AddWithValue("@Description", t.Description);
                cmd.Parameters.AddWithValue("@StartDate", t.StartDate);
                cmd.Parameters.AddWithValue("@DueDate", t.DueDate);
                cmd.Parameters.AddWithValue("@CompletionDate", (object)t.CompletionDate ?? DBNull.Value);
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
            using (var cmd = new SqlCommand(@"
                UPDATE Tasks SET
                    ProjectID=@ProjectID, TaskName=@TaskName, Description=@Description, StartDate=@StartDate, 
                    DueDate=@DueDate, CompletionDate=@CompletionDate, Status=@Status, Priority=@Priority,
                    EstimatedHours=@EstimatedHours, ActualHours=@ActualHours, AssignedTo=@AssignedTo,
                    ModifiedBy=@ModifiedBy, ModifiedDate=GETDATE()
                WHERE TaskID=@TaskID", con))
            {
                cmd.Parameters.AddWithValue("@TaskID", id);
                cmd.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmd.Parameters.AddWithValue("@TaskName", t.TaskName);
                cmd.Parameters.AddWithValue("@Description", t.Description);
                cmd.Parameters.AddWithValue("@StartDate", t.StartDate);
                cmd.Parameters.AddWithValue("@DueDate", t.DueDate);
                cmd.Parameters.AddWithValue("@CompletionDate", (object)t.CompletionDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", t.Status);
                cmd.Parameters.AddWithValue("@Priority", t.Priority);
                cmd.Parameters.AddWithValue("@EstimatedHours", t.EstimatedHours);
                cmd.Parameters.AddWithValue("@ActualHours", (object)t.ActualHours ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AssignedTo", t.AssignedTo);
                cmd.Parameters.AddWithValue("@ModifiedBy", t.ModifiedBy ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                return GetById(id);
            }
        }

        public bool Delete(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("DELETE FROM Tasks WHERE TaskID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool AddDependency(int taskId, int dependsOnTaskId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("INSERT INTO TaskDependencies (TaskID, DependsOnTaskID) VALUES (@taskId, @dependsOn)", con))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                cmd.Parameters.AddWithValue("@dependsOn", dependsOnTaskId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<TaskDependency> GetDependencies(int taskId)
        {
            var list = new List<TaskDependency>();
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT * FROM TaskDependencies WHERE TaskID = @taskId", con))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                con.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TaskDependency
                    {
                        DependencyID = (int)rdr["DependencyID"],
                        TaskID = (int)rdr["TaskID"],
                        DependsOnTaskID = (int)rdr["DependsOnTaskID"]
                    });
                }
            }
            return list;
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
