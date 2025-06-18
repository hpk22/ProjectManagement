using System.Collections.Generic;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/tasks")]
    public class TaskController : ApiController
    {
        private readonly TaskRepository repo = new TaskRepository();

        // GET: api/tasks
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            return Ok(repo.GetAll());
        }

        // GET: api/tasks/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            var task = repo.GetById(id);
            return task != null ? Ok(task) : (IHttpActionResult)NotFound();
        }

        // POST: api/tasks
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(Task model)
        {
            var created = repo.Create(model);
            return Ok(created);
        }

        // PUT: api/tasks/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(int id, Task model)
        {
            var updated = repo.Update(id, model);
            return Ok(updated);
        }

        // DELETE: api/tasks/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            return repo.Delete(id) ? Ok("Deleted") : (IHttpActionResult)BadRequest("Delete failed");
        }

        // POST: api/tasks/{id}/dependencies
        [HttpPost]
        [Route("{id}/dependencies")]
        public IHttpActionResult AddDependency(int id, [FromBody] int dependsOnTaskId)
        {
            bool added = repo.AddDependency(id, dependsOnTaskId);
            return added ? Ok("Dependency added") : (IHttpActionResult)BadRequest("Failed to add dependency");
        }

        // GET: api/tasks/{id}/dependencies
        [HttpGet]
        [Route("{id}/dependencies")]
        public IHttpActionResult GetDependencies(int id)
        {
            var deps = repo.GetDependencies(id);
            return Ok(deps);
        }
    }
}
