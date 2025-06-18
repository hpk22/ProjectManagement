using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/documents")]
    public class DocumentController : ApiController
    {
        private readonly DocumentRepository repo = new DocumentRepository();

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent()) return BadRequest("Invalid format");

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                var buffer = await file.ReadAsByteArrayAsync();
                var path = HttpContext.Current.Server.MapPath("~/Uploads/" + filename);
                File.WriteAllBytes(path, buffer);

                repo.SaveDocument(new Document
                {
                    DocumentName = filename,
                    FilePath = "/Uploads/" + filename,
                    FileSize = buffer.Length,
                    FileType = Path.GetExtension(filename),
                    UploadDate = DateTime.Now,
                    UploadedBy = 1
                });
            }

            return Ok("Uploaded");
        }

        [HttpGet, Route("{id}/download")]
        public HttpResponseMessage Download(int id)
        {
            var doc = repo.GetDocumentById(id);
            var path = HttpContext.Current.Server.MapPath(doc.FilePath);
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

        [HttpDelete, Route("{id}")]
        public IHttpActionResult Delete(int id) => Ok(repo.DeleteDocument(id));
    }
}
