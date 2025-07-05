using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [Authorize]
    [RoutePrefix("api/projects")]
    public class ProjectController : ApiController
    {
        private readonly ProjectRepository repo = new ProjectRepository();
        private readonly UserRepository userRepo = new UserRepository();
        private readonly ClientRepository clientRepo = new ClientRepository();

        // ✅ GET: api/projects
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(string status = null, int? manager = null, int? client = null)
        {
            try
            {
                var projects = repo.GetAllProjects(status, manager, client);

                var enriched = projects.Select(p => new Project
                {
                    ProjectID = p.ProjectID,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Status = p.Status,
                    Priority = p.Priority,
                    Budget = p.Budget,
                    ManagerID = p.ManagerID,
                    ClientID = p.ClientID,
                    ManagerName = GetManagerName(p.ManagerID),
                    ClientName = GetClientName(p.ClientID)
                }).ToList();

                return Ok(enriched);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ GET by ID
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var project = repo.GetProjectById(id);
                if (project == null) return NotFound();

                var dto = new Project
                {
                    ProjectID = project.ProjectID,
                    ProjectName = project.ProjectName,
                    Description = project.Description,
                    Status = project.Status,
                    Priority = project.Priority,
                    Budget = project.Budget,
                    ManagerID = project.ManagerID,
                    ClientID = project.ClientID,
                    ManagerName = GetManagerName(project.ManagerID),
                    ClientName = GetClientName(project.ClientID)
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ POST: Create Project
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "2")]
        public IHttpActionResult Create(Project model)
        {
            try
            {
                model.CreatedBy = model.ManagerID;
                model.CreatedDate = DateTime.Now;

                var created = repo.CreateProject(model);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ PUT: Update Project
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "2")]
        public IHttpActionResult Update(int id, Project model)
        {
            try
            {
                var updated = repo.UpdateProject(id, model);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ GET Projects assigned to user
        [HttpGet]
        [Route("user/{userId}")]
        public IHttpActionResult GetProjectsByUser(int userId)
        {
            try
            {
                var cs = System.Configuration.ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                var projects = new List<Project>();

                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(@"
                    SELECT p.ProjectID, p.ProjectName 
                    FROM Projects p
                    INNER JOIN ProjectTeam pt ON pt.ProjectID = p.ProjectID
                    WHERE pt.UserID = @uid", con))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    con.Open();
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        projects.Add(new Project
                        {
                            ProjectID = (int)rdr["ProjectID"],
                            ProjectName = rdr["ProjectName"].ToString()
                        });
                    }
                }

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ DELETE: Delete project
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "2")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                return repo.DeleteProject(id)
                    ? Ok("Project deleted successfully.")
                    : (IHttpActionResult)BadRequest("Failed to delete project.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ POST: Add team member to project
        [HttpPost]
        [Route("{id}/team")]
        [Authorize(Roles = "2")]
        public IHttpActionResult AddTeamMember(int id, TeamAssignment model)
        {
            try
            {
                bool added = repo.AddTeamMember(id, model.UserID, model.Role);
                return added
                    ? Ok("Team member added.")
                    : (IHttpActionResult)BadRequest("Failed to add member.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ GET: Project team
        [HttpGet]
        [Route("{id}/team")]
        public IHttpActionResult GetTeam(int id)
        {
            try
            {
                var team = repo.GetProjectTeam(id);
                return Ok(team);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 🔧 Helpers
        private string GetManagerName(int userId)
        {
            var user = userRepo.GetUserById(userId);
            return user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown";
        }

        private string GetClientName(int clientId)
        {
            var client = clientRepo.GetClientById(clientId);
            return client?.ClientName ?? "Unknown";
        }

        private int GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            var userIdClaim = identity?.Claims.FirstOrDefault(c => c.Type == "UserID");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }

    public class TeamAssignment
    {
        public int UserID { get; set; }
        public string Role { get; set; }
    }
}
