using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHRFLINSSuppRoleMatrix")]
    public class SAPHRFLINSSuppRoleMatrixController: BaseController<DomainModel.SAPHRFLINSSuppRoleMatrix, EntityModel.SAPHRFLINSSuppRoleMatrix, Dto.SAPHRFLINSSuppRoleMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPHRFLINSSuppRoleMatrices")]
        public List<Dto.SAPHRFLINSSuppRoleMatrix> GetAllSAPHRFLINSRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPHRFLINSSuppRoleMatrix.GetAllSAPHRFLINSSuppRoleMatrices(isValidRecords, isAllRecords));
        }
    }
}