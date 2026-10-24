using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationProcess")]
    public class AttestationProcessController : BaseController<DomainModel.AttestationProcess, EntityModel.AttestationProcess, Dto.AttestationProcess>
    {
        [HttpGet]
        [Route("GetByAttestationId")]
        public Dto.AttestationProcess GetByAttestationId(long attestationId)
        {
            return DtoMap(DomainModel.AttestationProcess.GetByAttestationId(attestationId));
        }

        [HttpGet]
        [Route("GetByIdIncludeAttestation")]
        public Dto.AttestationProcess GetByIdIncludeAttestation(long id)
        {
            return DtoMap(DomainModel.AttestationProcess.GetByIdIncludeAttestation(id));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string processName) 
        {
            return DomainModel.AttestationProcess.DoesExists(processName);
        }

        [HttpGet]
        [Route("DoesExistsByWorkflowStatus")]
        public bool DoesExistsByWorkflowStatus(string processName,string workflowStatus)
        {
            return DomainModel.AttestationProcess.DoesExistsByWorkflowStatus(processName, workflowStatus);
        }

        [HttpGet]
        [Route("DeleteAttestationProcess")]
        public bool DeleteAttestationProcess(long processId)
        {
           return DomainModel.AttestationProcess.DeleteAttestationProcess(processId);
        }
        [HttpGet]
        [Route("IsCampaignRunningForAttestation")]
        public bool IsCampaignRunningForAttestation(long attestationId)
        {
            return DomainModel.AttestationProcess.IsCampaignRunningForAttestation(attestationId);
        }
    }
}
