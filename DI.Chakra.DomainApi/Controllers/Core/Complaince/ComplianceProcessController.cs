using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ComplianceProcess")]
    public class ComplianceProcessController : BaseController<DomainModel.ComplianceProcess, EntityModel.ComplianceProcess, Dto.ComplianceProcess>
    {
        [HttpGet]
        [Route("GetByIdIncludePolicyExecutionDefinition")]
        public Dto.ComplianceProcess GetByIdIncludePolicyExecutionDefinition(long id) 
        {
            return DtoMap(DomainModel.ComplianceProcess.GetByIdIncludePolicyExecutionDefinition(id));
        }

        [HttpGet]
        [Route("DoesExistsByProcessName")]
        public bool DoesExistsByProcessName(string processName)
        {
            return DomainModel.ComplianceProcess.DoesExistsByProcessName(processName);
        }

        [HttpGet]
        [Route("DeleteComplianceProcess")]
        public void DeleteComplianceProcess(long processId)
        {
            DomainModel.ComplianceProcess.DeleteComplianceProcess(processId);
        }
    }
}
