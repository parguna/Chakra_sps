using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPRoleMatrix")]
    public class SAPRoleMatrixController : BaseController<DomainModel.SAPRoleMatrix, EntityModel.SAPRoleMatrix, Dto.SAPRoleMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPRoleEligibilityMatrices")]
        public List<Dto.SAPRoleMatrix> GetAllSAPRoleEligibilityMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPRoleMatrix.GetAllSAPRoleEligibilityMatrices(isValidRecords, isAllRecords));
        }
    }
}