using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOVAUserRoleMatrix")]
    public class NOVAUserRoleMatrixController: BaseController<DomainModel.NOVAUserRoleMatrix, EntityModel.NOVAUserRoleMatrix, Dto.NOVAUserRoleMatrix>
    {
        [HttpGet]
        [Route("GetNOVAUserRoleMatrices")]
        public List<Dto.NOVAUserRoleMatrix> GetNOVAUserRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NOVAUserRoleMatrix.GetNOVAUserRoleMatrices(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetExisitingRequestAndRole")]
        public List<Dto.NOVAUserRoleMatrix> GetExisitingRequestAndRole(AdHocActionType adhocActionType, long DIUserId, long endpointId, string SAPSection)
        {
            return DtoMapList(DomainModel.NOVAUserRoleMatrix.GetExisitingRequestAndRole(adhocActionType, DIUserId, endpointId, SAPSection));
        }

         

        [HttpGet]
        [Route("GetSAPSection")]
        public List<string> GetSAPSection(AdHocActionType adhocActionType, long DIUserId,long endpointId)
        {
            return DomainModel.NOVAUserRoleMatrix.GetSAPSection(adhocActionType, DIUserId,endpointId) ;
        }
    }
}