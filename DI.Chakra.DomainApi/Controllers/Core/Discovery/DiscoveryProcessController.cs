using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("DiscoveryProcess")]
    public class DiscoveryProcessController : BaseController<DomainModel.DiscoveryProcess, EntityModel.DiscoveryProcess, Dto.DiscoveryProcess>
    {

        [HttpGet]
        [Route("GetByIdIncludeEndpoint")]
        public Dto.DiscoveryProcess GetByIdIncludeEndpoint(long id)
        {
            return DtoMap(DomainModel.DiscoveryProcess.GetByIdIncludeEndpoint(id));
        }

        [HttpGet]
        [Route("DoesProcessNameAlreadyExists")]
        public bool DoesProcessNameAlreadyExists(string processName)
        {
            return DomainModel.DiscoveryProcess.DoesProcessNameAlreadyExists(processName);
        }

        [HttpGet]
        [Route("DeleteDiscoveryProcess")]
        public bool DeleteDiscoveryProcess(long  processId)
        {
           return DomainModel.DiscoveryProcess.DeleteDiscoveryProcess(processId,User.Identity.Name);
        }

        [HttpGet]
        [Route("GetDiscoveryProcessByProcessId")]
        public Dto.DiscoveryProcess GetDiscoveryProcessByProcessId(long processId)
        {

            var discoveryProcess = DomainModel.DiscoveryProcess.Get(d => d.Id == processId, true, 0, 0).FirstOrDefault();

            discoveryProcess.process = DomainModel.Process.GetByProcessTypeKeyId(processId, Infrastructure.Enums.ProcessType.Discovery);

            return DtoMap(discoveryProcess);
        }


         


    }
}
