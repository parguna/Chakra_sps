using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOVAExceptionalRoleMatrix")]
    public class NOVAExceptionalRoleMatrixController : BaseController<DomainModel.NOVAExceptionalRoleMatrix, EntityModel.NOVAExceptionalRoleMatrix, Dto.NOVAExceptionalRoleMatrix>
    {
        [HttpGet]
        [Route("GetNOVAExceptionalRoleMatrix")]
        public List<Dto.NOVAExceptionalRoleMatrix> GetNOVAExceptionalRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NOVAExceptionalRoleMatrix.GetNOVAExceptionalRoleMatrix(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetExceptionalMatrixCompanyCode")]
        public Dictionary<string,string> GetExceptionalMatrixCompanyCode(long endpointSystemId, AccountType accountType)
        {
            return DomainModel.NOVAExceptionalRoleMatrix.GetExceptionalMatrixCompanyCode(endpointSystemId, accountType);
        }
        [HttpGet]
        [Route("GetByCompanyCode")]
        public List<Dto.NOVAExceptionalRoleMatrix> GetByCompanyCode(long endpointSystemId,string companyCode,AdHocActionType adhocActionType,long endpointUserId, AccountType accountType, long userId)
        {
            return DtoMapList(DomainModel.NOVAExceptionalRoleMatrix.GetByCompanyCode(endpointSystemId, companyCode, adhocActionType, endpointUserId, accountType, userId));
        }
    }
}