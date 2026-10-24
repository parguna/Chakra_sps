using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Scheduler")]
    public class SchedulerController : BaseController<DomainModel.Scheduler, EntityModel.Scheduler, Dto.Scheduler>
    {
        [HttpGet]
        [Route("GetDiscoveryScheduler")]
        public Dto.Scheduler GetDiscoveryScheduler(long discoveryProcessId)
        {
            return DtoMap(DomainModel.Scheduler.GetDiscoveryScheduler(discoveryProcessId));
        }

        [HttpGet]
        [Route("GetComplianceScheduler")]
        public Dto.Scheduler GetComplianceScheduler(long complianceProcessId)
        {
            return DtoMap(DomainModel.Scheduler.GetComplianceScheduler(complianceProcessId));
        }

        [HttpGet]
        [Route("GetAttestationScheduler")]
        public Dto.Scheduler GetAttestationScheduler(long attestationProcessId)
        {
            return DtoMap(DomainModel.Scheduler.GetAttestationScheduler(attestationProcessId));
        }

    }
}
