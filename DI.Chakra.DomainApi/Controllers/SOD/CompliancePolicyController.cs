using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("CompliancePolicy")]
    public class CompliancePolicyController : BaseController<DomainModel.CompliancePolicy, EntityModel.CompliancePolicy, Dto.CompliancePolicy>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string compliancePolicyCode)
        {
            return DomainModel.CompliancePolicy.DoesExists(compliancePolicyCode);
        }

        [HttpGet]
        [Route("HasConflictFunction")]
        public bool HasConflictFunction(long conflictFunctionId)
        {
            return DomainModel.CompliancePolicy.HasConflictFunction(conflictFunctionId);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.CompliancePolicy.GetBySearchString(searchString);
        }

        [HttpGet]
        [Route("GetByIdWithNavigation")]
        public CompliancePolicy GetByIdWithNavigation(long id)
        {
            return DtoMap(DomainModel.CompliancePolicy.GetByIdWithNavigation(id));
        }
    }
}
