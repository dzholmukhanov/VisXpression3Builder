using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using VisXpression3Builder.Api.VXB;
using VisXpression3Builder.Lib;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Api.Controllers
{
    public class BasicFunctionsController : ApiController
    {
        private LibraryFacade VxbFacade;
        public BasicFunctionsController()
        {
            VxbFacade = new LibraryFacade(new UserDefinedFunctionsRepository(), new StaticFunctionsRepository(), new DomainFunctionsRepository());
        }
        [ResponseType(typeof(IEnumerable<FunctionDeclaration>))]
        public IHttpActionResult Get()
        {
            return Ok(VxbFacade.GetBasicFunctions());
        }
    }
}
