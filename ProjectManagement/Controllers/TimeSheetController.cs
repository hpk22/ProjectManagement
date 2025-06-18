using System;
using System.Collections.Generic;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/timesheets")]
    public class TimeSheetController : ApiController
    {
        private readonly TimeSheetRepository repo = new TimeSheetRepository();

        [HttpGet, Route("")]
        public IHttpActionResult GetAll(int? user = null, int? project = null, DateTime? startDate = null, DateTime? endDate = null)
            => Ok(repo.GetAllTimesheets(user, project, startDate, endDate));

        [HttpPost, Route("")]
        public IHttpActionResult Submit(TimeSheet ts) => Ok(repo.SubmitTimesheet(ts));

        [HttpPut, Route("{id}/approve")]
        public IHttpActionResult Approve(int id) => Ok(repo.ApproveTimesheet(id));

        [HttpPut, Route("{id}/reject")]
        public IHttpActionResult Reject(int id, [FromBody] string reason) => Ok(repo.RejectTimesheet(id, reason));
    }
}