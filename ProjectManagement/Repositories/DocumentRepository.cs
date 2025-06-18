using System;
using System.Configuration;
using System.Data.SqlClient;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories
{
    public class DocumentRepository
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public void SaveDocument(Document doc)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"INSERT INTO Documents 
                (ProjectID, TaskID, DocumentName, FilePath, FileSize, FileType, UploadedBy, UploadDate, Description)
                VALUES (@project, @task, @name, @path, @size, @type, @by, @date, @desc)", con))
            {
                cmd.Parameters.AddWithValue("@project", (object)doc.ProjectID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@task", (object)doc.TaskID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@name", doc.DocumentName);
                cmd.Parameters.AddWithValue("@path", doc.FilePath);
                cmd.Parameters.AddWithValue("@size", doc.FileSize);
                cmd.Parameters.AddWithValue("@type", doc.FileType);
                cmd.Parameters.AddWithValue("@by", doc.UploadedBy);
                cmd.Parameters.AddWithValue("@date", doc.UploadDate);
                cmd.Parameters.AddWithValue("@desc", (object)doc.Description ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Document GetDocumentById(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT * FROM Documents WHERE DocumentID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    return new Document
                    {
                        DocumentID = (int)rdr["DocumentID"],
                        DocumentName = rdr["DocumentName"].ToString(),
                        FilePath = rdr["FilePath"].ToString(),
                        FileSize = (int)rdr["FileSize"],
                        FileType = rdr["FileType"].ToString(),
                        UploadedBy = (int)rdr["UploadedBy"],
                        UploadDate = (DateTime)rdr["UploadDate"],
                        Description = rdr["Description"]?.ToString()
                    };
                }
                return null;
            }
        }

        public bool DeleteDocument(int id)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("DELETE FROM Documents WHERE DocumentID=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
