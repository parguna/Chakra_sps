using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODConflictFunctions")]
    public class SODConflictFunctionsController : BaseController<DomainModel.SODConflictFunctions, EntityModel.SODConflictFunctions, Dto.SODConflictFunctions>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string sodConflictCode)
        {
            return DomainModel.SODConflictFunctions.DoesExists(sodConflictCode);
        }

        [HttpGet]
        [Route("HasRiskCode")]
        public bool HasRiskCode(long riskCodeId)
        {
            return DomainModel.SODConflictFunctions.HasRiskCode(riskCodeId);
        }

        [HttpGet]
        [Route("HasFunction")]
        public bool HasFunction(long functionId)
        {
            return DomainModel.SODConflictFunctions.HasFunction(functionId);
        }

        [HttpGet]
        [Route("DeleteBySodConflictCode")]
        public void DeleteBySodConflictCode(string sodConflictCode)
        {
            DomainModel.SODConflictFunctions.DeleteBySodConflictCode(sodConflictCode);
        }

        [HttpGet]
        [Route("GetByIdWithNavigation")]
        public SODConflictFunctions GetByIdWithNavigation(long id)
        {
            return DtoMap(DomainModel.SODConflictFunctions.GetByIdWithNavigation(id));
        }


        [HttpGet]
        [Route("GetByIdIncludingSODRiskCode")]

        public SODConflictFunctions GetByIdIncludingSODRiskCode(long id)
        {
            return DtoMap(DomainModel.SODConflictFunctions.GetByIdIncludingSODRiskCode(id));
        }

        [HttpGet]
        [Route("GetAllByIncludeSodRiskCode")]

        public List<SODConflictFunctions> GetAllByIncludeSodRiskCode()
        {
            return DtoMapList(DomainModel.SODConflictFunctions.GetAllByIncludeSodRiskCode());
        }

        [HttpGet]
        [Route("GetBySodConflictCode")]
        public Dictionary<long, string> GetBySodConflictCode(string searchString, string filterString)
        {
            return DomainModel.SODConflictFunctions.GetBySodConflictCode(searchString, filterString);
        }
    }
}