using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using VisXpression3Builder.Api.VXB;
using VisXpression3Builder.Lib;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Api.Controllers
{
    public class StaticFunctionsController : ApiController
    {
        private LibraryFacade VxbFacade;
        public StaticFunctionsController()
        {
            VxbFacade = new LibraryFacade(new UserDefinedFunctionsRepository(), new StaticFunctionsRepository());
        }
        [ResponseType(typeof(IEnumerable<FunctionDeclaration>))]
        public IHttpActionResult Get()
        {
            return Ok(VxbFacade.GetStaticFunctions());
        }
    }
}
