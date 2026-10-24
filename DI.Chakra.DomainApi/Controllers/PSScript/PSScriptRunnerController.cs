using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PSScriptRunner")]
    public class PSScriptRunnerController:ApiController
    {
        [HttpGet]
        [Route("Run")]
        public bool Run(string file,string Error)
        {
            return DomainModel.PSScriptRunner.Run(file, Error);
        }
    }
}
