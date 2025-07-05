using System.Collections.Generic;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;
using System.Web.Http.Cors;
using System.Linq;
using System.Security.Claims;
using System;

namespace ProjectManagementSystem.Controllers
{
    [RoutePrefix("api/users")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly UserRepository repo = new UserRepository();

        // GET: api/users
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(string role = null, string status = null)
        {
            try
            {
                var users = repo.GetAllUsers(role, status);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/users/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var user = repo.GetUserById(id);
                return user == null ? (IHttpActionResult)NotFound() : Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/users/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update(int id, User user)
        {
            try
            {
                var principal = User as ClaimsPrincipal;
                string claimUserId = principal?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                string claimRoleId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                bool isAdmin = claimRoleId == "1";
                bool isSameUser = claimUserId == id.ToString();

                // 🔓 Auth check placeholder (enable when enforcing)
                // if (!isAdmin && !isSameUser) return Unauthorized();

                var updatedUser = repo.UpdateUser(id, user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/users/5/change-role
        [HttpPut]
        [Route("{id:int}/change-role")]
        public IHttpActionResult ChangeRole(int id, RoleChangeRequest request)
        {
            try
            {
                if (!IsAdmin()) return Unauthorized();

                var result = repo.ChangeUserRole(id, request.RoleID);
                return result ? Ok("Role updated.") : (IHttpActionResult)BadRequest("Role change failed.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/users/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                if (!IsAdmin()) return Unauthorized();

                var result = repo.DeleteUser(id);
                return result ? Ok("User deleted.") : (IHttpActionResult)BadRequest("Delete failed.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ✅ Helpers
        private bool IsAdmin()
        {
            var principal = User as ClaimsPrincipal;
            var claimRoleId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return claimRoleId == "1";
        }
    }

    public class RoleChangeRequest
    {
        public int RoleID { get; set; }
    }
}
