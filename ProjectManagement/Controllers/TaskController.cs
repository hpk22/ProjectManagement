using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/tasks")]
    public class TaskController : ApiController
    {
        private readonly TaskRepository repo = new TaskRepository();

        // GET: api/tasks
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                return Ok(repo.GetAll());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/tasks/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var task = repo.GetById(id);
                return task != null ? Ok(task) : (IHttpActionResult)NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/tasks/byproject/{projectId}
        [HttpGet]
        [Route("byproject/{projectId}")]
        public IHttpActionResult GetTasksByProject(int projectId)
        {
            try
            {
                var tasks = repo.GetTasksByProjectId(projectId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/tasks
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(Task model)
        {
            try
            {
                var created = repo.Create(model);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/tasks/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(int id, Task model)
        {
            try
            {
                var existingTask = repo.GetById(id);
                if (existingTask == null)
                    return NotFound();

                if (!string.IsNullOrWhiteSpace(model.TaskName)) existingTask.TaskName = model.TaskName;
                if (!string.IsNullOrWhiteSpace(model.Status)) existingTask.Status = model.Status;
                if (model.AssignedTo != 0) existingTask.AssignedTo = model.AssignedTo;

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
            try
            {
                return repo.Delete(id) ? Ok("Deleted") : (IHttpActionResult)BadRequest("Delete failed");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/tasks/user/{userId}
        [HttpGet]
        [Route("user/{userId}")]
        public IHttpActionResult GetTasksByUser(int userId)
        {
            try
            {
                var tasks = repo.GetTasksByUserId(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/tasks/{id}/status
        [HttpPut]
        [Route("{id}/status")]
        public IHttpActionResult UpdateStatus(int id, [FromBody] StatusUpdateModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Status))
                    return BadRequest("Status cannot be empty.");

                bool updated = repo.UpdateTaskStatus(id, model.Status);
                return updated ? Ok("Status updated successfully") : (IHttpActionResult)BadRequest("Failed to update status");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // (You can add dependency-related POSTs here later with try-catch too)
    }

    public class StatusUpdateModel
    {
        public string Status { get; set; }
    }
}
