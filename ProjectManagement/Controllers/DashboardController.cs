using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagementSystem.Controllers
{
    [Authorize]
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly DashboardRepository repo = new DashboardRepository();

        [HttpGet]
        [Route("stats")]
        public IHttpActionResult GetDashboardStats([FromUri] int roleId)
        {
            var stats = repo.GetDashboardStats(roleId);
            return Ok(stats);
        }
    }
}
