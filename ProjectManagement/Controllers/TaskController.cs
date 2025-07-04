using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    //[Authorize]
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

        // ✅ GET: api/tasks/byproject/{projectId}
        [HttpGet]
        [Route("byproject/{projectId}")]
        public IHttpActionResult GetTasksByProject(int projectId)
        {
            var tasks = repo.GetTasksByProjectId(projectId); // Make sure this method exists in the repository
            return Ok(tasks);
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
        // PUT: api/tasks/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(int id, Task model)
        {
            try
            {
                // Fetch the existing task
                var existingTask = repo.GetById(id);
                if (existingTask == null)
                    return NotFound();

                // Only update fields that are provided (check for nulls/defaults)
                if (!string.IsNullOrWhiteSpace(model.TaskName)) existingTask.TaskName = model.TaskName;
                if (!string.IsNullOrWhiteSpace(model.Status)) existingTask.Status = model.Status;
                if (model.AssignedTo != 0) existingTask.AssignedTo = model.AssignedTo;

                // ⚠️ Do NOT overwrite dates, completion, priority etc. unless explicitly intended

                var updated = repo.Update(id, existingTask);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // DELETE: api/tasks/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            return repo.Delete(id) ? Ok("Deleted") : (IHttpActionResult)BadRequest("Delete failed");
        }

        // GET: api/tasks/user/{userId}
        [HttpGet]
        [Route("user/{userId}")]
        public IHttpActionResult GetTasksByUser(int userId)
        {
            var tasks = repo.GetTasksByUserId(userId);
            return Ok(tasks);
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
