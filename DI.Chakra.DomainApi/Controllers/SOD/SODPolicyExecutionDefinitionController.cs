using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODPolicyExecutionDefinition")]
    public class SODPolicyExecutionDefinitionController : BaseController<DomainModel.SODPolicyExecutionDefinition, EntityModel.SODPolicyExecutionDefinition, Dto.SODPolicyExecutionDefinition>
    {
        [HttpGet]
        [Route("GetAllByIncludeCompliancePolicy")]
        public List<SODPolicyExecutionDefinition> GetAllByIncludeCompliancePolicy()
        {
            return DtoMapList(DomainModel.SODPolicyExecutionDefinition.GetAllByIncludeCompliancePolicy());
        }

        [HttpGet]
        [Route("GetByIdIncludingCompliance")]
        public SODPolicyExecutionDefinition GetByIdIncludingCompliance(long id)
        {
            return DtoMap(DomainModel.SODPolicyExecutionDefinition.GetByIdIncludingCompliance(id));
        }

        [HttpGet]
        [Route("HasPolicy")]
        public bool HasPolicy(long policyId)
        {
            return DomainModel.SODPolicyExecutionDefinition.HasPolicy(policyId);
        }
        [HttpGet]
        [Route("DoesExists")]
        public static bool DoesExists(string UniqueRunCode)
        {
            return DomainModel.SODPolicyExecutionDefinition.DoesExists(UniqueRunCode);
        }
        [HttpGet]
        [Route("HasException")]
        public bool HasException(long exceptionId)
        {
            return DomainModel.SODPolicyExecutionDefinition.HasException(exceptionId);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.SODPolicyExecutionDefinition.GetBySearchString(searchString);
        }


        [HttpGet]
        [Route("SODPolicyDeleteById")]
        public void SODPolicyDeleteById(long sodPolicyExecutionId)
        {
             DomainModel.SODPolicyExecutionDefinition.SODPolicyDeleteById(sodPolicyExecutionId);
        }



    }
}
