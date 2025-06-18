using System.Collections.Generic;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagementSystem.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly UserRepository repo = new UserRepository();

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(string role = null, string status = null)
        {
            var users = repo.GetAllUsers(role, status);
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            var user = repo.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(int id, User user)
        {
            var updatedUser = repo.UpdateUser(id, user);
            return Ok(updatedUser);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            var result = repo.DeleteUser(id);
            if (result)
                return Ok("User deleted.");
            else
                return BadRequest("Delete failed.");
        }

        [HttpPut]
        [Route("{id}/change-role")]
        public IHttpActionResult ChangeRole(int id, RoleChangeRequest request)
        {
            var result = repo.ChangeUserRole(id, request.RoleID);
            if (result)
                return Ok("Role updated.");
            else
                return BadRequest("Role change failed.");
        }
    }

    public class RoleChangeRequest
    {
        public int RoleID { get; set; }
    }
}
