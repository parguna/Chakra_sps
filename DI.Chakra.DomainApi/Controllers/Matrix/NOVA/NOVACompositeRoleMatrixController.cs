using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOVACompositeRoleMatrix")]
    public class NOVACompositeRoleMatrixController : BaseController<DomainModel.NOVACompositeRoleMatrix, EntityModel.NOVACompositeRoleMatrix, Dto.NOVACompositeRoleMatrix>
    {
        [HttpGet]
        [Route("GetNOVACompositeRoleMatrices")]
        public List<Dto.NOVACompositeRoleMatrix> GetNOVACompositeRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NOVACompositeRoleMatrix.GetNOVACompositeRoleMatrices(isValidRecords, isAllRecords));
        }
    }
}