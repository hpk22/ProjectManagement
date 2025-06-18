using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/projects")]
    public class ProjectController : ApiController
    {
        private readonly ProjectRepository repo = new ProjectRepository();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(string status = null, int? manager = null, int? client = null)
        {
            return Ok(repo.GetAllProjects(status, manager, client));
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            if (HttpContext.Current.Session["UserID"] == null)
                return Unauthorized();
            var project = repo.GetProjectById(id);
            return project != null ? Ok(project) : (IHttpActionResult)NotFound();
            
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(Project model)
        {
            var created = repo.CreateProject(model);
            return Ok(created);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(int id, Project model)
        {
            var updated = repo.UpdateProject(id, model);
            return Ok(updated);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            return repo.DeleteProject(id) ? Ok("Deleted") : (IHttpActionResult)BadRequest("Failed");
        }

        [HttpPost]
        [Route("{id}/team")]
        public IHttpActionResult AddTeamMember(int id, TeamAssignment model)
        {
            bool added = repo.AddTeamMember(id, model.UserID, model.Role);
            return added ? Ok("Team member added.") : (IHttpActionResult)BadRequest("Failed to add member.");
        }

        [HttpGet]
        [Route("{id}/team")]
        public IHttpActionResult GetTeam(int id)
        {
            var team = repo.GetProjectTeam(id);
            return Ok(team);
        }

    }

    public class TeamAssignment
    {
        public int UserID { get; set; }
        public string Role { get; set; }
    }
}

