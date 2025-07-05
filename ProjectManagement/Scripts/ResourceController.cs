using System;
using System.Collections.Generic;
using System.Web.Http;
using ProjectManagement.Models;
using ProjectManagement.Repositories;

namespace ProjectManagement.Controllers
{
    [RoutePrefix("api/resources")]
    public class ResourceController : ApiController
    {
        private readonly ResourceRepository repo = new ResourceRepository();

        [HttpGet, Route("")]
        public IHttpActionResult GetAll(string type = null) => Ok(repo.GetAllResources(type));

        [HttpGet, Route("{id}")]
        public IHttpActionResult GetById(int id) => Ok(repo.GetResourceById(id));

        [HttpPost, Route("")]
        public IHttpActionResult Create(Resource resource) => Ok(repo.CreateResource(resource));

        [HttpPut, Route("{id}")]
        public IHttpActionResult Update(int id, Resource resource) => Ok(repo.UpdateResource(id, resource));

        [HttpDelete, Route("{id}")]
        public IHttpActionResult Delete(int id) => Ok(repo.DeleteResource(id));
    }
}