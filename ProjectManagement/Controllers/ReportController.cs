using System;
using System.Web.Http;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportController : ApiController
    {
        private readonly ReportRepository repo = new ReportRepository();

        [HttpGet]
        [Route("project-status")]
        public IHttpActionResult GetProjectStatus(int projectId)
        {
            var data = repo.GetProjectStatus(projectId);
            return Ok(data);
        }

        [HttpGet]
        [Route("resource-utilization")]
        public IHttpActionResult GetResourceUtilization(DateTime startDate, DateTime endDate)
        {
            var data = repo.GetResourceUtilization(startDate, endDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("task-status")]
        public IHttpActionResult GetTaskStatus(int projectId)
        {
            var data = repo.GetTaskStatus(projectId);
            return Ok(data);
        }

        [HttpGet]
        [Route("timesheets")]
        public IHttpActionResult GetTimesheetSummary(DateTime startDate, DateTime endDate)
        {
            var data = repo.GetTimesheetSummary(startDate, endDate);
            return Ok(data);
        }
    }
}
