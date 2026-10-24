using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Process")]
    public class ProcessController : BaseController<DomainModel.Process, EntityModel.Process, Dto.Process>
    {

        [HttpGet]
        [Route("GetByProcessType")]
        public Dto.Process GetByProcessType(long ProcessId, ProcessType processType)
        {
            return DtoMap(DomainModel.Process.GetByProcessType(ProcessId,processType));
        }

        [HttpGet]
        [Route("DeleteAllNavigation")]
        public bool DeleteAllNavigation(long attestationProcessId, ProcessType processType)
        {
            return DomainModel.Process.DeleteAllNavigation(attestationProcessId, processType, User.Identity.Name);
        }
    }
}
