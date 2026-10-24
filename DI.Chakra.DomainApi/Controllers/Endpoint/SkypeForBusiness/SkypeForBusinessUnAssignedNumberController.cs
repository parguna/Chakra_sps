using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SkypeForBusinessUnAssignedNumber")]
    public class SkypeForBusinessUnAssignedNumberController : BaseController<DomainModel.SkypeForBusinessUnAssignedNumber, EntityModel.SkypeForBusinessUnAssignedNumber, Dto.SkypeForBusinessUnAssignedNumber>
    {
        [HttpGet]
        [Route("CheckNumberGeneratedAndAssigned")]
        public bool CheckNumberGeneratedAndAssigned(long endpointId,long id)
        {
            return DomainModel.SkypeForBusinessUnAssignedNumber.CheckNumberGeneratedAndAssigned(endpointId,id);
        }
        [HttpGet]
        [Route("DeleteRangeAndGeneratedMatrix")]
        public bool DeleteRangeAndGeneratedMatrix(long endpointId, long id)
        {
            return DomainModel.SkypeForBusinessUnAssignedNumber.DeleteRangeAndGeneratedMatrix(endpointId, id);
        }
        
        
    }
}