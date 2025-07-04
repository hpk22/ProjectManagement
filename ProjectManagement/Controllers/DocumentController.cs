using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/documents")]
    public class DocumentController : ApiController
    {
        private readonly DocumentRepository repo = new DocumentRepository();

        // POST: api/documents
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Invalid format. Expected multipart/form-data.");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                string projectIdStr = null, taskIdStr = null, description = null;
                string filename = null;
                byte[] buffer = null;

                foreach (var content in provider.Contents)
                {
                    var name = content.Headers.ContentDisposition.Name?.Trim('"');

                    switch (name)
                    {
                        case "projectId":
                            projectIdStr = await content.ReadAsStringAsync();
                            break;
                        case "taskId":
                            taskIdStr = await content.ReadAsStringAsync();
                            break;
                        case "description":
                            description = await content.ReadAsStringAsync();
                            break;
                        case "file":
                            filename = content.Headers.ContentDisposition.FileName?.Trim('"');
                            buffer = await content.ReadAsByteArrayAsync();

                            if (string.IsNullOrWhiteSpace(filename) || buffer.Length == 0)
                                return BadRequest("Missing or invalid file.");

                            string savePath = HttpContext.Current.Server.MapPath("~/Uploads/" + filename);
                            File.WriteAllBytes(savePath, buffer);
                            break;
                    }
                }

                // Get uploader ID from session
                var sessionUserId = HttpContext.Current?.Session?["UserID"];
                if (sessionUserId == null)
                    return Unauthorized();

                int uploaderId = Convert.ToInt32(sessionUserId);

                int? projectId = null, taskId = null;

                if (!string.IsNullOrWhiteSpace(projectIdStr) && int.TryParse(projectIdStr, out int pid))
                    projectId = pid;

                if (!string.IsNullOrWhiteSpace(taskIdStr) && int.TryParse(taskIdStr, out int tid))
                    taskId = tid;

                if (filename == null || buffer == null)
                    return BadRequest("File not received properly.");

                var doc = new Document
                {
                    ProjectID = projectId,
                    TaskID = taskId,
                    Description = description,
                    UploadedBy = uploaderId,
                    DocumentName = filename,
                    FilePath = "/Uploads/" + filename,
                    FileSize = buffer.Length,
                    FileType = Path.GetExtension(filename),
                    UploadDate = DateTime.Now
                };

                repo.SaveDocument(doc);

                return Ok("✅ Document uploaded successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("❌ Upload failed: " + ex.Message));
            }
        }

        // GET: api/documents/all
        [HttpGet, Route("all")]
        public IHttpActionResult GetAllDocuments()
        {
            try
            {
                var documents = repo.GetAllDocuments();
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/documents/{id}/download
        [HttpGet, Route("{id}/download")]
        public HttpResponseMessage Download(int id)
        {
            try
            {
                var doc = repo.GetDocumentById(id);
                if (doc == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

                var path = HttpContext.Current.Server.MapPath(doc.FilePath);
                if (!File.Exists(path)) return new HttpResponseMessage(HttpStatusCode.NotFound);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(path))
                };
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = doc.DocumentName
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        // DELETE: api/documents/{id}
        [HttpDelete, Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                bool success = repo.DeleteDocument(id);
                return success ? Ok("🗑️ Document deleted.") : (IHttpActionResult)NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
