using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPSRMEnablerRoleMatrix")]
    public class SAPSRMEnablerRoleMatrixController : BaseController<DomainModel.SAPSRMEnablerRoleMatrix, EntityModel.SAPSRMEnablerRoleMatrix, Dto.SAPSRMEnablerRoleMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPSRMEnablerMatrix")]
        public List<Dto.SAPSRMEnablerRoleMatrix> GetAllSAPSRMEnablerMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPSRMEnablerRoleMatrix.GetAllSAPSRMEnablerMatrix(isValidRecords, isAllRecords));
        }
    }
}