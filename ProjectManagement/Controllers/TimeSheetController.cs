using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
        {
            try
            {
                return Ok(repo.GetAllTimesheets(user, project, startDate, endDate));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Submit(TimeSheet ts)
        {
            try
            {
                return Ok(repo.SubmitTimesheet(ts));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("{id}/approve")]
        public IHttpActionResult Approve(int id)
        {
            try
            {
                int approverId = 2; // Replace with logic to get logged-in user (if using token)
                return Ok(repo.ApproveTimesheet(id, approverId));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

       


        [HttpPut, Route("{id}/reject")]
        public IHttpActionResult Reject(int id, [FromBody] string reason)
        {
            try
            {
                int approverId = 2; // Replace with dynamic approver ID later
                return Ok(repo.RejectTimesheet(id, reason, approverId));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
