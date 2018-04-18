using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VisXpression3Builder.Api.VXB;
using VisXpression3Builder.Lib;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Api.Controllers
{
    [RoutePrefix("api/UserDefinedFunctions")]
    public class UserDefinedFunctionsController : ApiController
    {
        private LibraryFacade VxbFacade;

        public UserDefinedFunctionsController()
        {
            VxbFacade = new LibraryFacade(new UserDefinedFunctionsRepository(), new StaticFunctionsRepository(), new DomainFunctionsRepository());
        }

        [ResponseType(typeof(IEnumerable<FunctionDeclaration>))]
        public IHttpActionResult Get()
        {
            return Ok(VxbFacade.GetUserDefinedFunctions());
        }

        [Route("{name}/Graph")]
        [ResponseType(typeof(D3NEGraph))]
        public IHttpActionResult GetGraph(string name)
        {
            var function = VxbFacade.FunctionsFacade.GetFunctionGraph(name);
            if (function == null) return NotFound();
            return Ok(function);
        }

        [Route("{name}/Graph")]
        public IHttpActionResult PostGraph([FromUri] string name, [FromBody]D3NEGraph graph)
        {
            VxbFacade.CreateUserDefinedFunction(name, graph, null);
            return Created("", graph);
        }

        [Route("{name}/Graph")]
        public IHttpActionResult PutGraph([FromUri] string name, [FromBody]D3NEGraph graph)
        {
            var updated = VxbFacade.UpdateUserDefinedFunction(name, graph, null);
            if (updated == null) return NotFound();
            return Ok(graph);
        }

        [Route("{name}")]
        public IHttpActionResult Delete(string name)
        {
            var found = VxbFacade.DeleteUserDefinedFunction(name);
            if (!found) return NotFound();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{name}/ExecutionRequest"), HttpPost]
        public IHttpActionResult PostExecutionRequest([FromUri]string name, [FromBody]FunctionExecutionRequest request)
        {
            var result = VxbFacade.ExecuteUserDefinedFunction(name, request);
            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}